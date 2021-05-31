using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.SQL;
using System;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureFilter
    {
        private readonly DBContexto _dbctx;

        public StructureFilter(DBContexto dbctx, OperatorType operatorType, JsonFieldStructure field, dynamic value, bool _fixed) 
        {
            Operator = operatorType;
            Field = field;
            Value = value;
            Fixed = _fixed;
            Dbctx = dbctx;
        }

        public OperatorType Operator { get; set; }

        public JsonFieldStructure Field { get; set; }

        public object Value { get; set; }

        public bool Fixed { get; set; }

        public DBContexto Dbctx;

        public ConditioningStrut Condition
        {
            get
            {
                ConditioningStrut condition;
                if (Value is int)
                    condition = new ConditioningStrut(Operator, $"{Field.TableName}.{Field.FieldName}", (int)Value);
                else if (Value is double)
                    condition = new ConditioningStrut(Operator, $"{Field.TableName}.{Field.FieldName}", (double)Value);
                else if (Value is decimal) 
                    condition = new ConditioningStrut(Operator, $"{Field.TableName}.{Field.FieldName}", (decimal)Value);                
                else if (Value is DateTime)
                    condition = new ConditioningStrut(Operator, $"{Field.TableName}.{Field.FieldName}", (DateTime)Value, _dbctx);
                else
                    condition = new ConditioningStrut(Operator, $"{Field.TableName}.{Field.FieldName}", Value?.ToString());

                condition.dbctx = Dbctx;
                return condition;
            }
        }         
    }
}
