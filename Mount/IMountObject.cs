using Cabronate.Base;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.Mount
{
    /// <summary>
    /// Interface que define o contrato das classe que vão carregar objetos a partir de consultas ao banco
    /// </summary>
    public interface IMountObject<T>
    {
        T mountObject(IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null);

        List<T> mountObjects(IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null);

        bool hasEmptyConstructor(Type type);
    }
}
