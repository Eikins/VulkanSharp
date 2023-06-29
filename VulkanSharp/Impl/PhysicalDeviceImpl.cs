using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using System.Runtime.InteropServices;
using SilkVk = Silk.NET.Vulkan;

namespace VulkanSharp.API
{
    public class PhysicalDeviceImpl : PhysicalDevice
    {
        private Vk Vk { get; }
        private SilkVk.PhysicalDevice PhysicalDevice { get; }

        public bool HasGraphicsQueue => GraphicsQueueIndex != -1;
        public bool HasComputeQueue => ComputeQueueIndex != -1;
        public bool HasTransferQueue => TransferQueueIndex != -1;

        public int GraphicsQueueIndex { get; private set; }
        public int ComputeQueueIndex { get; private set; }
        public int TransferQueueIndex { get; private set; }

        public PhysicalDeviceImpl(Vk vk, in SilkVk.PhysicalDevice physicalDevice)
        {
            Vk = vk;
            PhysicalDevice = physicalDevice;
            FetchQueues();
        }

        private void FetchQueues()
        {
            GraphicsQueueIndex = -1;
            ComputeQueueIndex = -1;
            TransferQueueIndex = -1;

            unsafe
            {
                uint queueFamilityCount = 0;
                Vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, ref queueFamilityCount, null);

                var queueFamilies = new QueueFamilyProperties[queueFamilityCount];
                fixed (QueueFamilyProperties* queueFamiliesPtr = queueFamilies)
                {
                    Vk.GetPhysicalDeviceQueueFamilyProperties(PhysicalDevice, ref queueFamilityCount, queueFamiliesPtr);
                }

                for (int i = 0; i < queueFamilies.Length; i++)
                {
                    var flags = queueFamilies[i].QueueFlags;
                    if (flags.HasFlag(QueueFlags.GraphicsBit) && flags.HasFlag(QueueFlags.TransferBit))
                    {
                        GraphicsQueueIndex = i;
                    }
                    else if (flags.HasFlag(QueueFlags.ComputeBit))
                    {
                        ComputeQueueIndex = i;
                    }
                    else if (flags.HasFlag(QueueFlags.TransferBit))
                    {
                        TransferQueueIndex = i;
                    }
                }
            }

        }
    }
}
