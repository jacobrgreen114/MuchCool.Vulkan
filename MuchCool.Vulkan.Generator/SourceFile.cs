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

namespace MuchCool.Vulkan.Generator; 

public class SourceFile {
    private readonly SourceBuilder _builder = new();

    public SourceFile() : this(null, null){}
    
    public SourceFile(string? ns, string[]? usings) {
        if (usings is not null) {
            foreach (var use in usings) 
                WriteUsing(use);
            WriteBlankLine();
        }

        if (ns is not null) {
            WriteNamespace(ns);
            WriteBlankLine();
        }
    }

    public override string ToString() {
        return _builder.ToString();
    }

    public void WriteBlankLine() {
        _builder.WriteLine();
    }

    public void WriteUsing(string use) {
        _builder.WriteIndentation().Write("using ").Write(use).Terminate();
    }

    public void WriteNamespace(string ns) {
        _builder.WriteIndentation().Write("namespace ").Write(ns).Terminate();
    }

    public void WriteAttribute(string attr) {
        _builder.WriteIndentation().Write('[').Write(attr).WriteLine(']');
    }

    public void WriteEnumStart(string name, string? type = null, AccessModifier access = AccessModifier.Unspecified) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.Write("enum ").Write(name);
        if (type is not null)
            _builder.Write(" : ").Write(type);
        WriteScopeStart();
    }

    public void WriteEnumValue(string name, string? value = null) {
        _builder.WriteIndentation().Write(name);
        if (value is not null)
            _builder.Write(" = ").Write(value);
        _builder.WriteLine(',');
    }

    public void WriteEnumEnd() {
        WriteScopeEnd();
    }
    
    public void WriteStructStart(string name, AccessModifier access = AccessModifier.Unspecified,  bool isUnsafe = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isUnsafe, "unsafe ").Write("struct ").Write(name);
        WriteScopeStart();
    }

    public void WriteStructField(string name, string type, string? defaultValue = null, AccessModifier access = AccessModifier.Unspecified, bool isReadonly = false, bool isStatic = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isStatic, "static ").WriteIf(isReadonly, "readonly ");
            
        _builder.Write(type).Write(' ').Write(name);
        if (defaultValue is not null)
            _builder.Write(" = ").Write(defaultValue);
        _builder.Terminate();
    }

    public void WriteStructFieldArray(
        string name,               string type, int arraySize, AccessModifier access = AccessModifier.Unspecified,
        bool   isReadonly = false, bool   isStatic = false
    ) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isStatic, "static ").WriteIf(isReadonly, "readonly ").Write("fixed ");
        _builder.Write(type).Write(' ').Write(name);
        _builder.Write('[').Write(arraySize.ToString()).Write(']');
        _builder.Terminate();
    }
    
    public void WriteStructEnd() {
        WriteScopeEnd();
    }

    public void WriteClassStart(string name, AccessModifier access = AccessModifier.Unspecified, bool isPartial = false, bool isUnsafe = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isUnsafe, "unsafe ").WriteIf(isPartial, "partial ").Write("class ").Write(name);
        WriteScopeStart();
    }
    
    public void WriteClassEnd() {
        WriteScopeEnd();
    }
    
    public void WriteConstructorStart(
        string typename, AccessModifier access = AccessModifier.Unspecified, bool isStatic = false
    ) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.Write(typename).Write('(').Write(')');
        WriteScopeStart();
    }
    
    public void WriteConstructorEnd() {
        WriteScopeEnd();
    }
    
    
    public void WriteScopeStart() {
        _builder.WriteLine(" {").Indent();
    }

    public void WriteScopeEnd() {
        _builder.UnIndent().WriteIndentation().WriteLine("}");
    }

    public void WriteComment(string comment) {
        _builder.Write("// ").WriteLine(comment);
    }
    
    private void WriteAccessModifier(AccessModifier access) {
        switch (access) {
            case AccessModifier.Private:
                _builder.Write("private ");
                break;
            case AccessModifier.Protected:
                _builder.Write("protected ");
                break;
            case AccessModifier.Internal:
                _builder.Write("internal ");
                break;
            case AccessModifier.Public:
                _builder.Write("public ");
                break;
        }
    }
}

public enum AccessModifier {
    Unspecified,
    Private,
    Protected,
    Internal,
    Public,
}

file static class SourceHelpers {

    public static SourceBuilder Terminate(this SourceBuilder builder) {
        return builder.WriteLine(';');
    }
    
}