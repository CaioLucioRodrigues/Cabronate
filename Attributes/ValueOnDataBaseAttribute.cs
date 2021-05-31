using System;

namespace Cabronate.DAO.Attributes
{

    /// <summary>
    /// Atributo que define o valor de persistência de um item do ENUM no banco de dados
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueOnDataBaseAttribute : System.Attribute
    {
        public ValueOnDataBaseAttribute(string description)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }

}
