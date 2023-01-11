
using System.Runtime.InteropServices;

namespace MuchCool.Vulkan.Native;

[StructLayout(LayoutKind.Explicit)]
public struct VkClearColorValue {
    [FieldOffset(0)] 
    public VkClearColorValueFloat floats;
    
    [FieldOffset(0)] 
    public VkClearColorValueInt ints;
    
    [FieldOffset(0)] 
    public VkClearColorValueUInt uints;
}

[StructLayout(LayoutKind.Sequential)]
public struct VkClearColorValueFloat {
    public float r, g, b, a;
}

[StructLayout(LayoutKind.Sequential)]
public struct VkClearColorValueInt {
    public int r, g, b, a;
}

[StructLayout(LayoutKind.Sequential)]
public struct VkClearColorValueUInt {
    public uint r, g, b, a;
}

[StructLayout(LayoutKind.Explicit)]
public struct VkClearValue {
    [FieldOffset(0)]
    private VkClearColorValue _value;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkDescriptorUpdateTemplate {
    private void* _handle;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkPipelineExecutableStatisticValueKHR {
    private ulong _value;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkDeviceOrHostAddressKHR {
    private void* _value;
}
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkDeviceOrHostAddressConstKHR {
    private void* _value;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkAccelerationStructureGeometryDataKHR {
    // todo : implement
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkDescriptorDataEXT {
    // todo : implement
}

