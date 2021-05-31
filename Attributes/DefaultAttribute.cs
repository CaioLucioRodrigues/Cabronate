using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define o default do campo quando o mesmo for null no banco
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DefaultAttribute : System.Attribute
    {
        public DefaultAttribute(object defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        public object defaultValue { get; set; }
    }
}
