namespace xarsu.Reference;

[AttributeUsage(AttributeTargets.Method)]
public class Il2CppHookAttribute : Attribute
{
    public Type DeclaringType { get; }
    public string MethodName { get; }
    public Type[]? ParameterTypes { get; } = null;

    public Il2CppHookAttribute(Type declaringType, string methodName)
    {
        DeclaringType = declaringType;
        MethodName = methodName;
    }

    public Il2CppHookAttribute(Type declaringType, string methodName, Type[] parameterTypes)
    {
        DeclaringType = declaringType;
        MethodName = methodName;
        ParameterTypes = parameterTypes;
    }
}