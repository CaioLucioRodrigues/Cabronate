using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public class GroupBy
    {
        public List<Column> Columns { get; }

        public GroupBy()
        {
            this.Columns = new List<Column>();
        }
    }
}
