using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VulkGen; 

public class SourceGenerator {
    private static readonly string[] _enabledFeatures = new[] {
        "VK_VERSION_1_0"
    };


    private static readonly string[] _enabledAuthors = new[] {
        "KHR", "EXT"
    };
    
    private static readonly string[] _enabledPlatforms = new[] {
        "win32"
    };
    
    
    
    public static void Generate(VulkanRegistry registry) {
        var enabledFeatures = 
            registry.Features.Values.Where(f => _enabledFeatures.Contains(f.Name)).ToArray();
        
        var enabledExtensions =
            registry.Extensions.Values
                .Where(e => _enabledAuthors.Contains(e.Author))
                .Where(e => e.Platform is null || _enabledPlatforms.Contains(e.Platform)).ToArray();

        var featuresTypes  = enabledFeatures.Select(f => f.RequiredTypes);
        var extensionTypes = enabledExtensions.Select(e => e.RequiredTypes);
        var enabledTypes =
            featuresTypes.Union(extensionTypes)
                .SelectMany(t => t)
                .Select(t => t.Name).ToArray();

        var featuresCommands  = enabledFeatures.Select(f => f.RequiredCommands);
        var extensionCommands = enabledExtensions.Select(e => e.RequiredCommands);
        var enabledCommands   = 
            featuresCommands.Union(extensionCommands)
                .SelectMany(c => c)
                .Select(c => c.Name).ToArray();
        
        
        TypeGenerator.Generate(registry);
        HandlesGenerator.Generate(registry, enabledTypes);
        StructGenerator.Generate(registry, enabledTypes);
        EnumGenerator.Generate(registry);
        CommandGenerator.Generate(registry, enabledCommands);
    }
}


public static class TypeGenerator {
    private static readonly Dictionary<string, string> _nativeTypes = new() {
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

        foreach (var type in _nativeTypes)
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
    private static readonly string[] Usings = new[] {
        "System.Runtime.InteropServices"
    };
    
    internal static void Generate(VulkanRegistry registry, IReadOnlyList<string> enabledTypes) {
        var handles = registry.Types.Handles;

        var builder = new SourceBuilder();

        foreach (var ns in Usings)
            builder.Write("using ").Write(ns).WriteLine(';');

        builder.WriteLine();

        foreach (var handle in handles.Where(h => enabledTypes.Contains(h.Name))) {
            GenerateHandle(builder, handle);
        }

        using var sourceFile = new StreamWriter("VulkanHandles.g.cs");
        sourceFile.Write(builder.ToString());
    }

    private static void GenerateHandle(SourceBuilder builder, VulkanHandle handle) {
        builder.WriteIndentation().WriteLine("[StructLayout(LayoutKind.Sequential)]")
            .Write("public unsafe struct ").Write(handle.Name).WriteLine(" {").Indent()
            .WriteIndentation().WriteLine("private void* _handle;").WriteLine()
            .WriteIndentation().Write("public static readonly ").Write(handle.Name).WriteLine(" Null = new();")
            .UnIndent().WriteLine('}').WriteLine();
    }
}

public static class StructGenerator {
    private static readonly string[] Usings = new[] {
        "System.Runtime.InteropServices"
    };
    
    public static void Generate(VulkanRegistry registry, IReadOnlyList<string> enabledTypes) {
        var structs = registry.Types.Structs;

        var builder = new SourceBuilder();
        foreach (var ns in Usings)
            builder.Write("using ").Write(ns).WriteLine(';');

        builder.WriteLine();
        
        foreach (var s in structs.Where(s => enabledTypes.Contains(s.Name)))
            WriteStruct(builder, s);

        using var sourceFile = new StreamWriter("VulkanStructs.g.cs");
        sourceFile.Write(builder.ToString());
    }


    private static SourceBuilder WriteStruct(SourceBuilder builder, VulkanStruct s) {
        builder
            .WriteLine("[StructLayout(LayoutKind.Sequential)]")
            .Write($"public unsafe struct ")
            .Write(s.Name)
            .WriteLine(" {")
            .Indent();

        foreach (var field in s.Fields) {
            WriteField(builder, field);
        }

        return builder.UnIndent().WriteLine("}").WriteLine();
    }

    private static SourceBuilder WriteField(SourceBuilder builder, VulkanField field) {
        builder.WriteIndentation().Write("public ").Write(field.TypeName.Replace("FlagBits", "Flags"));
        
        for (int i = 0; i < field.PointerDepth; ++i) 
            builder.Write('*');

        return builder.Write(' ').Write(field.Name).WriteLine(";");
    }
    
}

public static class EnumGenerator {
    private static readonly string[] Usings = new[] {
        "System"
    };
    
    public static void Generate(VulkanRegistry registry) {
        var enums    = registry.Types.Enums;
        var bitmasks = registry.Types.Bitmasks;

        var builder = new SourceBuilder();
        foreach (var ns in Usings)
            builder.Write("using ").Write(ns).WriteLine(';');

        foreach (var bitmask in bitmasks) {
            if (enums.ContainsKey(bitmask.Name)) continue;
            WriteBitmask(builder, bitmask);
        }

        builder.WriteLine("// Enums");
        
        foreach (var e in enums) {
            WriteEnum(builder, e.Value);
        }

        using var sourceFile = new StreamWriter("VulkanEnums.g.cs");
        sourceFile.Write(builder.ToString());
    }

    public static SourceBuilder WriteBitmask(SourceBuilder builder, VulkanBitmask bitmask) {
        //if (bitmask.Requires is not null) {
        //    Debugger.Break();
        //    return builder;
        //}
        
        builder.WriteIndentation().WriteLine("[Flags]")
            .WriteIndentation().Write("public enum ").Write(bitmask.Name);

        if (bitmask.TypeName is not null)
            builder.Write(" : ").Write(bitmask.TypeName);

        builder.WriteLine(" {").Indent();
        
        return builder.UnIndent()
            .WriteIndentation().WriteLine("}").WriteLine();
    }
    

    private static SourceBuilder WriteEnum(SourceBuilder builder, VulkanEnum e) {
        builder.WriteIndentation().WriteLineIf(e.IsBitmask, "[Flags]")
            .WriteIndentation().Write("public enum ").Write(e.Name);

        if (e.TypeName is not null)
            builder.Write(" : ").Write(e.TypeName);

        builder.WriteLine(" {").Indent();

        if (e.Enumeration is not null)
            foreach (var value in e.Enumeration.Value)
                WriteEnumerationValue(builder, value);

        builder.UnIndent()
            .WriteIndentation().WriteLine("}").WriteLine();

        return builder;
    }

    private static SourceBuilder WriteEnumerationValue(SourceBuilder builder, VulkanEnumerationValue value) {
        builder.WriteIndentation().Write(value.Name);
        if (value.Value is not null) {
            builder.Write(" = ").Write(value.Value);
        }

        builder.WriteLine(',');
        return builder;
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