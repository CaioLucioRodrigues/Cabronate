using Cabronate.Base;
using System;
using System.Text;

namespace Cabronate.DAO.SQL
{
    public static class ConditioningFactory
    {
        public static string Where()
        {
            return " WHERE ( 1 = 1 ) ";
        }

        public static string GetCondition(ConditioningStrut condition)
        {

            switch (condition.Operator)
            {
                case OperatorType.equal: return Equal(condition);
                case OperatorType.equalString: return EqualString(condition);
                case OperatorType.equalStringIgnoreCase: return EqualStringIgnoreCase(condition);
                case OperatorType.greater: return Greater(condition);
                case OperatorType.greaterequal: return Greaterequal(condition);
                case OperatorType.greaterequalString: return GreaterequalString(condition);
                case OperatorType.less: return Less(condition);
                case OperatorType.lessequal: return Lessequal(condition);
                case OperatorType.lessequalString: return LessequalString(condition);
                case OperatorType.like: return Like(condition);
                case OperatorType._in: return _in(condition);
                case OperatorType.freesql: return Freesql(condition);
                case OperatorType.notin: return Notin(condition);
                case OperatorType.different: return Different(condition);
                case OperatorType.differentString: return DifferentString(condition);
                case OperatorType.differentStringIgnoreCase: return DifferentStringIgnoreCase(condition);
                case OperatorType.dateBetween: return DateBetween(condition);
                case OperatorType.greaterString: return GreaterString(condition);
                case OperatorType.equalStringIgnoreCaseAndSpaces: return EqualStringIgnoreCaseAndSpaces(condition);
                case OperatorType.daysAgos: return DaysAgo(condition);
                case OperatorType.thisMonth: return ThisMonth(condition);
                case OperatorType.thisWeek: return ThisWeek(condition);
                case OperatorType.lateDate: return LateDate(condition);
                case OperatorType.today: return Today(condition);
                default: return "";            
            }
        }
        public static string GetJoin(ConditioningStrut condition)
        {
            switch (condition.Operator)
            {
                case OperatorType.join: return Join(condition);
                case OperatorType.leftJoin: return Leftjoin(condition);
                default: return "";
            }
        }

        public static string GetOrderBy(ConditioningStrut condition)
        {
            if (condition.Operator == OperatorType.orderBy)
                return Orderby(condition);
            else return "";
        }

        public static string GetLimit(ConditioningStrut condition)
        {
            if (!(condition.Value is Limit))
                throw new Exception("Parametros inválidos para OperatorType.limit");
            if (condition.Operator == OperatorType.limit)
                return Limit(condition);

            return String.Empty;
        }

        private static string EqualStringIgnoreCaseAndSpaces(ConditioningStrut condition)
        {
            return string.Format(" AND TRIM(UPPER({0})) = TRIM(UPPER('{1}')) ", 
                condition.Field, 
                DBContexto.QuoteSQLS((string)condition.Value));
        }

        private static string Equal(ConditioningStrut condition)
        {
            return string.Format(" AND {0} = {1} ", condition.Field, Convert.ToString(condition.Value));
        }

        private static string Different(ConditioningStrut condition)
        {
            return string.Format(" AND {0} <> {1} ", condition.Field, Convert.ToString(condition.Value));
        }

        private static string Greater(ConditioningStrut condition)
        {
            return string.Format(" AND {0} > {1} ", condition.Field, (string)condition.Value);
        }

        private static string GreaterString(ConditioningStrut condition)
        {
            return string.Format(" AND {0} > '{1}' ", condition.Field, (string)condition.Value);
        }

        private static string Greaterequal(ConditioningStrut condition)
        {
            return string.Format(" AND {0} >= {1} ", condition.Field, (string)condition.Value);
        }


        private static string GreaterequalString(ConditioningStrut condition)
        {
            return string.Format(" AND {0} >= '{1}' ", condition.Field, (string)condition.Value);
        }

        private static string Less(ConditioningStrut condition)
        {
            return string.Format(" AND {0} < {1} ", condition.Field, (string)condition.Value);
        }

        private static string Lessequal(ConditioningStrut condition)
        {
            return string.Format(" AND {0} <= {1} ", condition.Field, (string)condition.Value);
        }

        private static string LessequalString(ConditioningStrut condition)
        {
            return string.Format(" AND {0} <= '{1}' ", condition.Field, (string)condition.Value);
        }

        private static string Like(ConditioningStrut condition)
        {
            return string.Format(" AND UPPER({0}) LIKE UPPER('%{1}%') ", condition.Field, (string)condition.Value);
        }

