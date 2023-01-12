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


using System.Diagnostics;
using MuchCool.Vulkan.Generator.Registry.Xml;

namespace MuchCool.Vulkan.Generator.Registry; 

public sealed class VulkanRegistry {
    
    private readonly Dictionary<string, VulkanPlatform>    _platforms;
    private readonly Dictionary<string, VulkanEnumeration> _enumerations;

    private readonly VulkanTypes                           _types;
    private readonly Dictionary<string, VulkanCommand>     _commands;

    private readonly Dictionary<string, VulkanFeature> _features;
    private readonly Dictionary<string, VulkanExtension> _extensions;


    public IEnumerable<VulkanPlatform>    Platforms    => _platforms.Values;
    public IEnumerable<VulkanEnumeration> Enumerations => _enumerations.Values;
    
    public VulkanTypes            Types    => _types;
    public IReadOnlyDictionary<string, VulkanCommand> Commands => _commands;


    public IReadOnlyDictionary<string, VulkanFeature>   Features => _features;
    public IReadOnlyDictionary<string, VulkanExtension> Extensions => _extensions;

    public VulkanRegistry(XmlVulkanRegistry registry) {
        _platforms = registry.Platforms is null 
            ? new() 
            : registry.Platforms.Select(p => new VulkanPlatform(p)).ToDictionary(p => p.Name);

        _enumerations = registry.Enums is null
            ? new()
            : registry.Enums.Select(e => new VulkanEnumeration(e)).ToDictionary(e => e.Name);

        _types = new VulkanTypes(registry.Types, _enumerations);
        
        _commands = registry.Commands is null
            ? new()
            : registry.Commands.Where(c => c.Alias is null).Select(c => new VulkanCommand(c)).ToDictionary(c => c.Name);

        _features = registry.Features is null
            ? new()
            : registry.Features.Select(f => new VulkanFeature(f)).ToDictionary(f => f.Name);

        _extensions = registry.Extensions is null
            ? new()
            : registry.Extensions.Where(e => e.Supported != "disabled")
                .Select(e => new VulkanExtension(e)).ToDictionary(e => e.Name);
    }
}

public sealed class VulkanPlatform  {
    public string  Name    { get; }
    public string? Comment { get; }
    
    public VulkanPlatform(in XmlVulkanPlatform platform) {
        if (platform.Name is null) throw new Exception();
        Name         = platform.Name;
        Comment = platform.Comment;
    }
}

public sealed class VulkanTypes {
    
    private readonly Dictionary<string,VulkanBaseType> _baseTypes = new();
    private readonly Dictionary<string,VulkanBitmask>  _bitmasks  = new();
    private readonly Dictionary<string,VulkanHandle>   _handles   = new();
    private readonly Dictionary<string,VulkanEnum>     _enums     = new();
    private readonly Dictionary<string,VulkanStruct>   _structs   = new();

    
    public IEnumerable<VulkanBaseType> BaseTypes => _baseTypes.Values;
    public IEnumerable<VulkanBitmask>  Bitmasks  => _bitmasks.Values;
    public IEnumerable<VulkanHandle>   Handles   => _handles.Values;
    public IReadOnlyDictionary<string, VulkanEnum>              Enums     => _enums;
    public IEnumerable<VulkanStruct>   Structs   => _structs.Values;

    
    public VulkanTypes(IEnumerable<XmlVulkanType>? types, Dictionary<string, VulkanEnumeration> enumerations) {
        if (types is null) return;

        foreach (var type in types) {
            if (type.Category is null) continue;
            switch (type.Category) {
                case Constants.Bitmask: {
                    HandleBitmask(type, enumerations);
                    break;
                }
                case Constants.Handle: {
                    var handle = new VulkanHandle(type);
                    _handles.Add(handle.Name, handle);
                    break;
                }
                case Constants.Enum: {
                    HandleEnum(type, enumerations);
                    break;
                }
                case Constants.Struct: {
                    var s = new VulkanStruct(type);
                    _structs.Add(s.Name, s);
                    break;
                }
                case Constants.BaseType: {
                    if (type.Type is not null) {
                        var t = new VulkanBaseType(type);
                        _baseTypes.Add(t.Name, t);
                    }
                    break;
                }
            }
        }
    }

