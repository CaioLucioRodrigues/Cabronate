using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public enum JoinType { Inner = 0, Left = 1, Right = 2, Full = 3}
    public class Join
    {
        public JoinType? Type { get; set; }

        public Table Table { get; set; }

        public List<Condition> Conditions { get; } 

        public Join()
        {
            Conditions = new List<Condition>();
            Table = new Table();
        }

        public Join(Table table, Condition condition)
        {
            Table = table;
            Conditions = new List<Condition>() { condition };
        }

        public Join(JoinType type, Table table, Condition condition)
            : this(table, condition)
        {
            Type = type;
        }
    }
}
