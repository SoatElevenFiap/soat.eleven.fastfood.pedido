namespace Soat.Eleven.Pedidos.Core.ConditionRules;

public class RequiredException<T> : ConditionException<T>
{
    public RequiredException(T target, string argumentName) : base(target, argumentName)
    {
    }
}
