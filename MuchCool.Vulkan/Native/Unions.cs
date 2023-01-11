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

