using System;

namespace Cabronate.DAO.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class JoinPropertyAttribute : System.Attribute
    {
        public JoinPropertyAttribute(string prop)
        {
            this.Prop = prop;
        }
        public string Prop { get; set; }
    }
}
