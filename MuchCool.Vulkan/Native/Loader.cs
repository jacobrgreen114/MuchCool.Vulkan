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
using System.Security;

namespace MuchCool.Vulkan.Native;

internal static class Dll {
    internal const string            NAME   = "vulkan-1";
    internal const CallingConvention CONVENTION = CallingConvention.Winapi;
    internal const CharSet           CHAR_SET   = CharSet.Ansi;
}

public static class Loader {
    [SuppressUnmanagedCodeSecurity]
    [DllImport(Dll.NAME, EntryPoint = "vkGetInstanceProcAddr", CallingConvention = Dll.CONVENTION, CharSet = Dll.CHAR_SET)]
    private static extern unsafe void* _GetInstanceProcAddr(VkInstance instance, string pName);  
    
    [SuppressUnmanagedCodeSecurity]
    [DllImport(Dll.NAME, EntryPoint = "vkGetDeviceProcAddr", CallingConvention = Dll.CONVENTION, CharSet = Dll.CHAR_SET)]
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