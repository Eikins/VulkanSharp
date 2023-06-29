namespace VulkanSharp.API
{
    public struct Version
    {
        public uint Major;
        public uint Minor;
        public uint Patch;

        public Version(uint major, uint minor, uint patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }
    }
}
