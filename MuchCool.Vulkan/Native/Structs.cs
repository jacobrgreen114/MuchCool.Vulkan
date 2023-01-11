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
public unsafe struct VkImageBlit {
    // todo : implement
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkPhysicalDeviceMemoryProperties {
    // todo : implement
}