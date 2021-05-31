using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define se o campo é uma chave primária
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FKAttribute : System.Attribute
    {
        public FKAttribute(string tableTarget, string fieldTarget, string fieldCaption, string Caption)
        {
            this.TableTarget = tableTarget;
            this.FieldTarget = fieldTarget;
            this.FieldCaption = fieldCaption;
            this.Caption = Caption;
        }

        public string TableTarget { get; set; }

        public string FieldTarget { get; set; }

        public string FieldCaption { get; set; }

        public string Caption { get; set; }
    }
}
