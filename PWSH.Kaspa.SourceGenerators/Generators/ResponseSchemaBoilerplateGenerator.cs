namespace PWSH.Kaspa.SourceGenerators.Generators;

[Generator]
public sealed partial class ResponseSchemaBoilerplateGenerator 
    :
    AbstractBoilerplateGenerator
{
/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

    public ResponseSchemaBoilerplateGenerator() 
        :
        base("GenerateResponseSchemaBoilerplateAttribute") 
    { }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

    protected override void EmitNestedClassBoilerplate(StringBuilder builder, string[] nested_names, List<(string Name, string Type)> properties)
    {
        int indent = 0;

        // Begin nested class declarations
        foreach (var name in nested_names)
        {
            builder.AppendIndent(indent)
                   .Append($"partial class {name}").AppendLine();


            if (indent < nested_names.Length - 1)
            {
                builder
                       .AppendIndent(indent)
                       .Append("{").AppendLine();
            }

            indent++;
        }

        // Add actual generation here
        builder
            .AppendIndent(indent).Append(':').AppendLine()
            .AppendIndent(indent).Append($"IEquatable<{nested_names.Last()}>,").AppendLine()
            .AppendIndent(indent).Append("IJSONableDisplayable").AppendLine()
            .AppendIndent(indent - 1).Append('{');

        builder
            .AppendLine()
            .AppendEQUALITY()
            .AppendLine()
            .AppendEquals(nested_names.Last(), properties, indent)
            .AppendLine()
            .AppendGetHashCode(properties, indent)
            .AppendLine()
            .AppendEquals(nested_names.Last(), indent)
            .AppendLine()
            .AppendOperator(nested_names.Last(), indent);
            

        builder
            .AppendLine()
            .AppendHELPERS()
            .AppendLine()
            .AppendToJSON(indent);

        // Close all opened braces
        for (int i = nested_names.Length - 1; i >= 0; i--)
        {
            indent--;
            builder.AppendIndent(indent).Append('}').AppendLine();
        }
    }
}