    private void HandleBitmask(in XmlVulkanType type, Dictionary<string, VulkanEnumeration> enumerations) {
        Debug.Assert(type.Category == Constants.Bitmask);
        var bitmask = new VulkanBitmask(type, enumerations);
        _bitmasks.Add(bitmask.Name, bitmask);
    }
    
    
    private void HandleEnum(in XmlVulkanType type, Dictionary<string, VulkanEnumeration> enumerations) {
        Debug.Assert(type.Category == Constants.Enum);

        var name = type.Name ?? throw new Exception();

        VulkanEnumeration? enumeration;
        
        var alias = type.Alias;
        if (alias is null)
            enumerations.TryGetValue(name, out enumeration);
        else
            enumerations.TryGetValue(alias, out enumeration);

        var e = new VulkanEnum(type, enumerations, _bitmasks);
        _enums.Add(e.Name, e);
    }
}

public enum VulkanTypeCategory {
    BaseType,
    Handle,
    Bitmask,
    Enum,
    Struct
}

public abstract class VulkanType {
    public VulkanTypeCategory Category { get; }
    public string             Name     { get; protected set; }
    
    internal VulkanType(VulkanTypeCategory category, string? name) {
        Category = category;
        Name     = name ?? throw new Exception();
    }
}

public sealed class VulkanBaseType : VulkanType {
    public string Type { get; }
    
    public VulkanBaseType(in XmlVulkanType type) 
        : base(VulkanTypeCategory.BaseType, type.Name) {
        Debug.Assert(type.Category == Constants.BaseType);
        Type = type.Type ?? throw new Exception();
    }
}

public sealed class VulkanBitmask : VulkanType {
    public string? TypeName { get; }
    public string? Requires { get; }

    //public VulkanEnumeration? Enumeration { get; }
    
    public VulkanBitmask(in XmlVulkanType type, Dictionary<string, VulkanEnumeration> enumerations) 
        : base(VulkanTypeCategory.Bitmask, type.Name) {
        Debug.Assert(type.Category == Constants.Bitmask);

        TypeName = type.Type is null ? null : Helpers.FormatTypeName(type.Type);
        Requires = type.Requires;
        
        //if (Requires is not null)
        //    Enumeration = enumerations.TryGetValue(Requires, out var enumeration) ? enumeration : null;
    }
}

public sealed class VulkanHandle : VulkanType {
    public VulkanHandle(in XmlVulkanType type)
        : base(VulkanTypeCategory.Handle, type.Name) {
        Debug.Assert(type.Category == Constants.Handle);
    }
}

public sealed class VulkanEnum : VulkanType {
    public string? TypeName { get; }
    
    public bool _isBitmask = false;
    
    public VulkanEnumeration? Enumeration { get; }
    
    public VulkanEnum(in XmlVulkanType type, Dictionary<string, VulkanEnumeration> enumerations, Dictionary<string, VulkanBitmask> bitmasks)
        : base(VulkanTypeCategory.Enum, type.Name) {
        Debug.Assert(type.Category == Constants.Enum);

        
        TypeName = type.Type is null ? null : Helpers.FormatTypeName(type.Type);

        var aliasName = type.Alias;

        VulkanBitmask[] aliases;
        if (aliasName is null) {
            Enumeration = enumerations.TryGetValue(Name, out var enumeration) ? enumeration : null;
            aliases     = bitmasks.Values.Where(b => b.Requires == Name).ToArray();
        }
        else {
            Enumeration = enumerations.TryGetValue(aliasName, out var enumeration) ? enumeration : null;
            aliases     = bitmasks.Values.Where(b => b.Requires == aliasName).ToArray();
        }
        
        if (aliases.Length > 0) {
            var alias = aliases[0];
            TypeName   = alias.TypeName is null ? null : Helpers.FormatTypeName(alias.TypeName);
            _isBitmask = true;
        }
        
        if (Enumeration is not null && TypeName is null) {
            if (Enumeration.IsBitMask && Enumeration.BitWidth is not null) {
                TypeName = Enumeration.BitWidth switch {
                    32 => "uint",
                    64 => "ulong",
                    _ => throw new Exception()
                };
            }
        }

        Name = Name.Replace("FlagBits", "Flags");
    }

