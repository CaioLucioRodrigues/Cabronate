using Cabronate.Base;
using Cabronate.DAO.Value_Objects;
using System;

namespace Cabronate.DAO.Utils
{
    public static class UtilsDAO
    {
        public static Type GetTypeByClassName(string ClassName)
        {
            Type tipo = Type.GetType(ClassName);
            if (tipo == null)
                throw new Exception(string.Format("Classe {0} não localizada", ClassName));

            // Checar se a classe é descendente de um EcalcValueObjectDAO
            if (!tipo.IsSubclassOf(typeof(EcalcValueObjectDAO)))
                throw new Exception(string.Format("Classe {0} não descente de EcalcValueObjectDAO", ClassName));

            return tipo;
        }

        public static Type GetTypeByClassName(string className, ListaErros erros)
        {
            Type tipo = DAO.Utils.UtilsDAO.GetTypeByClassName(className);
            if (tipo == null)
                erros.AddError(string.Format("Classe {0} não localizada", className), ErrorType.Error);

            // Checar se a classe é descendente de um EcalcValueObjectDAO
            if (!tipo.IsSubclassOf(typeof(EcalcValueObjectDAO)))
                erros.AddError(string.Format("Classe {0} não descente de EcalcValueObjectDAO", className), ErrorType.Error);

            return tipo;
        }
    }
}
