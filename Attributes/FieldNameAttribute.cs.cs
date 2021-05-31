using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define o nome do campo no banco referente a uma propriedade de uma classe
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FieldNameAttribute : System.Attribute
    {
        public FieldNameAttribute(string description)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }
}
