using System.Xml.Serialization;

namespace MuchCool.Vulkan.Generator.Registry.Xml; 


[Serializable]
[XmlRoot("registry")]
public class XmlVulkanRegistry
{
    [XmlArray("tags")]
    [XmlArrayItem("tag")]
    public XmlVulkanTag[]? Tags;

    [XmlArray("platforms")]
    [XmlArrayItem("platform")]
    public XmlVulkanPlatform[]? Platforms;

    [XmlArray("types")]
    [XmlArrayItem("type")]
    public XmlVulkanType[]? Types;

    [XmlElement("enums")]
    public XmlVulkanEnum[]? Enums;

    [XmlArray("commands")]
    [XmlArrayItem("command")]
    public XmlVulkanCommand[]? Commands;

    [XmlElement("feature")] 
    public XmlVulkanFeature[]? Features;
    
    [XmlArray("extensions")]
    [XmlArrayItem("extension")]
    public XmlVulkanExtension[]? Extensions;
    

    private static readonly XmlSerializer _serializer = new(typeof(XmlVulkanRegistry));
    public static XmlVulkanRegistry FromFile(string path)
    {
        using var xml = File.OpenText(path);
        var registry = _serializer.Deserialize(xml) as XmlVulkanRegistry;
        if (registry is null)
            throw new Exception("Failed to deserialize registry.");

        return registry;
    }
}

[Serializable]
[XmlRoot("platform")]
public struct XmlVulkanPlatform
{
    [XmlAttribute("name")]
    public string? Name;

    [XmlAttribute("protect")]
    public string? Protect;

    [XmlAttribute("comment")]
    public string? Comment;
}


[Serializable]
[XmlRoot("tag")]
public struct XmlVulkanTag
{
    [XmlAttribute("name")]
    public string? Name;
}

[Serializable]
[XmlRoot("type")]
public struct XmlVulkanType
{
    [XmlAttribute("category")]
    public string? Category;

    [XmlAttribute("name")]
    public string? NameAttribute;

    [XmlElement("name")]
    public string? NameElement;
    
    [XmlElement("type")]
    public string? Type;

    [XmlAttribute("requires")]
    public string? Requires;
    
    [XmlAttribute("alias")]
    public string? Alias;

    [XmlElement("member")]
    public XmlVulkanTypeMember[]? Members;

    [XmlText] public string? Text;
    
    public string? Name => NameAttribute ?? NameElement;
}

[Serializable]
[XmlRoot("member")]
public struct XmlVulkanTypeMember
{
    [XmlAttribute("optional")]
    public string? Optional;

    [XmlAttribute("len")]
    public string? Length;

    [XmlElement("name")]
    public string? Name;

    [XmlElement("type")]
    public string? Type;

    [XmlText]
    public string[]? TypeModifiers;

    [XmlElement("enum")] 
    public string? ArraySize;
    
    [XmlAttribute("values")]
    public string? Values;
}

[Serializable]
public struct XmlVulkanEnum
{
    [XmlAttribute("name")]
    public string? Name;

    [XmlAttribute("type")]
    public string? Type;
    
    [XmlAttribute("bitwidth")]
    public string? BitWidth;

    [XmlElement("enum")]
    public XmlVulkanEnumValue[]? Values;
}

[Serializable]
public struct XmlVulkanEnumValue
{
    [XmlAttribute("type")]
    public string? Type;

    [XmlAttribute("name")]
    public string? Name;
    
    [XmlAttribute("value")]
    public string? Value;

    [XmlAttribute("alias")]
    public string? Alias;
    
    [XmlAttribute("bitpos")]
    public string? BitPos;
}

[Serializable]
[XmlRoot("command")]
public struct XmlVulkanCommand
{
    [XmlAttribute("successcodes")]
    public string? SuccessCodes;

    [XmlAttribute("errorcodes")]
    public string? ErrorCodes;

    [XmlAttribute("name")] 
    public string? _name; 
    
    [XmlAttribute("alias")] 
    public string? Alias;

    public string? Name => Prototype.Name ?? _name;
    
    [XmlElement("proto")]
    public XmlVulkanCommandPrototype Prototype;

    [XmlElement("param")]
    public XmlVulkanCommandParameter[]? Parameters;
}

[Serializable]
[XmlRoot("proto")]
public struct XmlVulkanCommandPrototype
{
    [XmlElement("type")]
    public string? ReturnType;

    [XmlElement("name")]
    public string? Name;
}

[Serializable]
[XmlRoot("param")]
public struct XmlVulkanCommandParameter
{
    [XmlAttribute("optional")]
    public string? Optional;

    [XmlElement("type")]
    public string? Type;

    [XmlElement("name")]
    public string? Name;

    [XmlText]
    public string? TypeModifiers;

    [XmlAttribute("len")]
    public string? LengthParameter;
}


[Serializable]
public struct XmlVulkanFeature {
    [XmlAttribute("api")] 
    public string? Api;
    
    [XmlAttribute("name")] 
    public string? Name;
    
    [XmlElement("require")] 
    public XmlVulkanRequire[]? Requires;
}

[Serializable]
public struct XmlVulkanExtension {
    [XmlAttribute("name")]
    public string? Name;
    
    [XmlAttribute("number")]
    public string? Number;
    
    [XmlAttribute("type")]
    public string? Type;
    
    [XmlAttribute("author")]
    public string? Author;
    
    [XmlAttribute("supported")]
    public string? Supported;
    
    [XmlAttribute("platform")]
    public string? Platform;
    
    [XmlElement("require")] 
    public XmlVulkanRequire[]? Requires;
}

[Serializable]
public struct XmlVulkanRequire {
    [XmlAttribute("feature")] 
    public string? Feature;
    
    [XmlElement("type")] 
    public XmlVulkanRequireType[]? Types;
    
    [XmlElement("command")] 
    public XmlVulkanRequireCommand[]? Commands;
}

[Serializable]
public struct XmlVulkanRequireType {
    [XmlAttribute("name")]
    public string? Name;
}

[Serializable]
public struct XmlVulkanRequireCommand {
    [XmlAttribute("name")]
    public string? Name;
}