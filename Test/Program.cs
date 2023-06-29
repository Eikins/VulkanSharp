using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using VulkanSharp.API;

//Create a window.
var options = WindowOptions.DefaultVulkan with
{
    Size = new Vector2D<int>(800, 600),
    Title = "Vulkan",
};

var window = Window.Create(options);
window.Load += OnLoad;
window.Update += OnUpdate;
window.Render += OnRender;

window.Run();
window.Dispose();

void OnRender(double obj)
{
}

void OnUpdate(double dt)
{
}

void OnLoad()
{
    IInputContext input = window.CreateInput();
    for (int i = 0; i < input.Keyboards.Count; i++)
    {
        input.Keyboards[i].KeyDown += KeyDown;
    }

    if (window.VkSurface is null)
    {
        throw new Exception("Windowing platform doesn't support Vulkan.");
    }

    IVulkan vk = new VulkanImpl();

    var createInfo = new InstanceCreateInfo()
    {
        ApplicationInfo = new ApplicationInfo()
        {
            ApplicationName = "TestVulkanApp",
            ApplicationVersion = new VulkanSharp.API.Version(1, 0, 0),
            EngineName = "TestVulkanEngine",
            EngineVersion = new VulkanSharp.API.Version(1, 0, 0)
        },
        EnabledExtensions = new List<string>(),
        EnabledLayers = new List<string>
        {
            "VK_LAYER_KHRONOS_validation"
        },
        DebugCallback = (severity, message) => Console.WriteLine(message),
        DebugMessageSeverityMask = MessageSeverity.Warning | MessageSeverity.Error
    };

    using (var instance = vk.CreateInstance(createInfo))
    {
        var physicalDevice = instance.GetPhysicalDevice((physicalDevice) =>
            physicalDevice.HasGraphicsQueue &&
            physicalDevice.HasComputeQueue &&
            physicalDevice.HasTransferQueue
        );
    }



}

void KeyDown(IKeyboard keyboard, Key key, int arg3)
{
    if (key == Key.Escape)
    {
        window.Close();
    }
}