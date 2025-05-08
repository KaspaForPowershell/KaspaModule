namespace PWSH.Kaspa.SourceGenerators;

public static class ExtensionMethods
{
    public static StringBuilder AppendIndent(this StringBuilder me, int indent_level)
        => me.Append(new string('\t', indent_level));

    public static StringBuilder AppendEQUALITY(this StringBuilder me)
        => me
            .Append("/* -----------------------------------------------------------------").AppendLine()
            .Append("EQUALITY                                                           |").AppendLine()
            .Append("----------------------------------------------------------------- */").AppendLine();

    public static StringBuilder AppendHELPERS(this StringBuilder me)
        => me
            .Append("/* -----------------------------------------------------------------").AppendLine()
            .Append("HELPERS                                                            |").AppendLine()
            .Append("----------------------------------------------------------------- */").AppendLine();

    public static StringBuilder AppendOVERRIDES(this StringBuilder me)
        => me
            .Append("/* -----------------------------------------------------------------").AppendLine()
            .Append("OVERRIDES                                                          |").AppendLine()
            .Append("----------------------------------------------------------------- */").AppendLine();

    public static StringBuilder AppendEquals(this StringBuilder me, string class_name, int indent_level)
        => me
            .AppendIndent(indent_level).Append("public override bool Equals(object? obj)").AppendLine()
            .AppendIndent(indent_level + 1).Append($"=> Equals(obj as {class_name});").AppendLine();

    public static StringBuilder AppendEquals(this StringBuilder me, string class_name, IEnumerable<(string Name, string Type)> property_infos, int indent_level)
    {
        me
        .AppendIndent(indent_level).Append($"public bool Equals({class_name}? other)").AppendLine()
        .AppendIndent(indent_level).Append('{').AppendLine()
        .AppendIndent(indent_level + 1).Append("if (other is null) return false;").AppendLine()
        .AppendIndent(indent_level + 1).Append("if (ReferenceEquals(this, other)) return true;").AppendLine()
        .AppendLine();

        if (property_infos.Count() == 0) 
            return me
                .AppendIndent(indent_level + 1).Append("return true;").AppendLine()
                .AppendIndent(indent_level).Append('}').AppendLine();

        var next = 0;
        var previous = next;

        foreach (var property in property_infos)
        {
            if (property_infos.Count() == 1)
            {
                if (property.Type.Contains("List<")) me.AppendIndent(indent_level + 1).Append($"return {property.Name}.CompareList(other.{property.Name})");
                else if (property.Type.StartsWith("string")) me.AppendIndent(indent_level + 1).Append($"return {property.Name}.CompareString(other.{property.Name})");
                else me.AppendIndent(indent_level + 1).Append($"return {property.Name} == other.{property.Name}");

                break;
            }
            else
            {
                if (next == 0) me.AppendIndent(indent_level + 1).Append("return");
                if (previous != next) me.Append(" &&");

                if (property.Type.Contains("List<"))
                {
                    me
                    .AppendLine()
                    .AppendIndent(indent_level + 2).Append($"{property.Name}.CompareList(other.{property.Name})");
                }
                else if (property.Type.StartsWith("string"))
                {
                    me
                    .AppendLine()
                    .AppendIndent(indent_level + 2).Append($"{property.Name}.CompareString(other.{property.Name})");
                }
                else
                {
                    me
                    .AppendLine()
                    .AppendIndent(indent_level + 2).Append($"{property.Name} == other.{property.Name}");
                }
            }

            previous = next;
            next++;
        }

        return me
                .Append(';').AppendLine()
                .AppendIndent(indent_level).Append('}').AppendLine();
    }

    public static StringBuilder AppendGetHashCode(this StringBuilder me, IEnumerable<(string Name, string Type)> property_infos, int indent_level)
    {
        me
        .AppendIndent(indent_level).Append("public override int GetHashCode()").AppendLine()
        .AppendIndent(indent_level).Append('{').AppendLine();

        if (property_infos.Count() == 0)
        {
            return me
                .AppendIndent(indent_level + 1).Append("return 0;").AppendLine()
                .AppendIndent(indent_level).Append('}').AppendLine();
        }

        var batches = new List<List<string>>();
        var currentBatch = new List<string>();

        foreach (var property in property_infos)
        {
            // Non-list types go into HashCode.Combine
            if (!property.Type.Contains("List<"))
            {
                currentBatch.Add(property.Name);
                if (currentBatch.Count == 7)
                {
                    batches.Add(currentBatch);
                    currentBatch = [];
                }
            }
            else
            {
                // Flush current batch if full or not empty
                if (currentBatch.Count > 0)
                {
                    batches.Add(currentBatch);
                    currentBatch = [];
                }

                // List<T> gets its own statement
                batches.Add([$"{property.Name}.GenerateHashCode"]);
            }
        }

        if (currentBatch.Count > 0)
            batches.Add(currentBatch);

        var isFirst = true;

        for (var i = 0; i < batches.Count; i++)
        {
            var batch = batches[i];

            if (batch.Count == 1 && batch[0].Contains("GenerateHashCode"))
            {
                if (isFirst)
                {
                    me.AppendIndent(indent_level + 1).Append($"var hash = {batch[0]}(0);").AppendLine();
                    isFirst = false;
                }
                else me.AppendIndent(indent_level + 1).Append($"hash = {batch[0]}(hash);").AppendLine();
            }
                
            else
            {
                var args = string.Join(", ", batch);
                if (isFirst)
                {
                    me.AppendIndent(indent_level + 1).Append($"var hash = HashCode.Combine({args});").AppendLine();
                    isFirst = false;
                }
                else 
                    me.AppendIndent(indent_level + 1).Append($"hash = HashCode.Combine(hash, {args});").AppendLine();
            }
        }

        return me
            .AppendIndent(indent_level + 1).Append("return hash;").AppendLine()
            .AppendIndent(indent_level).Append('}').AppendLine();
    }

    public static StringBuilder AppendOperator(this StringBuilder me, string class_name, int indent_level)
        => me
            .AppendIndent(indent_level).Append($"public static bool operator ==({class_name}? left, {class_name}? right)").AppendLine()
            .AppendIndent(indent_level).Append('{').AppendLine()
            .AppendIndent(indent_level + 1).Append("if (left is null) return right is null;").AppendLine()
            .AppendIndent(indent_level + 1).Append("return left.Equals(right);").AppendLine()
            .AppendIndent(indent_level).Append('}').AppendLine()
            .AppendLine()
            .AppendIndent(indent_level).Append($"public static bool operator !=({class_name}? left, {class_name}? right)").AppendLine()
            .AppendIndent(indent_level + 1).Append("=> !(left == right);").AppendLine();

    public static StringBuilder AppendToJSON(this StringBuilder me, int indent_level)
        => me
            .AppendIndent(indent_level).Append("public string ToJSON()").AppendLine()
            .AppendIndent(indent_level + 1).Append("=> JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);").AppendLine();

    public static StringBuilder AppendToString(this StringBuilder me, int indent_level)
        => me
            .AppendIndent(indent_level).Append("public override string ToString()").AppendLine()
            .AppendIndent(indent_level + 1).Append("=> JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);").AppendLine();
}
