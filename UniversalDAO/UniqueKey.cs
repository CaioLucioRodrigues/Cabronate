using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Errors;
using System;
using System.Reflection;

namespace Cabronate.DAO.UniversalDAO
{
    public class UniqueKey<T> : IKeyFieldEvaluator<T>
        where T : EcalcValueObject
    {
        private T obj;

        public UniqueKey(T obj)
        {
            this.obj = obj;
        }

        /// <summary>
        /// Retorna a chave do objeto passado
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>int conteúdo da propriedade com a tag [KeyField]</returns>
        public int getKeyFieldValue()
        {
            object[] objArray = new object[0];
            PropertyInfo property = (PropertyInfo)this.obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(this.obj));
            if (property.CanRead)
                return Convert.ToInt32(property.GetValue(this.obj, objArray));
            else
                throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_PROPERTY, this.obj.GetType().ToString()),
                    DateTime.Now);
        }

        /// <summary>
        /// Seta a chave no objeto passado
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">valor da chave</param>
        public void setKeyFieldValue(int value)
        {
            object[] objArray = new object[0];
            PropertyInfo property = (PropertyInfo)this.obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(this.obj));
            if (property.CanRead)
                property.SetValue(this.obj, value, objArray);
            else
                throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_PROPERTY, this.obj.GetType().ToString()),
                    DateTime.Now);
        }
    }
}
