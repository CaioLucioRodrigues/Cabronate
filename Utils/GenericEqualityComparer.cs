using Cabronate.DAO.Attributes;
using System;
using System.Collections.Generic;

namespace Cabronate.DAO.Utils
{
    /// <summary>
    /// Classe criada para executar distincts de listas genéricas que usem o Cabronate.DAO
    /// </summary>
    /// <typeparam name="T">Classe a ser comparada</typeparam>
    public sealed class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
            return
                Convert.ToInt32(x.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getKeyField(x)).GetValue(x, new object[0])) ==
                Convert.ToInt32(y.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getKeyField(y)).GetValue(y, new object[0]));
        }

        public int GetHashCode(T x)
        {
            if (Object.ReferenceEquals(x, null)) return 0;
            return Convert.ToInt32(x.GetType().GetProperty(Cabronate.DAO.Attributes.AttributeReaderSingleton.AttributeReader.getKeyField(x)).GetValue(x, new object[0])).GetHashCode();
        }
    }
}