    public bool IsBitmask => _isBitmask;
}

public sealed class VulkanStruct : VulkanType {
    private readonly List<VulkanField> _fields = new();

    public IEnumerable<VulkanField> Fields => _fields;

    public VulkanStruct(in XmlVulkanType type)
        : base(VulkanTypeCategory.Struct, type.Name) {
        Debug.Assert(type.Category == Constants.Struct);
        if (type.Members is not null) {
            _fields.AddRange( type.Members.Select(m => new VulkanField(m)));
        }
    }
}

public sealed class VulkanField {
    public string  Name         { get; }
    public string  TypeName     { get; }
    public string? Value        { get; }
    public int     PointerDepth { get; } = 0;
    public bool    IsArray      { get; } = false;
    public int     ArraySize    { get; } = 0;
    
    public VulkanField(in XmlVulkanTypeMember member) {
        if (member.Name is null || member.Type is null) throw new Exception();
        Name     = Helpers.FormatFieldName(member.Name);
        TypeName = Helpers.FormatTypeName((member.Type));
        Value    = member.Values;
        
        if (member.TypeModifiers is not null) {
            PointerDepth = Helpers.FindPointerDepth(member.TypeModifiers);
            IsArray      = Helpers.IsInlineArray(member.TypeModifiers);

            if (member.ArraySize is not null)
                ArraySize = !IsArray ? 0 : Helpers.GetArraySize(member.ArraySize);
            else {
                if (IsArray) {
                    foreach (var modifier in member.TypeModifiers) {
                        if (modifier.StartsWith("["))
                            ArraySize = ParseArray(modifier);
                    }
                }
            }
        }
    }

    // todo : make array parsing less hacky
    private static int ParseArray(string array) {
        if (array.Count(c => c is ']') <= 1) // not multi-dimensional
            return int.Parse(array.Substring(1, array.Length - 2));
        else { // multi-dimensional
            var dimensions = array.Split(']');
            var count      = int.Parse(dimensions[0].Substring(1));

            for (int i = 1; i < dimensions.Length - 1; ++i) {
                count *= int.Parse(dimensions[i].Substring(1));
            }
            
            return count;
        }
    }
}

public sealed class VulkanEnumeration {
    public           string                   Name { get; }
    private readonly VulkanEnumerationValue[] _values;
    public           bool                     IsBitMask  { get; }
    public           int?                      BitWidth { get; }
    
    public IEnumerable<VulkanEnumerationValue> Value => _values;

    public VulkanEnumeration(in XmlVulkanEnum e) {
        if (e.Name is null) throw new Exception();
        Name = e.Name;

        _values = e.Values is null
            ? Array.Empty<VulkanEnumerationValue>()
            : e.Values.Select(v => new VulkanEnumerationValue(v)).ToArray();

        IsBitMask = e.Type is not null && e.Type == "bitmask";

        BitWidth = e.BitWidth is null ? null : int.Parse(e.BitWidth);
    }
    
}

public sealed class VulkanEnumerationValue {
    public string  Name  { get; }
    public string? Value { get; }
    
    public VulkanEnumerationValue(in XmlVulkanEnumValue value) {
        if (value.Name is null) throw new Exception();
        Name = value.Name;

        if (value.Value is not null)
            Value = value.Value;
        else if (value.BitPos is not null)
            Value = $"((uint)1) << {value.BitPos}";
        else if (value.Alias is not null)
            Value = value.Alias;
        else
            Value = null;
    }
    
}


public class VulkanCommand {
    public           string                   Name       { get; }
    public           string?                  Alias      { get; }
    public           string                  ReturnType { get; }
    private readonly VulkanCommandParameter[] _params;

    public IReadOnlyList<VulkanCommandParameter> Parameters => _params;

