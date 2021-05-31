using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public enum SortOrder { Ascending, Descending }
    public class OrderBy
    {
        public List<Tuple<Column, SortOrder>> Columns { get; }
        public OrderBy()
        {
            this.Columns = new List<Tuple<Column, SortOrder>>();
        }
    }
}
