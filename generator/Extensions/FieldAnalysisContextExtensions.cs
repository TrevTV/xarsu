using Cpp2IL.Core.Model.Contexts;

namespace xarsu.Generator.Extensions;

internal static class FieldAnalysisContextExtensions
{
    extension(FieldAnalysisContext fieldCtx)
    {
        public bool IsUnstripped
        {
            get => fieldCtx.GetExtraData<object>("Unstripped") is true;
            set => fieldCtx.PutExtraData<object>("Unstripped", value);
        }
    }
}