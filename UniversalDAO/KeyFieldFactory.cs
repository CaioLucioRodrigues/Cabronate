using Cabronate.Base;
using Cabronate.DAO.Attributes;
using System.Collections.Generic;

namespace Cabronate.DAO.UniversalDAO
{
    public static class KeyFieldFactory<T> where T : EcalcValueObject
    {
        public static IKeyFieldEvaluator<T> getKeyFieldClass(T o)
        {
            List<string> properties = AttributeReaderSingleton.AttributeReader.getPropertiesKeyField(o);

            switch (properties.Count)
            {
                case 1:
                    return new UniqueKey<T>(o);
                // Todo MultipleKeys
                default:
                    return new UniqueKey<T>(o);
            }
        }
    }
}
