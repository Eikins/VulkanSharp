namespace VulkanSharp.API
{
    public interface IVulkan
    {
        Instance CreateInstance(in InstanceCreateInfo createInfo);
    }
}
