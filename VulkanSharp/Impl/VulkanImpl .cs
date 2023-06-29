using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using System.Runtime.InteropServices;
using SilkVk = Silk.NET.Vulkan;

namespace VulkanSharp.API
{
    public class VulkanImpl : IVulkan
    {
        private Vk Vk { get; }

        public VulkanImpl()
        {
            Vk = Vk.GetApi();
        }

        public Instance CreateInstance(in InstanceCreateInfo createInfo)
        {
            Instance instance;
            unsafe
            {
                var appInfo = createInfo.ApplicationInfo;
                bool areValidationLayersEnabled = createInfo.EnabledLayers.Count > 0;

                if (areValidationLayersEnabled && !CheckValidationLayerSupport(createInfo.EnabledLayers))
                {
                    throw new Exception("Validation layers are not supported on this device.");
                }

                if (areValidationLayersEnabled)
                {
                    createInfo.EnabledExtensions.Add(ExtDebugUtils.ExtensionName);
                }

                SilkVk.ApplicationInfo vkAppInfo = new()
                {
                    SType = StructureType.ApplicationInfo,
                    PApplicationName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.ApplicationName),
                    ApplicationVersion = new Version32(appInfo.ApplicationVersion.Major, appInfo.ApplicationVersion.Minor, appInfo.ApplicationVersion.Patch),
                    PEngineName = (byte*)Marshal.StringToHGlobalAnsi(appInfo.EngineName),
                    EngineVersion = new Version32(appInfo.EngineVersion.Major, appInfo.EngineVersion.Minor, appInfo.EngineVersion.Patch),
                    ApiVersion = Vk.Version12
                };

                SilkVk.InstanceCreateInfo vkCreateInfo = new()
                {
                    SType = StructureType.InstanceCreateInfo,
                    PApplicationInfo = &vkAppInfo,
                    EnabledExtensionCount = (uint) createInfo.EnabledExtensions.Count,
                    EnabledLayerCount = (uint) createInfo.EnabledLayers.Count,
                };

                if (createInfo.EnabledExtensions.Count > 0)
                {
                    vkCreateInfo.PpEnabledExtensionNames = (byte**)SilkMarshal.StringArrayToPtr(createInfo.EnabledExtensions);
                }

                if (areValidationLayersEnabled)
                {
                    var debugCallback = createInfo.DebugCallback;
                    DebugUtilsMessengerCreateInfoEXT debugCreateInfo = new()
                    {
                        SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                        MessageSeverity = (DebugUtilsMessageSeverityFlagsEXT) createInfo.DebugMessageSeverityMask.GetValueOrDefault(),
                        MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                                 DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt |
                                 DebugUtilsMessageTypeFlagsEXT.ValidationBitExt,
                        PfnUserCallback = (DebugUtilsMessengerCallbackFunctionEXT)((severity, type, pCallbackData, pUserData) =>
                        {
                            var message = Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage);
                            debugCallback?.Invoke((MessageSeverity) severity, message);
                            return Vk.False;
                        })
                    };

                    vkCreateInfo.PpEnabledLayerNames = (byte**)SilkMarshal.StringArrayToPtr(createInfo.EnabledLayers);
                    vkCreateInfo.PNext = &debugCreateInfo;
                }

                if (Vk.CreateInstance(vkCreateInfo, null, out var vkInstance) != Result.Success)
                {
                    throw new Exception("Instance creation failed.");
                }

                instance = new InstanceImpl(Vk, vkInstance);

                Marshal.FreeHGlobal((IntPtr) vkAppInfo.PApplicationName);
                Marshal.FreeHGlobal((IntPtr) vkAppInfo.PEngineName);
                SilkMarshal.Free((nint)vkCreateInfo.PpEnabledExtensionNames);
                SilkMarshal.Free((nint)vkCreateInfo.PpEnabledLayerNames);
            }

            return instance;
        }


        private bool CheckValidationLayerSupport(List<string> validationLayers)
        {
            bool result;
            unsafe
            {
                uint layerCount = 0;
                Vk.EnumerateInstanceLayerProperties(ref layerCount, null);
                var availableLayers = new LayerProperties[layerCount];
                fixed (LayerProperties* availableLayersPtr = availableLayers)
                {
                    Vk.EnumerateInstanceLayerProperties(ref layerCount, availableLayersPtr);
                }

                var availableLayerNames = availableLayers.Select(layer => Marshal.PtrToStringAnsi((IntPtr)layer.LayerName)).ToHashSet();
                result = validationLayers.All(availableLayerNames.Contains);
            }

            return result;
        }
    }
}
