using Cpp2IL.Core.Model.Contexts;
using System.Reflection;

namespace xarsu.Generator.Extensions;

internal static class MethodAnalysisContextExtensions
{
    extension(MethodAnalysisContext method)
    {
        public bool IsInstanceConstructor => method.Name == ".ctor";
        public bool IsStaticConstructor => method.Name == ".cctor";
        public bool IsConstructor => method.IsInstanceConstructor || method.IsStaticConstructor;
        public bool IsPublic => (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
        public bool IsSpecialName => (method.Attributes & MethodAttributes.SpecialName) != default;

        public bool IsUnstripped
        {
            get => method.GetExtraData<object>("Unstripped") is true;
            set => method.PutExtraData<object>("Unstripped", value);
        }
    }
}