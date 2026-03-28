namespace xarsu.Reference;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public class OriginalTypeNameAttribute(string assemblyName, string namespce, string name) : Attribute
{
    public readonly string AssemblyName = assemblyName;
    public readonly string Namespace = namespce;
    public readonly string Name = name;
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property)]
public class OriginalNameAttribute(string name) : Attribute
{
    public readonly string Name = name;
}