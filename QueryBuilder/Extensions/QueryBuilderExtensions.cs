using Cabronate.Base;
using Cabronate.DAO.QueryBuilder.Dialects;
using Cabronate.DAO.QueryBuilder.Statements;
using Cabronate.DAO.SQL;
using System;
using System.Data;

namespace Cabronate.DAO.QueryBuilder.Extensions
{
    public static class QueryBuilderExtensions
    {
        public static Query From(this DBContexto dbctx, string tableName, string @as = null)
        {
            if (dbctx is null) throw new ArgumentNullException(nameof(dbctx));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(nameof(tableName));

            var query = new Query(dbctx);

            query.Select.TableFrom.Name = tableName;
            query.Select.TableFrom.Alias = @as;
            return query;
        }

        public static Query Join(this Query query, string table, string @as, string leftField, Statements.OperatorType @operator, string rightField)
        {
            Join join = new Join(new Table(table, @as), new Condition(@operator, leftField, rightField));
            query.Joins.Add(join);
            return query;
        }

        public static Query Join(this Query query, string table, string @as, string freeSql)
        {
            Join join = new Join(new Table(table, @as), new Condition(freeSql));
            query.Joins.Add(join);
            return query;
        }

        public static Query LeftJoin(this Query query, string table, string @as, string leftField, Statements.OperatorType @operator, string rightField)
        {
            Join join = new Join(JoinType.Left, new Table(table, @as), new Condition(@operator, leftField, rightField));
            query.Joins.Add(join);
            return query;
        }

        public static Query LeftJoin(this Query query, string table, string @as, string freeSql)
        {
            Join join = new Join(JoinType.Left, new Table(table, @as), new Condition(freeSql));
            query.Joins.Add(join);
            return query;
        }

        public static Query RightJoin(this Query query, string table, string @as, string leftField, Statements.OperatorType @operator, string rightField)
        {
            Join join = new Join(JoinType.Right, new Table(table, @as), new Condition(@operator, leftField, rightField));
            query.Joins.Add(join);
            return query;
        }

        public static Query RightJoin(this Query query, string table, string @as, string freeSql)
        {
            Join join = new Join(JoinType.Right, new Table(table, @as), new Condition(freeSql));
            query.Joins.Add(join);
            return query;
        }

        /// <summary>
        /// Colunas que devem ser selecionadas na query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columns">Array de tuplas (TABELA|ALIAS, NOME COLUNA, ALIAS COLUNA)</param>
        /// <returns></returns>
        public static Query Select(this Query query, params Tuple<string,string,string>[] columns)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            foreach (var col in columns)
            {
                Table tbl = query.FindTable(col.Item1);
                query.Select.Columns.Add(new Column(tbl, col.Item2, col.Item3));
            }
            return query;
        }

        /// <summary>
        /// Colunas que devem ser selecionadas na query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static Query Select(this Query query, params string[] columns)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            
            foreach (var sql in columns)
                query.Select.Columns.Add(new Column(sql));
            return query;
        }

        public static Query Where(this Query query, string freeSql)
        {
            if(query is null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(freeSql)) throw new ArgumentNullException(nameof(freeSql));

            query.Where = new Where();

            query.Where.Conditions.Add(new Condition(freeSql));

            return query;
        }

        public static Query Where(this Query query, Condition condition)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (condition is null) throw new ArgumentNullException(nameof(condition));


            query.Where = new Where();
            query.Where.Conditions.Add(condition);

            return query;
        }

        public static Query And(this Query query, Condition condition)
        {
            if (query.Where is null) throw new NullReferenceException("Tentando usar uma cláusula AND sem WHERE");
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (condition is null) throw new ArgumentNullException(nameof(condition));
            
            query.Where.Conditions.Add(condition);
            return query;
        }

        public static Query And(this Query query, string freeSql)
        {
            if (query.Where is null) throw new NullReferenceException("Tentando usar uma cláusula AND sem WHERE");
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(freeSql)) throw new ArgumentNullException(nameof(freeSql));


            query.Where.Conditions.Add(new Condition(freeSql));
            return query;
        }

        public static Query Or(this Query query)
        {
            return query;
        }

        public static Query OrderBy(this Query query, string table, string column, SortOrder? order = null)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(table)) throw new ArgumentNullException(nameof(table));
            if (String.IsNullOrWhiteSpace(column)) throw new ArgumentNullException(nameof(column));

            query.OrderBy = new OrderBy();

            Column col = new Column(query.FindTable(table),column);
            query.OrderBy.Columns.Add(new Tuple<Column, SortOrder>(col, order.GetValueOrDefault(SortOrder.Ascending)));
            return query;
        }

        public static Query OrderBy(this Query query, string column, SortOrder? order = null)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(column)) throw new ArgumentNullException(nameof(column));

            query.OrderBy = new OrderBy();

            Column col = new Column(column);
            query.OrderBy.Columns.Add(new Tuple<Column, SortOrder>(col, order.GetValueOrDefault(SortOrder.Ascending)));
            return query;
        }

        public static Query ThenOrderBy(this Query query, string table, string column, SortOrder? order = null)
        {
            if(query.OrderBy is null) throw new NullReferenceException("Tentando usar ThenOrderBy sem a cláusula OrderBy");
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (String.IsNullOrWhiteSpace(table)) throw new ArgumentNullException(nameof(table));
            if (String.IsNullOrWhiteSpace(column)) throw new ArgumentNullException(nameof(column));
            
            Column col = new Column(query.FindTable(table), column);
            query.OrderBy.Columns.Add(new Tuple<Column, SortOrder>(col, order.GetValueOrDefault(SortOrder.Ascending)));

            return query;
        }

        public static Query Take(this Query query, int count)
        {
            if (query.OffsetPagination is null)
                query.OffsetPagination = new Pagination();

            query.OffsetPagination.Number = count;
            return query;
        }

        public static Query Skip(this Query query, int count)
        {
            if (query.OffsetPagination is null)
                query.OffsetPagination = new Pagination();

            //TODO: validar negativos

            query.OffsetPagination.Offset = count;
            return query;
        }

        public static Query GroupBy(this Query query)
        {

            query.GroupBy = new GroupBy();
            return query;
        }

        public static Query ThenGroupBy(this Query query)
        {
            if (query.GroupBy is null) throw new NullReferenceException("Tentando usar ThenGroupBy sem a cláusula GroupBy");
            
            return query;
        }

        public static Query Distinct(this Query query)
        {
            query.Select.Distinct = true;
            return query;
        }

        public static DataTable Run(this Query query, bool withStatistics = false)
        {
            query.RunQuery();

            if (withStatistics)
            {
                query.CountAllRecords();
                query.CountFilteredRecords();
            }

            return query.Records;
        }

        public static int Count(this Query query)
        {
            return 0;
        }
    }
}
