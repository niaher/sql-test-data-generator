using System.Linq.Expressions;
using System.Text;

namespace TestDataGenerator;

public class SqlBuilder
{
    private readonly StringBuilder _sb = new();

    public string Sql => _sb.ToString();

    public void BuildSql<T>(
        IEnumerable<T> list,
        params Expression<Func<T, object?>>[] fields)
    {
        var columns = fields
            .Select(t => t.GetMemberName())
            .JoinStrings(", ");

        var functions = fields
            .Select(t => t.Compile())
            .ToList();

        var data = list
            .Select(t => functions.Select(f => f(t)?.AsSqlValue() ?? "NULL").JoinStrings(", "))
            .Select(t => $"({t})")
            .ToList();

        _sb.Append($"INSERT INTO {typeof(T).Name} ({columns}) VALUES\n\t{data.JoinStrings(",\n\t")};\n\n");
    }
}