using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public class Column
    {
        public Table Table { get; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string FreeSql { get; set; }
        public bool HasAlias => !String.IsNullOrWhiteSpace(this.Alias);
        public string ColumnName => (Table.HasAlias) ? $"{Table.Alias}.{this.Name}" : $"{Table.Name}.{this.Name}";
        
        public Column()
        {
            Table = new Table();
        }
        public Column(Table table, string columnName, string @as = null)
        {
            this.Table = table;
            this.Name = columnName;
            this.Alias = @as;
        }
        public Column(string freeSql)
        {
            this.Table = new Table();
            this.FreeSql = freeSql;
        }
    }
}
