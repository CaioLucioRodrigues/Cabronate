using System;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Atributo que define qual o operation responsável
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OperationAtribute : System.Attribute
    {
        public OperationAtribute(Type operation)
        {
            this.Operation = operation;
        }

        public Type Operation { get; set; }
    }
}
