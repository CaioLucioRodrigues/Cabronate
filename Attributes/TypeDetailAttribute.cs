using System;

namespace Cabronate.DAO.Attributes
{
    public enum FieldTypeDetail { BoolInt, BoolChar, CanNull, None, IntDouble, DecimalDouble, NotNull, ENumString };

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class TypeDetailAttribute : System.Attribute
    {
        public TypeDetailAttribute(FieldTypeDetail[] type)
        {
            this.type = type;
        }
        public FieldTypeDetail[] type { get; set; }
    }
}
