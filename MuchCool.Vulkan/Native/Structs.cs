/* Copyright (c) 2023 Jacob R. Green
 *
 * This file is part of MuchCool.Vulkan.
 *
 * MuchCool.Vulkan is free software: you can redistribute it and/or modify it under the terms of the GNU General
 * Public License as published by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * MuchCool.Vulkan is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even
 * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with MuchCool.Vulkan. If not,
 * see <https://www.gnu.org/licenses/>.
 */

using System.Runtime.InteropServices;

namespace MuchCool.Vulkan.Native;


public struct Bool32 {
    private uint _value;

    public static bool operator !(Bool32 b) {
        return b._value == VK_FALSE;
    }

    public static bool operator true(Bool32 b) {
        return b._value != VK_FALSE;
    }

    public static bool operator false(Bool32 b) {
        return b._value == VK_FALSE;
    }

    private const          uint   VK_TRUE  = 1;
    private const          uint   VK_FALSE = 0;
    public static readonly Bool32 True     = new Bool32 { _value = VK_TRUE };
    public static readonly Bool32 False    = new Bool32 { _value = VK_FALSE };

}

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

[StructLayout(LayoutKind.Sequential)]
public struct VkImageBlit {
    public VkImageSubresourceLayers srcSubresource;
    public VkOffset3D               srcOffsets0;
    public VkOffset3D               srcOffsets1;
    public VkImageSubresourceLayers dstSubresource;
    public VkOffset3D               dstOffsets0;
    public VkOffset3D               dstOffsets1;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkPhysicalDeviceMemoryProperties {
    // todo : implement
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkPhysicalDeviceGroupProperties {
    public       VkStructureType sType;
    public       void*           pNext;
    public       uint            physicalDeviceCount;
    public fixed ulong           physicalDevices[32];
    public       uint            subsetAllocation;
}

