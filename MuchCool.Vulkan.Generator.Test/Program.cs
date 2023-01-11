
using MuchCool.Vulkan.Generator;


internal class Program {
    private static void Main() {
        var generator = new SourceGenerator();
        generator.GenerateToFiles();
    }
}