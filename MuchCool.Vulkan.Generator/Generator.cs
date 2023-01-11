using Microsoft.CodeAnalysis;
using VulkGen;

namespace MuchCool.Vulkan.Generator; 

[Generator]
public class Generator : ISourceGenerator {
    private const string XML_PATH = "C:\\Users\\Jacob\\Desktop\\vk.xml";

    private readonly VulkanRegistry _registry;

    public Generator() {
        _registry = new VulkanRegistry(XmlVulkanRegistry.FromFile(XML_PATH));
    }
    
    public void Initialize(GeneratorInitializationContext context) {
        
    }

    public void Execute(GeneratorExecutionContext context) {
        SourceGenerator.Generate(_registry);
    }
}