using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CStream : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if (obj == null) return null;

            Stream stream = (Stream)obj;
            stream.Position = 0;
            byte[] byteArr = new byte[(int)stream.Length];
            stream.Read(byteArr, 0, (int)stream.Length);
            return byteArr;
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
            {
                MemoryStream ms = new MemoryStream();
                if (value is string)
                {
                    StreamWriter writer = new StreamWriter(ms);
                    writer.Write(value);
                    writer.Flush();
                }
                else
                {
                    byte[] valorByte = (byte[])value;
                    ms.Write(valorByte, 0, valorByte.Length);
                }

                ms.Position = 0;
                property.SetValue(obj, ms, null);
            }
        }
    }
}
