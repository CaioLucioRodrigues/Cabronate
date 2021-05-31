using Cabronate.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.Value_Objects
{
    public interface INormalizarDados
    {
        void NormalizarDados(EcalcValueObjectDAO vo, DBContexto dbctx);
    }
}