    public VulkanCommand(in XmlVulkanCommand command) {
        Name       = command.Name ?? throw new Exception();
        Alias      = command.Alias;
        ReturnType = Helpers.FormatTypeName(command.Prototype.ReturnType ?? throw new Exception());

        if (Alias is null && ReturnType is null) throw new Exception();

        _params = command.Parameters is null
            ? Array.Empty<VulkanCommandParameter>()
            : command.Parameters.Select(p => new VulkanCommandParameter(p)).ToArray();
    }

    public bool IsAlias => Alias is not null;
}


public class VulkanCommandParameter {
    public string Name         { get; }
    public string TypeName     { get; }
    public int    PointerDepth { get; }
    public bool   IsConst      { get; }
    public bool   Optional     { get; }
    public bool   SemiOptional { get; }
    public bool   IsArray      { get; }
    
    public VulkanCommandParameter(in XmlVulkanCommandParameter param) {
        Name         = Helpers.FormatFieldName(param.Name ?? throw new Exception());
        TypeName     = Helpers.FormatTypeName(param.Type  ?? throw new Exception());
        PointerDepth = Helpers.FindPointerDepth(param.TypeModifiers) ;
        IsConst      = Helpers.IsConst(param.TypeModifiers);
        Optional     = param.Optional?.Contains("true") ?? false;
        SemiOptional = Optional && (param.Optional?.Contains("false") ?? false);
        IsArray      = param.LengthParameter is not null;
    }

    public bool IsInParameter => TypeName is not "void" 
                                 && TypeName is not "sbyte"
                                 && !Optional 
                                 && !IsArray
                                 && IsConst 
                                 && PointerDepth > 0;

    public bool IsRefParameter => TypeName is not "void" 
                                  && TypeName is not "sbyte" 
                                  && SemiOptional 
                                  && PointerDepth > 0;

    public bool IsOutParameter => TypeName is not "void" 
                                  && TypeName is not "sbyte" 
                                  && !IsConst 
                                  && !Optional 
                                  && !IsArray
                                  && PointerDepth > 0;
}



public class VulkanFeature {
    public string  Name { get; }
    public string? Api  { get; }
    
    private readonly List<VulkanRequiredType>    _requiredTypes = new();
    private readonly List<VulkanRequiredCommand> _requiredCommands = new();

    public IEnumerable<VulkanRequiredType>    RequiredTypes    => _requiredTypes;
    public IEnumerable<VulkanRequiredCommand> RequiredCommands => _requiredCommands;

    public VulkanFeature(in XmlVulkanFeature feature) {
        Name = feature.Name ?? throw new Exception();
        Api  = feature.Api;
        
        if(feature.Requires is not null){
            foreach (var require in feature.Requires) {
                if (require.Types is not null) {
                    foreach (var type in require.Types) {
                        _requiredTypes.Add(new VulkanRequiredType(type));
                    }
                }
                if (require.Commands is not null) {
                    foreach (var command in require.Commands) {
                        _requiredCommands.Add(new VulkanRequiredCommand(command));
                    }
                }
            }
        }
    }
}

public class VulkanRequire {
    public string Name { get; }

    public VulkanRequire(string? name) {
        Name = name ?? throw new Exception();
    }
    
    public override int GetHashCode() {
        return Name.GetHashCode();
    }
}


public class VulkanExtension {
    public string  Name     { get; }
    public string  Author   { get; }
    public string? Type     { get; }
    public string? Platform { get; }
    
    private readonly List<VulkanRequiredType>    _requiredTypes    = new();
    private readonly List<VulkanRequiredCommand> _requiredCommands = new();

    public IEnumerable<VulkanRequiredType>    RequiredTypes    => _requiredTypes;
    public IEnumerable<VulkanRequiredCommand> RequiredCommands => _requiredCommands;
    
    public VulkanExtension(in XmlVulkanExtension feature) {
        Name     = feature.Name   ?? throw new Exception();
        Author   = feature.Author ?? throw new Exception();
        Type     = feature.Type;
        Platform = feature.Platform;
        
        if(feature.Requires is not null){
            foreach (var require in feature.Requires) {
                if (require.Types is not null) {
                    foreach (var type in require.Types) {
                        _requiredTypes.Add(new VulkanRequiredType(type));
                    }
                }
                if (require.Commands is not null) {
                    foreach (var command in require.Commands) {
                        _requiredCommands.Add(new VulkanRequiredCommand(command));
                    }
                }
            }
        }
    }

