namespace VulkanSharp.API
{
    [Flags]
    public enum QueueType : int
    {
        Graphics = 1,
        Compute = 2,
        Transfer = 4
    }
}
