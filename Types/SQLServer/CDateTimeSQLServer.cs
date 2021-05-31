using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CDateTimeSQLServer : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if ((type.Contains(FieldTypeDetail.CanNull)) && ((obj == null) || ((DateTime)obj == DateTime.MinValue)))
                return DBNull.Value;
            else if ((obj == null) || ((DateTime)obj <= DateTime.MinValue))
                return ((DateTime)System.Data.SqlTypes.SqlDateTime.MinValue).AddDays(1);
            else
                return obj;
        }

        public void setValue(PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value)
            {
                object[] defaultValue = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultValue.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultValue[0]).defaultValue, null);
            }
            else if ((property.PropertyType.ToString() == "System.Nullable`1[System.DateTime]")
                && (((DateTime)value == DateTime.MinValue) || (((DateTime)value).ToString("MM/yyyy") == "01/1753") || (((DateTime)value).ToString("MM/yyyy") == "12/1899")))
                property.SetValue(obj, null, null);
            else if (((DateTime)value == DateTime.MinValue) || (((DateTime)value).ToString("MM/yyyy") == "01/1753") || (((DateTime)value).ToString("MM/yyyy") == "12/1899"))
                property.SetValue(obj, DateTime.MinValue, null); //SQL Server min DateTime 01/01/1753 will return 01/01/0001 to the C# property
            else
                property.SetValue(obj, value, null);
        }
    }
}
