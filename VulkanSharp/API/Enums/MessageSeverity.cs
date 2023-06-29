namespace VulkanSharp.API
{
    [Flags]
    public enum MessageSeverity : int
    {
        Verbose = 0x1,
        Info = 0x10,
        Warning = 0x100,
        Error = 0x1000
    }
}
