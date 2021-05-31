using Cabronate.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.SQL
{
    public enum OperatorType
    {
        [Display(Name = "Igual à")] equal,
        [Display(Name = "Diferente de")] different,
        [Display(Name = "Igual à")] equalString,
        [Display(Name = "Diferente de")] differentString,
        [Display(Name = "Igual (ignorando maiúsculas e minúsculas) à")] equalStringIgnoreCase,
        [Display(Name = "Diferente (ignorando maiúsculas e minúsculas) de")] differentStringIgnoreCase,
        [Display(Name = "Maior que")] greater,
        [Display(Name = "Maior ou Igual que")] greaterequal,
        [Display(Name = "Menor que")] less,
        [Display(Name = "Menor ou Igual que")] lessequal,
        [Display(Name = "Menor que")] lessequalString,
        [Display(Name = "Contém")] like,
        [Display(Name = "Dentro de")] _in,
        [Display(Name = "Fora de")] notin,
        [Display(Name = "Texto Livre")] freesql,
        [Display(Name = "Ligação com")] join,
        [Display(Name = "Ligação não obrigatória com")] leftJoin,
        [Display(Name = "Data Entre")] dateBetween,
        [Display(Name = "Não Definido")] Undefined,
        [Display(Name = "Maior ou Igual que")] greaterequalString,
        [Display(Name = "Ordenar")] orderBy,
        [Display(Name = "Maior que")] greaterString,
        [Display(Name = "Limite em")] limit,
        [Display(Name = "Igual (ignorando maiúsculas, minúsculas e espaços) à")] equalStringIgnoreCaseAndSpaces,
        [Display(Name = "Dias Atrás")] daysAgos,
        [Display(Name = "Mês Atual")] thisMonth,
        [Display(Name = "Semana Atual")] thisWeek,
        [Display(Name = "Data Atrasada")] lateDate,
        [Display(Name = "Hoje")] today
    };

    public struct DateBetween
    {
        public DateTime DateFrom;
        public DateTime DateTo;
    }

    [Serializable]
    public struct Limit
    {
        public int Number;
        public int Offset;
    }

    [Serializable]
    public class ConditioningStrut
    {
        public OperatorType Operator { get; set; }
        
        public string Field { get; set; }

        public object Value { get; set; }

        public object AditionalValue { get; set; }

        public DBContexto dbctx { get; set; }

        public ConditioningStrut() {}

        public ConditioningStrut(OperatorType _operator, string field, string value)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = value;
        }

        public ConditioningStrut(OperatorType _operator, string field, int value)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = value.ToString();
        }

        public ConditioningStrut(OperatorType _operator, string field, double value)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public ConditioningStrut(OperatorType _operator, string field, decimal value)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public ConditioningStrut(OperatorType _operator, string field, DateTime value, DBContexto dbctx)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = dbctx.FormataDataSQL(value);
        }

        public ConditioningStrut(OperatorType _operator, Limit value)
        {
            this.Operator = _operator;
            this.Value = value;
        }

        public ConditioningStrut(OperatorType _operator, string field, DateBetween value, DBContexto dbctx)
        {
            this.Operator = _operator;
            this.Field = field;
            this.Value = dbctx.FormataDataSQL(value.DateFrom);
            this.AditionalValue = dbctx.FormataDataSQL(value.DateTo);
        }

        public static OperatorType StringToOperatorType(string operatorType)
        {
            switch (operatorType)
            {
                case "equal": return OperatorType.equal;
                case "equalString ": return OperatorType.equalString;
                case "equalStringIgnoreCase": return OperatorType.equalStringIgnoreCase;
                case "greater": return OperatorType.greater;
                case "greaterequal": return OperatorType.greaterequal;
                case "less": return OperatorType.less;
                case "lessequal": return OperatorType.lessequal;
                case "like": return OperatorType.like;
                case "_in": return OperatorType._in;
                case "freesql": return OperatorType.freesql;
                case "notin": return OperatorType.notin;
                case "different": return OperatorType.different;
                case "differentString": return OperatorType.differentString;
                case "differentStringIgnoreCase": return OperatorType.differentStringIgnoreCase;
                case "dateBetween": return OperatorType.dateBetween;
                case "greaterString": return OperatorType.greaterString;
                case "equalStringIgnoreCaseAndSpaces": return OperatorType.equalStringIgnoreCaseAndSpaces;
                case "daysAgos": return OperatorType.daysAgos;
                case "thisMonth": return OperatorType.thisMonth;
                case "thisWeek": return OperatorType.thisWeek;
                case "lateDate": return OperatorType.lateDate;
                case "today": return OperatorType.today;
                default: return OperatorType.Undefined;                    
            }
        }

        public static List<ConditioningStrut> CreateFilter(DBContexto dbctx, object filters)
        {
            var conditions = new List<ConditioningStrut>();
            Type type = filters.GetType();
            PropertyInfo[] properties = type.GetProperties();
            
            properties.ToList().ForEach(property =>
            {
                object valor = property.GetValue(filters);
                if (valor is int)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (int)valor));
                else if (valor is string)
                    conditions.Add(new ConditioningStrut(OperatorType.equalString, property.Name, (string)valor));
                else if (valor is double)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (double)valor));
                else if (valor is decimal)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (decimal)valor));
                else if (valor is DateTime)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (DateTime)valor, dbctx));
                else if (valor is Enum)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (int)valor));
                else if (valor is DateBetween)
                    conditions.Add(new ConditioningStrut(OperatorType.equal, property.Name, (DateBetween)valor, dbctx));
                else if (valor is List<int>)
                    conditions.Add(new ConditioningStrut(OperatorType._in, property.Name, EcalcCSV.ListaToCSV((List<int>)valor)));
                else if (valor is List<string>)
                    conditions.Add(new ConditioningStrut(OperatorType._in, property.Name, EcalcCSV.ListaToCSV((List<string>)valor)));
                else
                    throw new Exception(string.Format("Tipo {0} não esperado na função CreateFilter", valor.GetType()));
            });
            return conditions;
        }
    }
}
