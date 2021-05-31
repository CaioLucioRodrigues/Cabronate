using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define se a propriedade é um objeto filho 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ChildAttibute : System.Attribute
    {
    }
}
