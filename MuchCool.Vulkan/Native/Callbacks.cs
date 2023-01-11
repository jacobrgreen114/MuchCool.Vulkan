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
