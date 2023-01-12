using System.Runtime.InteropServices;

namespace MuchCool.Vulkan.Native; 

public static class Vk {


    #region Instance

    public static unsafe VkInstance CreateInstance(PFN_vkCreateInstance proc, in VkInstanceCreateInfo createInfo) {
        var result   = proc(createInfo, null, out var instance);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return instance;
    }
    
    public static unsafe void DestroyInstance(PFN_vkDestroyInstance proc, VkInstance instance) {
        proc(instance, null);
    }

    #endregion

    #region Physical Device

    public static unsafe VkPhysicalDevice[] EnumeratePhysicalDevices(
        PFN_vkEnumeratePhysicalDevices proc, VkInstance instance
    ) {
        uint count  = default;
        var  result = proc(instance, ref count, null);
        if (result != VkResult.VK_SUCCESS) throw new Exception();

        var physicalDevices = new VkPhysicalDevice[count];
        fixed (VkPhysicalDevice* pDevice = physicalDevices)
            result = proc(instance, ref count, pDevice);
        if (result != VkResult.VK_SUCCESS) throw new Exception();

        return physicalDevices;
    }

    #endregion

    #region Device

    public static unsafe VkDevice CreateDevice(
        PFN_vkCreateDevice proc, VkPhysicalDevice physicalDevice, in VkDeviceCreateInfo createInfo
    ) {
        var result = proc(physicalDevice, createInfo, null, out var device);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return device;
    }

    public static unsafe void DestroyDevice(PFN_vkDestroyDevice proc, VkDevice device) {
        proc(device, null);
    }
    
    public static VkQueue GetDeviceQueue(PFN_vkGetDeviceQueue proc, VkDevice device, uint queueFamilyIndex, uint queueIndex) {
        proc(device, queueFamilyIndex, queueIndex, out var queue);
        return queue;
    }

    #endregion

    #region SurfaceKHR

    public static unsafe VkSurfaceKHR CreateWin32SurfaceKHR(
        PFN_vkCreateWin32SurfaceKHR proc, VkInstance instance, in VkWin32SurfaceCreateInfoKHR createInfo
    ) {
        var result = proc(instance, createInfo, null, out var surface);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return surface;
    }

    public static unsafe void DestroyWin32SurfaceKHR(
        PFN_vkDestroySurfaceKHR proc, VkInstance instance, VkSurfaceKHR surface
    ) {
        proc(instance, surface, null);
    }

    #endregion
    
    #region SwapchainKHR

    public static unsafe VkSwapchainKHR CreateSwapchainKHR(
        PFN_vkCreateSwapchainKHR proc, VkDevice device, in VkSwapchainCreateInfoKHR createInfo
    ) {
        var result = proc(device, createInfo, null, out var swapchain);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return swapchain;
    }

    public static unsafe void DestroySwapchainKHR(
        PFN_vkDestroySwapchainKHR proc, VkDevice device, VkSwapchainKHR swapchain
    ) {
        proc(device, swapchain, null);
    }

    public static unsafe VkImage[] GetSwapchainImagesKHR(PFN_vkGetSwapchainImagesKHR proc, VkDevice device, VkSwapchainKHR swapchain) {
        uint count  = 0;
        var  result = proc(device, swapchain, ref count, null);
        if (result != VkResult.VK_SUCCESS) throw new Exception();

        var images = new VkImage[count];
        fixed (VkImage* pImages = images)
            result = proc(device, swapchain, ref count, pImages);
        if (result != VkResult.VK_SUCCESS) throw new Exception();

        return images;
    }

    #endregion

    #region ImageView

    public static unsafe VkImageView CreateImageView(
        PFN_vkCreateImageView proc, VkDevice device, in VkImageViewCreateInfo createInfo
    ) {
        var result = proc(device, createInfo, null, out var imageView);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return imageView;
    }

    public static unsafe void DestroyImageView(PFN_vkDestroyImageView proc, VkDevice device, VkImageView imageView) {
        proc(device, imageView, null);
    }

    #endregion

    #region Framebuffer

    public static unsafe VkFramebuffer CreateFramebuffer(
        PFN_vkCreateFramebuffer proc, VkDevice device, in VkFramebufferCreateInfo createInfo
    ) {
        var result = proc(device, createInfo, null, out var framebuffer);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return framebuffer;
    }

