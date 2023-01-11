using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using MuchCool.Vulkan.Generator.Registry;
using MuchCool.Vulkan.Generator.Registry.Xml;

namespace MuchCool.Vulkan.Generator; 

[Generator]
internal class SourceGenerator : ISourceGenerator {
    private const  string VULKAN_XML_RELATIVE_PATH = "../External/Vulkan-Docs/xml/vk.xml";
    private static string _vulkanXmlPath          = FindVulkanXmlFile();

    private static string FindVulkanXmlFile([CallerFilePath] string? currentFile = null) {
        if (currentFile is null) return string.Empty;
        return Path.Combine(Path.GetDirectoryName(currentFile) ?? string.Empty, VULKAN_XML_RELATIVE_PATH);
    }
    
    
    internal const string VULKAN_NAMESPACE = "MuchCool.Vulkan.Native";
    
    private static readonly string[] EnabledFeatures = new[] {
        "VK_VERSION_1_0"
    };


    private static readonly string[] EnabledAuthors = new[] {
        "KHR", "EXT"
    };
    
    private static readonly string[] EnabledPlatforms = new[] {
        "win32"
    };
    

    private readonly VulkanRegistry _registry;

    public SourceGenerator() {
        _registry = new VulkanRegistry(XmlVulkanRegistry.FromFile(_vulkanXmlPath));
    }


    public void Initialize(GeneratorInitializationContext context) { }


    public void Execute(GeneratorExecutionContext context) {
        var enabledFeatures = 
            _registry.Features.Values.Where(f => EnabledFeatures.Contains(f.Name)).ToArray();
        
        var enabledExtensions =
            _registry.Extensions.Values
                .Where(e => EnabledAuthors.Contains(e.Author))
                .Where(e => e.Platform is null || EnabledPlatforms.Contains(e.Platform)).ToArray();

        var featuresTypes  = enabledFeatures.Select(f => f.RequiredTypes);
        var extensionTypes = enabledExtensions.Select(e => e.RequiredTypes);
        var enabledTypes =
            featuresTypes.Union(extensionTypes)
                .SelectMany(t => t)
                .Select(t => t.Name).ToArray();

        var featuresCommands  = enabledFeatures.Select(f => f.RequiredCommands);
        var extensionCommands = enabledExtensions.Select(e => e.RequiredCommands);
        var enabledCommands = 
            featuresCommands.Union(extensionCommands)
                .SelectMany(c => c)
                .Select(c => c.Name).ToArray();
        
        
        //TypeGenerator.Generate(_registry);
        HandlesGenerator.Generate(context, _registry, enabledTypes);
        StructGenerator.Generate(context, _registry, enabledTypes);
        EnumGenerator.Generate(context, _registry);
        //CommandGenerator.Generate(_registry, enabledCommands);
    }
    
    
    
}


public static class TypeGenerator {
    private static readonly Dictionary<string, string> NativeTypes = new() {
        {"uint8_t", "System.Byte"},
        {"int8_t", "System.SByte"},
        {"uint16_t", "System.UInt16"},
        {"int16_t", "System.Int16"},
        {"uint32_t", "System.UInt32"},
        {"int32_t", "System.Int32"},
        {"uint64_t", "System.UInt64"},
        {"int64_t", "System.Int64"},
        {"size_t", "System.UInt64"},
        
        {"VkSampleMask", "System.UInt32"},
        {"VkBool32", "System.UInt32"},
        {"VkFlags", "System.UInt32"},
        {"VkFlags64", "System.UInt64"},
        {"VkDeviceSize", "System.UInt64"},
        {"VkDeviceAddress", "System.UInt64"},
        
        {"DWORD", "System.UInt32"},
        {"HANDLE", "System.IntPtr"},
        {"HWND", "System.IntPtr"},
        {"HINSTANCE", "System.IntPtr"},
        {"HMONITOR", "System.IntPtr"}
    };


    internal static void Generate(VulkanRegistry registry) {
        var types = registry.Types.BaseTypes;

        var builder = new SourceBuilder();
        
        foreach (var type in NativeTypes)
            WriteType(builder, type.Key, type.Value);
        
        //foreach (var type in types) {
        //    WriteType(builder, type);
        //}

        using var sourceFile = new StreamWriter("VulkanTypes.g.cs");
        sourceFile.Write(builder.ToString());
    }

    private static void WriteType(SourceBuilder builder, VulkanBaseType type) {
        WriteType(builder, type.Name, type.Type);
    }

    private static void WriteType(SourceBuilder builder, string type, string nativeType) {
        builder.Write("global using ").Write(type).Write(" = ").Write(nativeType).WriteLine(';');
    }
}


public static class HandlesGenerator {
    private const string GENERATED_FILE = "VulkanHandles.g.cs";
    
    private static readonly string[] Usings = new[] {
        "System.Runtime.InteropServices"
    };
    
