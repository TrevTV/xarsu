using AsmResolver.DotNet;
using Cpp2IL.Core.Model.Contexts;
using System.Reflection;
using Cpp2IL.Core.Utils.AsmResolver;

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

        public bool ImplementsAnInterfaceMethod
        {
            get
            {
                var count = 0;
                foreach (var x in method.Overrides)
                {
                    count++;
                    if (count > 1)
                    {
                        return true;
                    }
                }
                return count == 1 && method.BaseMethod is null;
            }
        }
    }
}