using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public enum OperatorType { Equal }
    public class Where
    {
        public List<Condition> Conditions { get; }

        public Where()
        {
            Conditions = new List<Condition>();
        }
    }
    public class Condition
    {
        public string LeftOperand { get; set; }

        public string RightOperand { get; set; }

        public OperatorType Operator { get; set; }
        
        /// <summary>
        /// Determina se o filtro é constante.
        /// </summary>
        public bool IsConstant { get; set; }

        public string FreeSql { get; private set; }

        public Condition() { }

        public Condition(OperatorType @operator, string left, string right)
        {
            Operator = @operator;
            LeftOperand = left;
            RightOperand = right;
        }
        
        public Condition(string freeSql)
        {
            FreeSql = freeSql;
        }
        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(FreeSql))
                return FreeSql;

            return $"{LeftOperand} = {RightOperand}";
        }
    }
}
