using System.Linq.Expressions;

namespace TestDataGenerator;

public static class Extensions
{
    public static string JoinStrings<T>(this IEnumerable<T> items, string separator)
    {
        return string.Join(separator, items);
    }

    public static string? AsSqlValue(this object? value)
    {
        if (value == null) return "NULL";

        return value switch
        {
            DateTime dateTime => $"'{dateTime:yyyy-MM-dd}'",
            string asString => $"'{asString.Replace("'", "''")}'",
            decimal asDecimal => asDecimal.ToString("F2"),
            _ => value.ToString()
        };
    }

    public static string GetMemberName(this Expression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.MemberAccess:
                return ((MemberExpression)expression).Member.Name;
            case ExpressionType.Convert:
                return GetMemberName(((UnaryExpression)expression).Operand);
            case ExpressionType.Lambda:
                return GetMemberName(((LambdaExpression)expression).Body);
            default:
                throw new NotSupportedException(expression.NodeType.ToString());
        }
    }
}