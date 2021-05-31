using Cabronate.DAO.QueryBuilder.Interfaces;
using Cabronate.DAO.QueryBuilder.Statements;
using Cabronate.DAO.SQL;
using System;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Dialects
{
    public class SqlANSI : IQueryStatements
    {
        public virtual string AliasStatement(string name, string alias)
        {
            return $"{name} {ANSIWords.AS} {alias}";
        }

        public virtual string ConditionStatement(Condition condition)
        {
            return $"AND {condition.FreeSql} ";
        }

        public virtual string GroupByStatement(GroupBy groupby)
        {
            throw new System.NotImplementedException();
        }

        public virtual string JoinStatement(Join join)
        {
            string type = GetJoinType(join);
            string tbl = (join.Table.HasAlias) ? AliasStatement(join.Table.Name, join.Table.Alias) : join.Table.Name;

            return $"{type} {ANSIWords.JOIN} {tbl} {ANSIWords.ON} ({join.Conditions.First()}) ";
        }

        private static string GetJoinType(Join join)
        {
            if (!join.Type.HasValue)
                return ANSIWords.INNER;

            switch (join.Type.Value)
            {
                case JoinType.Inner:
                    return ANSIWords.INNER;
               case JoinType.Left:
                    return ANSIWords.LEFT;
                case JoinType.Right:
                    return ANSIWords.RIGHT;
                case JoinType.Full:
                    return ANSIWords.FULL;
                default:
                    return ANSIWords.INNER;
            }
        }

        public virtual string OffsetPaginationStatement(Pagination pagination)
        {
            throw new System.NotImplementedException();
        }

        public virtual string OrderByStatement(OrderBy orderby)
        {
            if (orderby is null) throw new ArgumentNullException(nameof(orderby));
            if (!orderby.Columns.Any()) throw new Exception("Sem colunas para ordenar");

            StringBuilder stat = new StringBuilder(ANSIWords.ORDERBY);
            orderby.Columns.ForEach(col => stat.Append($" {col.Item1.FreeSql} {((col.Item2 == SortOrder.Ascending) ? ANSIWords.ASC : ANSIWords.DESC)} "));

            return stat.ToString();
        }

        public virtual string SelectStatement(Select select)
        {
            string strColumns;

            string from = (select.TableFrom.HasAlias) 
                ? AliasStatement(select.TableFrom.Name, select.TableFrom.Alias) 
                : select.TableFrom.Name;

            if (select.Columns.Any())
            {
                strColumns = string.Join($"{ANSIWords.COMMA} ", select.Columns.Select(c =>
                {
                    return (string.IsNullOrEmpty(c.FreeSql)) ? 
                               ((c.HasAlias) ? 
                                   AliasStatement(c.ColumnName, c.Alias) : 
                                   c.ColumnName) : 
                               c.FreeSql;
                }));
            }
            else
                strColumns = ANSIWords.ASTERISK;

            if(select.Distinct) 
                return $"{ANSIWords.SELECT} {ANSIWords.DISTINCT} {strColumns} {ANSIWords.FROM} {from} ";

            return $"{ANSIWords.SELECT} {strColumns} {ANSIWords.FROM} {from} ";
        }

        public virtual string QueryCommand(Query query)
        {
            //SELECT column1, column2, ... FROM table_name
            StringBuilder queryCommand = new StringBuilder(SelectStatement(query.Select)).AppendLine();

            //JOIN
            query.Joins.ForEach(join => queryCommand.AppendLine(JoinStatement(join)));

            //WHERE 1 = 1
            if(query.Where != null && query.Where.Conditions.Any())
            {
                queryCommand.AppendLine($"{ANSIWords.WHERE} 1 {ANSIWords.EQUAL} 1 ");
                query.Where.Conditions.ForEach(condition => queryCommand.AppendLine(ConditionStatement(condition)));
            }

            //GROUP BY
            if (query.GroupBy != null && query.GroupBy.Columns.Any())
                queryCommand.AppendLine(GroupByStatement(query.GroupBy));

            //ORDER BY
            if (query.OrderBy != null && query.OrderBy.Columns.Any())
                queryCommand.AppendLine(OrderByStatement(query.OrderBy));
            //pagination (LIMIT, ROWS... TO, etc)
            if (query.OffsetPagination?.Number > 0 || query.OffsetPagination?.Offset > 0)
                queryCommand.AppendLine(OffsetPaginationStatement(query.OffsetPagination));


            return queryCommand.ToString();
        }

        public string CountCommand(Query query, bool filtering)
        {
            //SELECT column1, column2, ... FROM table_name
            Select count = new Select(query.Select.TableFrom);
            count.Columns.Add(new Column("COUNT(*)"));
            StringBuilder queryCommand = new StringBuilder(SelectStatement(count)).AppendLine();

            //JOINS
            query.Joins.ForEach(join => queryCommand.AppendLine(JoinStatement(join)));

            //WHERE 1 = 1
            queryCommand.AppendLine($"{ANSIWords.WHERE} 1 {ANSIWords.EQUAL} 1 ");
            
            if(query.Where != null && query.Where.Conditions.Any())
            {
                query.Where.Conditions.ForEach(condition => {
                    if (filtering || (!filtering && condition.IsConstant))//se a contagem não estiver considerando os filtros ou tiver considerando mas o filtro for fixo
                        queryCommand.AppendLine(ConditionStatement(condition));
                });
            }

            //GROUP BY
            if (query.GroupBy != null && query.GroupBy.Columns.Any())
                queryCommand.AppendLine(GroupByStatement(query.GroupBy));

            return queryCommand.ToString();
        }
    }
}
