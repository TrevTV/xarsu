using Cpp2IL.Core.Model.Contexts;
using System.Diagnostics.CodeAnalysis;

namespace xarsu.Generator.Extensions;

internal static class TypeAnalysisContextExtensions
{
    extension(TypeAnalysisContext type)
    {
        public bool IsModuleType => type.DeclaringType is null && string.IsNullOrEmpty(type.DefaultNamespace) && type.DefaultName == "<Module>";
        public bool IsPrivateImplementationDetailsType => type.DeclaringType is null && string.IsNullOrEmpty(type.DefaultNamespace) && type.DefaultName == "<PrivateImplementationDetails>";
        public bool HasGenericParameters => type.GenericParameters.Count > 0;

        [MaybeNull]
        public Type SourceType
        {
            get => type.GetExtraData<Type>("SourceType");
            set => type.PutExtraData("SourceType", value);
        }

        [MaybeNull]
        public MethodAnalysisContext PointerConstructor
        {
            get => type.GetExtraData<MethodAnalysisContext>("PointerConstructor");
            set => type.PutExtraData("PointerConstructor", value);
        }

        public bool IsUnstripped
        {
            get => type.GetExtraData<object>("Unstripped") is true;
            set => type.PutExtraData<object>("Unstripped", value);
        }

        public KnownTypeCode KnownType
        {
            get => type.GetExtraStruct("KnownType", KnownTypeCode.None);
            set => type.PutExtraStruct("KnownType", value);
        }

        public FieldAnalysisContext GetFieldByName(string? name)
        {
            return type.TryGetFieldByName(name) ?? throw new Exception($"Field {name} not found in type {type.Name}");
        }

        public FieldAnalysisContext? TryGetFieldByName(string? name)
        {
            for (var i = type.Fields.Count - 1; i >= 0; i--)
            {
                var field = type.Fields[i];
                if (field.Name == name)
                {
                    return field;
                }
            }
            return null;
        }

        public bool TryGetFieldByName(string? name, [NotNullWhen(true)] out FieldAnalysisContext? field)
        {
            field = type.TryGetFieldByName(name);
            return field is not null;
        }

        public MethodAnalysisContext GetImplicitConversionFrom(TypeAnalysisContext sourceType)
        {
            return GetConversion("op_Implicit", type, sourceType, type.SelfInstantiateIfGeneric());
        }

        public MethodAnalysisContext GetImplicitConversionTo(TypeAnalysisContext targetType)
        {
            return GetConversion("op_Implicit", type, type.SelfInstantiateIfGeneric(), targetType);
        }

        public MethodAnalysisContext GetExplicitConversionFrom(TypeAnalysisContext sourceType)
        {
            return GetConversion("op_Explicit", type, sourceType, type.SelfInstantiateIfGeneric());
        }

        public MethodAnalysisContext GetExplicitConversionTo(TypeAnalysisContext targetType)
        {
            return GetConversion("op_Explicit", type, type.SelfInstantiateIfGeneric(), targetType);
        }

        public TypeAnalysisContext MaybeMakeGenericInstanceType(IReadOnlyCollection<TypeAnalysisContext> genericArguments)
        {
            if (type.GenericParameters.Count == 0)
            {
                return type;
            }
            else
            {
                return type.MakeGenericInstanceType(genericArguments);
            }
        }

        public TypeAnalysisContext SelfInstantiateIfGeneric() => type.MaybeMakeGenericInstanceType(type.GenericParameters);

        private static MethodAnalysisContext GetConversion([ConstantExpected] string name, TypeAnalysisContext declaringType, TypeAnalysisContext sourceType, TypeAnalysisContext targetType)
        {
            return declaringType.Methods.First(m =>
            {
                return m.Name == name
                    && m.IsStatic
                    && m.Parameters.Count == 1
                    && TypeAnalysisContextEqualityComparer.Instance.Equals(m.ReturnType, targetType)
                    && TypeAnalysisContextEqualityComparer.Instance.Equals(m.Parameters[0].ParameterType, sourceType);
            });
        }
    }
}