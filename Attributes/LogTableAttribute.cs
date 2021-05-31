using Cabronate.DAO.Business_Objects;
using System;

namespace Cabronate.DAO.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LogTableAttribute : Attribute
    {
        public LogTableAttribute(TipoLogTabela type)
        {
            Type = type;
        }

        public TipoLogTabela Type { get; set; }
    }
}
