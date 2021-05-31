using Cabronate.DAO.Business_Objects;
using System;

namespace Cabronate.DAO.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OriginAttribute : Attribute
    {
        public OriginAttribute(string origin)
        {
            Origin = origin;
        }

        public string Origin { get; set; }
    }
}
