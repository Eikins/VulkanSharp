namespace VulkanSharp.API
{
    public delegate void DebugCallback(MessageSeverity severity, string? message);

    public struct InstanceCreateInfo
    {
        public ApplicationInfo ApplicationInfo;
        public List<string> EnabledExtensions;
        public List<string> EnabledLayers;
        public DebugCallback? DebugCallback;
        public MessageSeverity? DebugMessageSeverityMask;
    }
}
