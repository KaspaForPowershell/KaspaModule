namespace PWSH.Kaspa.SourceGenerators.Containers;

public sealed class DataContainer(string namespace_name, string class_name, IEnumerable<(string Name, string Type)> property_infos)
    :
    IEquatable<DataContainer>
{
    private readonly string _namespaceName = namespace_name;
    private readonly string _className = class_name;
    private readonly IEnumerable<(string Name, string Type)> _propertyInfos = property_infos;

/* -----------------------------------------------------------------
ACCESSORS                                                          |
----------------------------------------------------------------- */

    public string NamespaceName => this._namespaceName;
    public string ClassName => this._className;
    public IEnumerable<(string Name, string Type)> PropertyInfos => this._propertyInfos;

/* -----------------------------------------------------------------
EQUALITY                                                           |
----------------------------------------------------------------- */

    public bool Equals(DataContainer? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return
            NamespaceName == other.NamespaceName &&
            ClassName == other.ClassName &&
            PropertyInfos.SequenceEqual(other.PropertyInfos);
    }

    public override int GetHashCode()
    {
        var hash = 0;
        hash = hash * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this._namespaceName);
        hash = hash * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this._className);
        return hash;
    }

    public override bool Equals(object? obj)
        => Equals(obj as DataContainer);

/* -----------------------------------------------------------------
OVERRIDE                                                           |
----------------------------------------------------------------- */

    public override string ToString()
        => $"Namespace: {this._namespaceName}\nClass: {this._className}";
}