using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define o nome da tabela no banco referente a uma classe
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }
}
