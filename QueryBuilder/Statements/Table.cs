using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public class Table
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool HasAlias => !String.IsNullOrWhiteSpace(this.Alias);

        public Table()
        {

        }
        public Table(string tableName, string @as = null)
        {
            Name = tableName;
            Alias = @as;
        }
    }
}
