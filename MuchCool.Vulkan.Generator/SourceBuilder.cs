using System.Text;

namespace VulkGen; 

public class SourceBuilder {
    private readonly StringBuilder _builder = new();
    public override string ToString() => _builder.ToString();

    public int    IndentLevel { get; private set; }
    public string Indentation { get; private set; } = "  ";


    public SourceBuilder Write(string str) {
        _builder.Append(str);
        return this;
    }

    public SourceBuilder Write(char c) {
        _builder.Append(c);
        return this;
    }
    
    public SourceBuilder WriteLine() {
        return Write(Environment.NewLine);;
    }    
    
    public SourceBuilder WriteLine(char c) {
        return Write(c).WriteLine();
    }

    public SourceBuilder WriteLine(string str) {
        _builder.Append(str);
        return WriteLine();
    }
    
    public SourceBuilder WriteIf(bool condition, string str) {
        return condition ? Write(str) : this;
    }
    
    public SourceBuilder WriteLineIf(bool condition, string str) {
        return condition ? WriteLine(str) : this;
    }

    public SourceBuilder Indent() {
        ++IndentLevel;
        return this;
    }

    public SourceBuilder UnIndent() {
        if (IndentLevel == 0) throw new InvalidOperationException("Indent level already 0.");
        --IndentLevel;
        return this;
    }

    public SourceBuilder WriteIndentation() {
        for (int i = 0; i < IndentLevel; ++i)
            _builder.Append(Indentation);
        return this;
    }
}