        private static string _in(ConditioningStrut condition)
        {
            return string.Format(" AND {0} IN ({1}) ", condition.Field, (string)condition.Value);
        }

        private static string Freesql(ConditioningStrut condition)
        {
            return string.Format(" AND ({0}) ", (string)condition.Value);
        }

        private static string EqualString(ConditioningStrut condition)
        {
            return string.Format(" AND {0} = '{1}' ", condition.Field, DBContexto.QuoteSQLS((string)condition.Value));
        }

        private static string DifferentString(ConditioningStrut condition)
        {
            return string.Format(" AND {0} <> '{1}' ", condition.Field, DBContexto.QuoteSQLS((string)condition.Value));
        }

        private static string EqualStringIgnoreCase(ConditioningStrut condition)
        {
            return string.Format(" AND UPPER({0}) = UPPER('{1}') ", condition.Field, DBContexto.QuoteSQLS((string)condition.Value));
        }

        private static string DifferentStringIgnoreCase(ConditioningStrut condition)
        {
            return string.Format(" AND UPPER({0}) = UPPER('{1}') ", condition.Field, DBContexto.QuoteSQLS((string)condition.Value));
        }

        private static string Notin(ConditioningStrut condition)
        {
            if (((string)condition.Value).Trim() != "")
            {
                return string.Format(" AND {0} NOT IN ({1}) ", condition.Field, (string)condition.Value);
            }
            return string.Empty;
        }

        private static string Join(ConditioningStrut condition)
        {
            if (((string)condition.Value).Trim() != "")
            {
                return string.Format(" JOIN {0} ON ({1}) ", condition.Field, (string)condition.Value);
            }
            return string.Empty;
        }

        private static string Orderby(ConditioningStrut condition)
        {
            if (((string)condition.Value).Trim() != "")
                return String.Format(" {0} {1}", condition.Field, (string)condition.Value);

            return string.Empty;
        }


        private static string Limit(ConditioningStrut condition)
        {
            Limit param = (Limit)condition.Value;
            StringBuilder str = new StringBuilder();
            if (param.Number <= 0 && param.Offset > 0)
                param.Number = 1;
            if (param.Number > 0)
                str.Append($"ROWS {param.Number}");
            if (param.Offset > 0)
                str.Append($" TO {param.Offset}");

            return str.ToString();
        }

        private static string Leftjoin(ConditioningStrut condition)
        {
            if (((string)condition.Value).Trim() != "")
            {
                return string.Format(" LEFT JOIN {0} ON ({1}) ", condition.Field, (string)condition.Value);
            }
            return string.Empty;
        }

        private static string DateBetween(ConditioningStrut condition)
        {
            return string.Format(" AND {0} BETWEEN {1} AND {2} ", condition.Field, (string)condition.Value, (string)condition.AditionalValue);
        }

        private static string DaysAgo(ConditioningStrut condition)
        {
            return $" AND {condition.Field} >= {condition.dbctx.FormataDataSQL(DateTime.Now.AddDays(Convert.ToInt32(condition.Value) * -1))}";
        }

        private static string ThisMonth(ConditioningStrut condition)
        {   
            var firstDayOfMonth = EcalcDateTime.GetFirstDayMonth(DateTime.Now);
            var lastDayOfMonth = EcalcDateTime.GetLastDayMonth(DateTime.Now);            

            return $" AND {condition.Field} BETWEEN {condition.dbctx.FormataDataSQL(firstDayOfMonth)} AND {condition.dbctx.FormataDataSQL(lastDayOfMonth)}";
        }

        private static string ThisWeek(ConditioningStrut condition)
        {
            var firstDayOfWeek = EcalcDateTime.StartOfWeek(DateTime.Now);            
            var lastDayOfWeek = EcalcDateTime.EndOfWeek(DateTime.Now); 

            return $" AND {condition.Field} BETWEEN {condition.dbctx.FormataDataSQL(firstDayOfWeek)} AND {condition.dbctx.FormataDataSQL(lastDayOfWeek)}";
        }

        private static string LateDate(ConditioningStrut condition)
        {
            return string.Format($" AND {condition.Field} < CURRENT_TIMESTAMP ");
        }

        private static string Today(ConditioningStrut condition)
        {
            var startOfDay = DateTime.Now.StartOfDay();
            var endOfDay = DateTime.Now.EndOfDay();

            return $" AND {condition.Field} BETWEEN {condition.dbctx.FormataDataSQL(startOfDay)} AND {condition.dbctx.FormataDataSQL(endOfDay)}";
        }
    }
}
