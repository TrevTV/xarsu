namespace xarsu.Reference;

[AttributeUsage(AttributeTargets.Method)]
public class Il2CppHookAttribute(Type declaringType, string methodName) : Attribute
{
    public Type DeclaringType { get; } = declaringType;
    public string MethodName { get; } = methodName;
}