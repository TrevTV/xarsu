namespace xarsu.Generator.Operands;

public sealed class EndLabel : ILabel
{
    public static readonly EndLabel Instance = new();
    private EndLabel()
    {
    }
}
