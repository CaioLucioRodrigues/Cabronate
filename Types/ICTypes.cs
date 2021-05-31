using Cabronate.DAO.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public interface ICTypecs
    {
        /// <summary>
        /// Pegando o valor para persistência 
        /// </summary>                
        object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib);

        /// <summary>
        /// Pegando o valor para setar na propriedade
        /// </summary>        
        void setValue(PropertyInfo property, object obj, object value);
    }
}
