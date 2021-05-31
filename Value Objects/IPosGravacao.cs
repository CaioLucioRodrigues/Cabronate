using Cabronate.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.Value_Objects
{
    public interface IPosGravacao
    {
        void PosGravacao(EcalcValueObjectDAO vo, DBContexto dbctx);
    }
}
