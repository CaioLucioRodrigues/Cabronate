using System;
using System.ComponentModel.DataAnnotations;

namespace Cabronate.DAO.Attributes
{
    public class GreaterThanAttribute : ValidationAttribute
    {   
        public GreaterThanAttribute(double startNumber)
        {
            StartNumber = startNumber;
        }

        public double StartNumber { get; set; }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return Convert.ToDouble(value) > StartNumber;
            }
            return false;
        }
    }
}
