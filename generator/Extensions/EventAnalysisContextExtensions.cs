using Cpp2IL.Core.Model.Contexts;

namespace xarsu.Generator.Extensions;

internal static class EventAnalysisContextExtensions
{
    extension(EventAnalysisContext property)
    {
        public bool IsUnstripped
        {
            get => property.GetExtraData<object>("Unstripped") is true;
            set => property.PutExtraData<object>("Unstripped", value);
        }
    }
}