    public override string ToString() {
        return $"Extension : {Name}";
    }
}


public class VulkanRequiredType : VulkanRequire {
    public VulkanRequiredType(in XmlVulkanRequireType type) : base(type.Name){

    }


}

public class VulkanRequiredCommand : VulkanRequire {
    public VulkanRequiredCommand(in XmlVulkanRequireCommand command) : base(command.Name){

    }
}

public static class Constants {
    public const string BaseType    = "basetype";
    public const string Handle      = "handle";
    public const string Bitmask     = "bitmask";
    public const string Enum        = "enum";
    public const string Struct      = "struct";
}

public static class Helpers {
    public static int FindPointerDepth(IEnumerable<char>? str) {
        return str is null ? 0 : str.Count(c => c is '*');
    }
    
    public static int FindPointerDepth(string[]? str) {
        return str is null ? 0 : FindPointerDepth(str.SelectMany(c => c));
    }

    public static bool IsInlineArray(string[]? typeModifiers) {
        return typeModifiers is not null && typeModifiers.SelectMany(t => t).Contains('[');
    }
    
    public static bool IsConst(string[]? str) {
        return str is null ? false : str.Any(s => s.Contains("const"));
    }

    // todo : implement generation of typedefs
    public static string FormatTypeName(string typename) {
        return typename switch {
            "char"                => "sbyte",
            "uint8_t"            => "byte",
            "int8_t"            => "sbyte",   
            "uint16_t"            => "ushort",
            "int16_t"            => "short",      
            "uint32_t"            => "uint",
            "int32_t"            => "int",
            "uint64_t"            => "ulong",
            "int64_t"            => "long",
            "size_t"            => "ulong",
            "VkBool32" => "Bool32",
            "VkFlags" => "uint",
            "VkFlags64" => "ulong",
            "VkDeviceSize" => "ulong",
            "VkDeviceAddress" => "ulong",
            "VkSampleMask" => "uint",
            "VkQueueGlobalPriorityKHR" => "uint", // todo : TEMP PATCH
            "VkFragmentShadingRateCombinerOpKHR" => "int", // todo : TEMP PATCH
            "PFN_vkVoidFunction"  => "void*",
            "LPCWSTR"             => "char*",
            "DWORD"             => "uint",
            "HANDLE"             => "void*",
            "HINSTANCE"             => "void*",
            "HWND"             => "void*",
            "HMONITOR"             => "void*",
            "SECURITY_ATTRIBUTES" => "void",
            //var vk when vk.StartsWith("Vk")       => vk[2..],
            //var pfn when pfn.StartsWith("PFN_vk") => pfn.Replace("PFN_vk", "Pfn"),
            var t when true                       => t.Replace("FlagBits", "Flags")
        };
    }

    public static string FormatFieldName(string fieldName) {
        return fieldName switch {
            "object"        => "obj",
            "event" => "evt",
            var n when true => n
        };
    }

    // todo : implement generation of constants
    public static int GetArraySize(string? define) {
        return define switch {
            "VK_MAX_PHYSICAL_DEVICE_NAME_SIZE"         => 256,
            "VK_UUID_SIZE"                             => 16,
            "VK_LUID_SIZE"                             => 8,
            "VK_MAX_EXTENSION_NAME_SIZE"               => 256,
            "VK_MAX_DESCRIPTION_SIZE"                  => 256,
            "VK_MAX_MEMORY_TYPES"                      => 32,
            "VK_MAX_MEMORY_HEAPS"                      => 16,
            "VK_MAX_DEVICE_GROUP_SIZE"                 => 32,
            "VK_MAX_DRIVER_NAME_SIZE"                  => 256,
            "VK_MAX_DRIVER_INFO_SIZE"                  => 256,
            "VK_MAX_GLOBAL_PRIORITY_SIZE_KHR"          => 16,
            "VK_MAX_SHADER_MODULE_IDENTIFIER_SIZE_EXT" => 32,
            null                                       => 0,
            _                                          => throw new Exception()
        };
    }
}
