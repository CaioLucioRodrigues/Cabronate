using Cabronate.Base;

namespace Cabronate.DAO.Value_Objects
{
    public interface IValidarPersistencia
    {
        bool ValidarPersistencia(EcalcValueObjectDAO vo, DBContexto dbctx);
    }
}
