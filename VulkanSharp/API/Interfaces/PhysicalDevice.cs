namespace VulkanSharp.API
{
    public interface PhysicalDevice
    {
        bool HasGraphicsQueue { get; }
        bool HasTransferQueue { get; }
        bool HasComputeQueue { get; }

        Device CreateDevice();
    }
}
