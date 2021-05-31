using Cabronate.Base;
using Cabronate.Base.Caching;
using Cabronate.DAO.QueryBuilder.Interfaces;
using Cabronate.DAO.QueryBuilder.Statements;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.QueryBuilder
{
    [Serializable]
    public class Query
    {
        public DBContexto DBContext { get; }

        public IDbCommand DBCommand { get; }

        public int Count { get; private set; }

        public int CountFiltered { get; private set; }        

        public DataTable Records { get; private set; }
        
        public string QueryString { 
            get
            {
                var builder = QueryFactory.GetQueryBuilder(DBContext);
                var strQuery = builder.QueryCommand(this);
                return strQuery;
            } 
        }

        #region Statements
        public Select Select { get; }
        public List<Join> Joins { get; }
        public Where Where { get; set; }
        public GroupBy GroupBy { get; set; }
        public OrderBy OrderBy { get; set; }
        public Pagination OffsetPagination { get; set; }
        #endregion

        public List<Table> QueryTables 
        {
            get
            {
                var lst = new List<Table>();
                lst.Add(Select.TableFrom);
                lst.AddRange(Joins.Select(j => j.Table));
                return lst;
            }
        }

        public List<Condition> QueryConditions
        {
            get
            {
                var lst = new List<Condition>();
                if(Where != null)
                    lst.AddRange(Where.Conditions);

                Joins.ForEach(j => lst.AddRange(j.Conditions));
                
                return lst;
            }
        }

        public Query(DBContexto dbctx)
        {
            DBContext = dbctx;
            DBCommand = dbctx.CreateCommand("");
            Select = new Select();
            Joins = new List<Join>();

        }

        public Table GetTableByName(string name)
        {
            return QueryTables.Find(t => t.Name == name);
        }

        public Table GetTableByAlias(string alias)
        {
            return QueryTables.Find(t => t.Alias == alias);
        }

        internal Table FindTable(string name)
        {
            Table tb = GetTableByAlias(name);

            if (tb is null)
                tb = GetTableByName(name);

            if(tb is null)
                throw new KeyNotFoundException($"Tabela {name} não utilizada na query");

            return tb;
        }

        public void RunQuery()
        {
            DBCommand.CommandText = QueryString;
            Records = DBContext.GetDataTable(DBCommand);
        }

        public void CountAllRecords()
        {
            var builder = QueryFactory.GetQueryBuilder(DBContext);
            var cmd = builder.CountCommand(this, filtering: false);
            Count = RunCountQuery(cmd, filtering: false);
        }

        public void CountFilteredRecords()
        {
            var builder = QueryFactory.GetQueryBuilder(DBContext);
            var cmd = builder.CountCommand(this, filtering: true);
            CountFiltered = RunCountQuery(cmd, filtering: true); 
        }

        private int RunCountQuery(string query, bool filtering)
        {
            QueryStatistics qs;
            var cacheKey = GetCountQueryCacheKey(query, filtering);
            
            if (WGlobal.HasAnyValue(cacheKey))
                return WGlobal.Get<QueryStatistics>(cacheKey).Value;                                
            else
                qs = new QueryStatistics(cacheKey);
            
            DBCommand.CommandText = query;
            DataTable dt = DBContext.GetDataTable(DBCommand);
            qs.Value = Convert.ToInt32(dt.Rows[0][0]);
            
            return qs.Value;
        }

        private string GetCountQueryCacheKey(string query, bool withFilters)
        {
            var sb = new StringBuilder(query);
            if(withFilters)
            {
                var lst = new List<string>();
                for(int i = 0; i < DBCommand.Parameters.Count; i++)
                {
                    if (DBCommand.Parameters[i] is IDataParameter p)
                        lst.Add(Convert.ToString(p.Value));
                }
                sb.Append($";{String.Join(";", lst)}");
            }
            return Cabronate.Base.EcalcString.MD5Hash(sb.ToString());
        }
    }
}
