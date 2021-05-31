using Cabronate.DAO.ReportBuilder.Types;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;

namespace Cabronate.DAO.ReportBuilder.StructureJsons
{
    [Serializable]
    public class JsonFieldStructure
    {
        public JsonFieldStructure() 
        {
            Operation = Operation.Undefined;
            InMountingFormula = false;
            CanBeLookUp = false;
            FieldsForFormula = new List<JsonFieldStructure>();
            Values = new List<JsonValuesStructure>();
        }

        public string FieldName { get; set; }

        public string FieldFriendlyName { get; set; }

        public string TableName { get; set; }

        public string TableFriendlyName { get; set; }
        
        public int Order { get; set; }

        public bool CanBeLookUp { get; set; }

        public DataType TypeData { get; set; }

        public Operation Operation { get; set; }

        public List<JsonValuesStructure> Values { get; set; }

        #region campos para fórmula
        public List<JsonFieldStructure> FieldsForFormula { get; set; }

        public MathematicalSign Sign { get; set; }

        public bool InMountingFormula { get; set; }                
        #endregion

        public override string ToString()
        {
            return (!string.IsNullOrWhiteSpace(FieldFriendlyName) ? FieldFriendlyName : FieldName);
        }

        public static List<OperatorType> GetOperatorsByType(int type)
        {
            DataType dataType = (DataType)type;
            List<OperatorType> operators = null;

            switch (dataType)
            {
                case DataType.Date:
                    operators = new List<OperatorType>()
                        { OperatorType.equal, OperatorType.different, OperatorType.greater, OperatorType.greaterequal,
                          OperatorType.less, OperatorType.lessequal, OperatorType.dateBetween, OperatorType.daysAgos,
                          OperatorType.thisMonth, OperatorType.thisWeek, OperatorType.lateDate, OperatorType.today };
                    break;
                case DataType.Integer:
                case DataType.Double:
                    operators = new List<OperatorType>()
                        { OperatorType.equal, OperatorType.different, OperatorType.greater, OperatorType.greaterequal,
                          OperatorType.less, OperatorType.lessequal, OperatorType._in, OperatorType.notin };
                    break;
                case DataType.String:
                    operators = new List<OperatorType>()
                        { OperatorType.equalString, OperatorType.differentString, OperatorType.greaterString, OperatorType.greaterequalString,
                          OperatorType.lessequalString, OperatorType.like, OperatorType._in, OperatorType.notin };
                    break;
                default:
                    operators = new List<OperatorType>();
                    break;
            }

            return operators;
        }
    }
}
