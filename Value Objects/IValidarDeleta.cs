using Cabronate.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.Value_Objects
{
    public interface IValidarDeleta
    {
        bool ValidarDeleta(EcalcValueObjectDAO vo, DBContexto dbctx);
    }
}
