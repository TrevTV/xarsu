namespace xarsu.Reference;

[AttributeUsage(AttributeTargets.Method)]
public class MethodTokenAttribute(uint token) : Attribute
{
    public readonly uint Token = token;
}