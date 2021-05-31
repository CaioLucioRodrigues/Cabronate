using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CString : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            //
            if ((obj == null) && (type.Contains(FieldTypeDetail.CanNull)))
            {
                return DBNull.Value;
            }
            else if ((obj == null) || (((string)obj).Trim() == ""))
            {
                return string.Empty;
            }
            else
            {
                int maxLength = getStringLength(validationAttrib);
                if (maxLength > 0)
                    return ((string)obj).Substring(0, Math.Min(maxLength, ((string)obj).Length));
                else
                    return obj;
            }
        }

        public void setValue(PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value)
            {
                object[] defaultValue = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultValue.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultValue[0]).defaultValue, null);
            }
            else
                property.SetValue(obj, Convert.ToString(value), null);
        }

        private int getStringLength(ValidationAttribute[] validationAttrib)
        {
            try
            {
                ValidationAttribute[] listAtrib = validationAttrib.Where(v => v.GetType() == typeof(StringLengthAttribute)).ToArray();
                if (listAtrib.Count() > 0)
                    return ((StringLengthAttribute)listAtrib[0]).MaximumLength;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
