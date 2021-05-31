using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define a chave primaria de uma classe no banco
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class KeyFieldAttribute : System.Attribute
    {
    }
}