    public static unsafe void DestroyFramebuffer(
        PFN_vkDestroyFramebuffer proc, VkDevice device, VkFramebuffer framebuffer
    ) {
        proc(device, framebuffer, null);
    }

    #endregion
    
    #region Buffer

    public static unsafe VkBuffer CreateBuffer(PFN_vkCreateBuffer proc, VkDevice device, in VkBufferCreateInfo createInfo) {
        var result = proc(device, createInfo, null, out var buffer);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return buffer;
    }

    public static unsafe void DestroyBuffer(PFN_vkDestroyBuffer proc, VkDevice device, VkBuffer buffer) {
        proc(device, buffer, null);
    }
    
    #endregion

    #region Command Pool
    
    public static unsafe VkCommandPool CreateCommandPool(
        PFN_vkCreateCommandPool proc, VkDevice device, in VkCommandPoolCreateInfo createInfo
    ) {
        var result = proc(device, createInfo, null, out var commandPool);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
        return commandPool;
    }

    public static unsafe void DestroyCommandPool(
        PFN_vkDestroyCommandPool proc, VkDevice device, VkCommandPool commandPool
    ) {
        proc(device, commandPool, null);
    }

    
    public static unsafe void ResetCommandPool(
        PFN_vkResetCommandPool proc, VkDevice device, VkCommandPool pool, VkCommandPoolResetFlags flags = 0
    ) {
        var result = proc(device, pool, flags);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
    }

    #endregion
    
    #region Command Buffer

    #region Allocate / Free
    
    public static unsafe VkCommandBuffer[] AllocateCommandBuffers(
        PFN_vkAllocateCommandBuffers proc, VkDevice device, in VkCommandBufferAllocateInfo allocateInfo
    ) {
        var result         = VkResult.VK_SUCCESS;
        var commandBuffers = new VkCommandBuffer[allocateInfo.commandBufferCount];
        fixed (VkCommandBuffer* pBuffers = commandBuffers)
            result = proc(device, allocateInfo, pBuffers);
        if (result != VkResult.VK_SUCCESS) throw new Exception();

        return commandBuffers;
    }

    public static unsafe void FreeCommandBuffers(PFN_vkFreeCommandBuffers proc, VkDevice device, VkCommandPool commandPool, VkCommandBuffer[] commandBuffers) {
        fixed (VkCommandBuffer* pBuffers = commandBuffers)
            proc(device, commandPool, (uint)commandBuffers.Length, pBuffers);
    }

    #endregion
    
    #region Recording

    public static void BeginCommandBuffer(
        PFN_vkBeginCommandBuffer proc, VkCommandBuffer commandBuffer, in VkCommandBufferBeginInfo beginInfo
    ) {
        var result = proc(commandBuffer, beginInfo);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
    }
    
    public static void EndCommandBuffer(
        PFN_vkEndCommandBuffer proc, VkCommandBuffer commandBuffer
    ) {
        var result = proc(commandBuffer);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
    }

    public static void ResetCommandBuffer(
        PFN_vkResetCommandBuffer proc, VkCommandBuffer commandBuffer, VkCommandBufferResetFlags flags = 0
    ) {
        var result = proc(commandBuffer, flags);
        if (result != VkResult.VK_SUCCESS) throw new Exception();
    }
    
    #endregion
    
    #region Commands

    public static void CmdBeginRenderPass(PFN_vkCmdBeginRenderPass proc, VkCommandBuffer commandBuffer, in VkRenderPassBeginInfo beginInfo, VkSubpassContents subpassContents) {
        proc(commandBuffer, beginInfo, subpassContents);
    }

    public static void CmdEndRenderPass(PFN_vkCmdEndRenderPass proc, VkCommandBuffer commandBuffer) {
        proc(commandBuffer);
    }
    
    public static void CmdDraw(PFN_vkCmdDraw proc, VkCommandBuffer commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance) {
        proc(commandBuffer, vertexCount, instanceCount, firstVertex, firstInstance);
    }

    public static void CmdDrawIndexed(
        PFN_vkCmdDrawIndexed proc,        VkCommandBuffer commandBuffer, uint indexCount, uint instanceCount,
        uint                 firstIndex, int vertexOffset,  uint            firstInstance
    ) {
        proc(commandBuffer, indexCount, instanceCount, firstIndex, vertexOffset, firstInstance);
    }
    
    #endregion

    #endregion
}
