using System.Runtime.InteropServices;

namespace MuchCool.Vulkan.Native;

public class Loader {
    private const string DllName = "vulkan-1";

    [DllImport(DllName, EntryPoint = "vkGetInstanceProcAddr", CharSet = CharSet.Ansi)]
    private static extern unsafe void* _GetInstanceProcAddr(VkInstance instance, string pName);  
    
    [DllImport(DllName, EntryPoint = "vkGetDeviceProcAddr", CharSet = CharSet.Ansi)]
    private static extern unsafe void* _GetDeviceProcAddr(VkDevice device, string pName);


    public static unsafe T GetInstanceProcAddr<T>(VkInstance instance, string procName) where T : Delegate {
        var addr = _GetInstanceProcAddr(instance, procName);
        return Marshal.GetDelegateForFunctionPointer<T>((nint)addr);
    }    
    
    public static unsafe T GetDeviceProcAddr<T>(VkDevice device, string procName) where T : Delegate {
        var addr = _GetDeviceProcAddr(device, procName);
        return Marshal.GetDelegateForFunctionPointer<T>((nint)addr);
    }
}