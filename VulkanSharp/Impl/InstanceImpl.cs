using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using System.Runtime.InteropServices;
using SilkVk = Silk.NET.Vulkan;

namespace VulkanSharp.API
{
    public class InstanceImpl : Instance
    {
        private Vk Vk { get; }
        private SilkVk.Instance Instance { get; }

        public InstanceImpl(Vk vk, in SilkVk.Instance instance)
        {
            Vk = vk;
            Instance = instance;
        }

        public PhysicalDevice? GetPhysicalDevice(Predicate<PhysicalDevice> predicate)
        {
            PhysicalDevice? selectedDevice = null;

            unsafe
            {
                uint devicedCount = 0;
                Vk.EnumeratePhysicalDevices(Instance, ref devicedCount, null);

                if (devicedCount == 0)
                {
                    throw new Exception("failed to find GPUs with Vulkan support!");
                }

                var devices = new SilkVk.PhysicalDevice[devicedCount];
                fixed (SilkVk.PhysicalDevice* devicesPtr = devices)
                {
                    Vk.EnumeratePhysicalDevices(Instance, ref devicedCount, devicesPtr);
                }

                foreach (var device in devices)
                {
                    PhysicalDevice physicalDevice = new PhysicalDeviceImpl(Vk, device);
                    if (predicate(physicalDevice))
                    {
                        selectedDevice = physicalDevice;
                        break;
                    }
                }
            }

            return selectedDevice;

        }

        public PhysicalDevice? GetPhysicalDevice() => GetPhysicalDevice((physicalDevice) => physicalDevice.HasGraphicsQueue);

        public void Dispose()
        {
            unsafe
            {
                Vk.DestroyInstance(Instance, null);
            }
        }
    }
}
