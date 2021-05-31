using Cabronate.DAO.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace Cabronate.DAO.Mount
{
    public class MountObjectMapperField
    {
        public PropertyInfo Property { get; set; }

        public string FieldName { get; set; }

        public bool TemDefault { get; set; }

        public int FieldOrdinal { get; set; }

        public FieldTypeDetail[] types { get; set; }

        public ValidationAttribute[] attributesValidation { get; set; }

        public object[] attributesKeyField { get; set; }

        public IDbDataParameter paramIns { get; set; }

        public IDbDataParameter paramUpd { get; set; }
    }
}
