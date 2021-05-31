using Cabronate.Base;

namespace Cabronate.DAO.UniversalDAO
{
    public interface IKeyFieldEvaluator<T>
        where T : EcalcValueObject
    {
        int getKeyFieldValue();

        void setKeyFieldValue(int value);
    }
}
