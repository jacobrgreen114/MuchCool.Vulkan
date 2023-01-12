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

using Microsoft.CodeAnalysis.Operations;

namespace MuchCool.Vulkan.Generator; 

public sealed class SourceFile {
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

    public SourceFile WriteBlankLine() {
        _builder.WriteLine();
        return this;
    }

    public SourceFile WriteUsing(string use) {
        _builder.WriteIndentation().Write("using ").Write(use).Terminate();
        return this;
    }

    public SourceFile WriteNamespace(string ns) {
        _builder.WriteIndentation().Write("namespace ").Write(ns).Terminate();
        return this;
    }

    public SourceFile WriteAttribute(string attr) {
        _builder.WriteIndentation().Write('[').Write(attr).WriteLine(']');
        return this;
    }

    public SourceFile WriteEnumStart(string name, string? type = null, AccessModifier access = AccessModifier.Unspecified) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.Write("enum ").Write(name);
        if (type is not null)
            _builder.Write(" : ").Write(type);
        WriteScopeStart();
        return this;
    }

    public SourceFile WriteEnumValue(string name, string? value = null) {
        _builder.WriteIndentation().Write(name);
        if (value is not null)
            _builder.Write(" = ").Write(value);
        _builder.WriteLine(',');
        return this;
    }

    public SourceFile WriteEnumEnd() {
        WriteScopeEnd();
        return this;
    }
    
    public SourceFile WriteStructStart(string name, AccessModifier access = AccessModifier.Unspecified,  bool isUnsafe = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isUnsafe, "unsafe ").Write("struct ").Write(name);
        WriteScopeStart();
        return this;
    }

    public SourceFile WriteStructField(string name, string type, string? defaultValue = null, AccessModifier access = AccessModifier.Unspecified, bool isReadonly = false, bool isStatic = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isStatic, "static ").WriteIf(isReadonly, "readonly ");
            
        _builder.Write(type).Write(' ').Write(name);
        if (defaultValue is not null)
            _builder.Write(" = ").Write(defaultValue);
        _builder.Terminate();
        return this;
    }

    public SourceFile WriteStructFieldArray(
        string name,               string type, int arraySize, AccessModifier access = AccessModifier.Unspecified,
        bool   isReadonly = false, bool   isStatic = false
    ) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isStatic, "static ").WriteIf(isReadonly, "readonly ").Write("fixed ");
        _builder.Write(type).Write(' ').Write(name);
        _builder.Write('[').Write(arraySize.ToString()).Write(']');
        _builder.Terminate();
        return this;
    }
    
    public SourceFile WriteStructEnd() {
        WriteScopeEnd();
        return this;
    }

    public SourceFile WriteClassStart(string name, AccessModifier access = AccessModifier.Unspecified, bool isPartial = false, bool isUnsafe = false) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.WriteIf(isUnsafe, "unsafe ").WriteIf(isPartial, "partial ").Write("class ").Write(name);
        WriteScopeStart();
        return this;
    }
    
    public SourceFile WriteClassEnd() {
        WriteScopeEnd();
        return this;
    }
    
    public SourceFile WriteConstructorStart(
        string typename, AccessModifier access = AccessModifier.Unspecified, bool isStatic = false
    ) {
        _builder.WriteIndentation();
        WriteAccessModifier(access);
        _builder.Write(typename).Write('(').Write(')');
        WriteScopeStart();
        return this;
    }
    
    public SourceFile WriteConstructorEnd() {
        WriteScopeEnd();
        return this;
    }
    
    
    private void WriteScopeStart() {
        _builder.WriteLine(" {").Indent();
    }

    private void WriteScopeEnd() {
        _builder.UnIndent().WriteIndentation().WriteLine("}");
    }

    public SourceFile WriteComment(string comment) {
        _builder.Write("// ").WriteLine(comment);
        return this;
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