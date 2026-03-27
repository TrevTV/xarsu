using Cpp2IL.Core.Model.Contexts;

namespace xarsu.Generator.Extensions;

internal static class PropertyAnalysisContextExtensions
{
    extension(PropertyAnalysisContext property)
    {
        public bool IsUnstripped
        {
            get => property.GetExtraData<object>("Unstripped") is true;
            set => property.PutExtraData<object>("Unstripped", value);
        }
    }
}