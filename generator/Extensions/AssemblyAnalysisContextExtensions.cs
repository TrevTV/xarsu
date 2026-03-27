using Cpp2IL.Core.Model.Contexts;
using LibCpp2IL.BinaryStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace xarsu.Generator.Extensions;

internal static class AssemblyAnalysisContextExtensions
{
    extension (AssemblyAnalysisContext assembly)
    {
        public bool IsReferenceAssembly
        {
            get => assembly.GetExtraData<object>("ReferenceAssembly") is true;
            set => assembly.PutExtraData<object>("ReferenceAssembly", value);
        }

        public bool IsUnstripped
        {
            get => assembly.GetExtraData<object>("Unstripped") is true;
            set => assembly.PutExtraData<object>("Unstripped", value);
        }

        public InjectedTypeAnalysisContext InjectType(Type type)
        {
            var result = assembly.InjectType(type.Namespace ?? "", type.Name, null, type.Attributes);
            if (type.ContainsGenericParameters)
            {
                foreach (var genericParameter in type.GetGenericArguments())
                {
                    var genericParameterContext = new GenericParameterTypeAnalysisContext(genericParameter.Name, genericParameter.GenericParameterPosition, Il2CppTypeEnum.IL2CPP_TYPE_VAR, genericParameter.GenericParameterAttributes, result);
                    result.GenericParameters.Add(genericParameterContext);
                }
            }
            result.SourceType = type;
            return result;
        }

        public TypeAnalysisContext GetTypeByFullNameOrThrow(string fullName)
        {
            return assembly.GetTypeByFullName(fullName) ?? throw new($"Unable to find type by full name {fullName}");
        }
    }
}