    internal static void Generate(GeneratorExecutionContext context, VulkanRegistry registry, IReadOnlyList<string> enabledTypes) {
        var handles = registry.Types.Handles;

        var builder = new SourceFile(SourceGenerator.VULKAN_NAMESPACE, Usings);

        foreach (var handle in handles.Where(h => enabledTypes.Contains(h.Name))) {
            GenerateHandle(builder, handle);
        }

        context.AddSource(GENERATED_FILE, builder.ToString());
    }

    
    private static void GenerateHandle(SourceFile builder, VulkanHandle handle) {
        builder.WriteAttribute("StructLayout(LayoutKind.Sequential)");
        builder.WriteStructStart(handle.Name, AccessModifier.Public, true);
        builder.WriteStructField("_handle", "void*", null, AccessModifier.Private);
        builder.WriteBlankLine();
        builder.WriteStructField("Null", handle.Name, "new()", AccessModifier.Public, true, true);
        builder.WriteStructEnd();
        builder.WriteBlankLine();
    }
}

public static class StructGenerator {
    private const string GENERATED_FILE = "VulkanStructs.g.cs";

    private static readonly string[] Usings = new[] {
        "System.Runtime.InteropServices"
    };
    
    public static void Generate(GeneratorExecutionContext context, VulkanRegistry registry, IReadOnlyList<string> enabledTypes) {
        var structs = registry.Types.Structs;

        var builder = new SourceFile(SourceGenerator.VULKAN_NAMESPACE, Usings);

        foreach (var s in structs.Where(s => enabledTypes.Contains(s.Name))) {
            WriteStruct(builder, s);
        }

        context.AddSource(GENERATED_FILE, builder.ToString());
    }


    private static void WriteStruct(SourceFile builder, VulkanStruct s) {
        builder.WriteAttribute("StructLayout(LayoutKind.Sequential)");
        builder.WriteStructStart(s.Name, AccessModifier.Public, true);
        
        foreach (var field in s.Fields) {
            WriteField(builder, field);
        }

        builder.WriteStructEnd();
        builder.WriteBlankLine();
    }

    private static void WriteField(SourceFile builder, VulkanField field) {
        var typename = CreateTypeName(field.TypeName, field.PointerDepth);
        builder.WriteStructField(field.Name, typename, null, AccessModifier.Public);
    }

    private static string CreateTypeName(string baseType, int pointerDepth) {
        var typeBuilder = new StringBuilder(baseType);
        typeBuilder.Replace("FlagBits", "Flags");
        for (int i = 0; i < pointerDepth; ++i) 
            typeBuilder.Append('*');

        return typeBuilder.ToString();
    }
}

public static class EnumGenerator {
    private const string GENERATED_FILE = "VulkanEnums.g.cs";

    private static readonly string[] Usings = new[] {
        "System"
    };
    
    public static void Generate(GeneratorExecutionContext context, VulkanRegistry registry) {
        var enums    = registry.Types.Enums;
        var bitmasks = registry.Types.Bitmasks;

        var builder = new SourceFile(SourceGenerator.VULKAN_NAMESPACE, Usings);

        foreach (var bitmask in bitmasks) {
            if (enums.ContainsKey(bitmask.Name)) continue;
            WriteBitmask(builder, bitmask);
        }

        builder.WriteComment("Enums");
        
        foreach (var e in enums) {
            WriteEnum(builder, e.Value);
        }

        context.AddSource(GENERATED_FILE, builder.ToString());
    }

    public static void WriteBitmask(SourceFile builder, VulkanBitmask bitmask) {
        //if (bitmask.Requires is not null) {
        //    Debugger.Break();
        //    return builder;
        //}
        
        builder.WriteAttribute("Flags");
        builder.WriteEnumStart(bitmask.Name, bitmask.TypeName, AccessModifier.Public);
        builder.WriteEnumEnd();
        builder.WriteBlankLine();
    }
    

    private static void WriteEnum(SourceFile builder, VulkanEnum e) {
        if (e.IsBitmask)
            builder.WriteAttribute("Flags");
        
        builder.WriteEnumStart(e.Name, e.TypeName, AccessModifier.Public);
        
        if (e.Enumeration is not null)
            foreach (var value in e.Enumeration.Value)
                WriteEnumerationValue(builder, value);

        builder.WriteEnumEnd();
        builder.WriteBlankLine();
    }

    private static void WriteEnumerationValue(SourceFile builder, VulkanEnumerationValue value) {
        builder.WriteEnumValue(value.Name, value.Value);
    }
}


public static class CommandGenerator {
    
    internal static void Generate(VulkanRegistry registry, IReadOnlyList<string> enabledCommands) {
        var commands = registry.Commands.Values;

        var builder = new SourceBuilder();

        foreach (var command in commands.Where(c => enabledCommands.Contains(c.Name)))
            WriteCommand(builder, command);

        using var sourceFile = new StreamWriter("VulkanCommands.g.cs");
        sourceFile.Write(builder.ToString());
    }

    private static void WriteCommand(SourceBuilder builder, VulkanCommand command) {
        var name = command.Name.Replace("vk", "PFN_vk");

        builder.WriteIndentation().Write("public unsafe delegate ").Write(command.ReturnType).Write(' ').Write(name)
            .Write('(');

        var count     = command.Parameters.Count;
        var lastIndex = count - 1;
        for (int i = 0; i < count; ++i) {
            WriteParameter(builder, command.Parameters[i]);
            if (i < lastIndex) builder.Write(", ");
        }

        builder.WriteLine(");");
    }

    private static void WriteParameter(SourceBuilder builder, VulkanCommandParameter parameter) {
        builder.Write(parameter.TypeName);

        for (int i = 0; i < parameter.PointerDepth; ++i)
            builder.Write('*');

        builder.Write(' ').Write(parameter.Name);
    }

}