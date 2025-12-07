namespace Soat.Eleven.FastFood.Core.ConditionRules;

public abstract class ConditionException<T>
{
    internal T Target { get; set; }
    internal string ArgumentName { get; set; }

    internal ConditionException(T target, string argumentName)
    {
        Target = target;
        ArgumentName = argumentName;
    }
}
