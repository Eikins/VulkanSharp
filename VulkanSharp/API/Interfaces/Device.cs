namespace VulkanSharp.API
{
    public interface Device
    {
        Queue GetQueue(QueueType queueType);
    }
}
