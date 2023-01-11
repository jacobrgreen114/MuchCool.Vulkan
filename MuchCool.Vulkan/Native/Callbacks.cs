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

using size_t = System.UInt64;
using VkBool32 = System.UInt32;

namespace MuchCool.Vulkan.Native;

public unsafe delegate void* PFN_vkAllocationFunction(
    void* pUserData,
    size_t                                                  size,
    size_t                                                  alignment,
    VkSystemAllocationScope                                 allocationScope);
    
public unsafe delegate void* PFN_vkReallocationFunction(
    void*                   pUserData,
    void*                   pOriginal,
    size_t                  size,
    size_t                  alignment,
    VkSystemAllocationScope allocationScope);
    
public unsafe delegate void PFN_vkFreeFunction(
    void* pUserData,
    void* pMemory);
    
public unsafe delegate void PFN_vkInternalAllocationNotification(
    void*                    pUserData,
    size_t                   size,
    VkInternalAllocationType allocationType,
    VkSystemAllocationScope  allocationScope);
    
public unsafe delegate void PFN_vkInternalFreeNotification(
    void*                    pUserData,
    size_t                   size,
    VkInternalAllocationType allocationType,
    VkSystemAllocationScope  allocationScope);
    
public unsafe delegate VkBool32 PFN_vkDebugUtilsMessengerCallbackEXT(
    VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
    VkDebugUtilsMessageTypeFlagsEXT        messageTypes,
    VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
    void*                                       pUserData);

public unsafe delegate void PFN_vkDeviceMemoryReportCallbackEXT(
    VkDeviceMemoryReportCallbackDataEXT* pCallbackData,
    void*                                pUserData);
