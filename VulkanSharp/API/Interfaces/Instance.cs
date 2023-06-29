namespace VulkanSharp.API
{
    public interface Instance : IDisposable
    {
        PhysicalDevice? GetPhysicalDevice(Predicate<PhysicalDevice> predicate);
        PhysicalDevice? GetPhysicalDevice();
    }
}
