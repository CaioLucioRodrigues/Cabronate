using Cabronate.DAO.QueryBuilder.Statements;

namespace Cabronate.DAO.QueryBuilder.Interfaces
{
    public interface IQueryStatements
    {
        string SelectStatement(Select select);

        string JoinStatement(Join join);

        string GroupByStatement(GroupBy groupby);

        string ConditionStatement(Condition condition);

        string OrderByStatement(OrderBy orderby);

        string OffsetPaginationStatement(Pagination pagination);

        string AliasStatement(string name, string alias);

        string QueryCommand(Query query);

        string CountCommand(Query query, bool filtering);
    }
}
