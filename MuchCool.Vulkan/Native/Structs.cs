using System.Runtime.InteropServices;

namespace MuchCool.Vulkan.Native; 

//todo : move to manual handles
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkAccelerationStructureNV {
    private void* _handle;
}

// todo move out of here
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkAttachmentReference2 {
    // todo : implement
}

