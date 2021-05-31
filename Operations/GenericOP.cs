using Cabronate.Base;
using Cabronate.DAO.SQL;
using Cabronate.DAO.UniversalDAO;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;


namespace Cabronate.DAO.Operations
{
    public abstract class GenericOP
    {
        /// <summary>
        /// Se alguem adicionar um parâmetro a mais, reveja o método SalvarTipo por conta do reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vo"></param>
        /// <param name="dbctx"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public static ListaErros Salvar<T>(T vo, DBContexto dbctx = null, bool lazy = false)
             where T : EcalcValueObjectDAO, new()
        {
            return GenericOP.Salvar(new List<T> { vo }, dbctx, lazy);
        }

        public static ListaErros Salvar<T>(List<T> vos, DBContexto dbctx = null, bool lazy = false)
            where T : EcalcValueObjectDAO, new()
        {
            return Salvar<T>(vos, null, dbctx, lazy);
        }

        public static ListaErros Salvar<T>(List<T> vos, SQLControlFields fields, DBContexto dbctx = null, bool lazy = false)
            where T : EcalcValueObjectDAO, new()
        {
            ListaErros listaErros = new ListaErros();
            if (vos.Count > 0)
            {
                var salvarMesmoComErros = !vos.Where(v => v.PersistWithErrors == false)
                                              .Any();

                UniversalDAO<T> DAO = new UniversalDAO<T>(dbctx, lazy);
                GenericOP op = vos.First().GetOP();
                foreach (T item in vos)
                {
                    InternalNormalizarDados(op, item, dbctx);
                    InternalValidarPersistencia(op, item, dbctx);
                    listaErros.Errors.AddRange(item.Erros.Errors);
                }
                if (!listaErros.HasAnyError || salvarMesmoComErros)
                {
                    DAO.SaveObjectsWithControlFields(vos, fields);
                    foreach (T item in vos)
                        InternalPosGravacao(op, item, dbctx);
                }
            }
            return listaErros;
        }

        public static ListaErros TrySalvar<T>(List<T> vos, SQLControlFields fields, DBContexto dbctx = null)
               where T : EcalcValueObjectDAO, new()
        {
            var listaErros = new ListaErros();
            if (vos.Count > 0)
            {
                var DAO = new UniversalDAO<T>(dbctx);
                GenericOP op = vos.First().GetOP();
                foreach (T item in vos)
                {
                    InternalNormalizarDados(op, item, dbctx);
                    InternalValidarPersistencia(op, item, dbctx);
                    listaErros.Errors.AddRange(item.Erros.Errors);
                }
                if (!listaErros.HasAnyError)
                {
                    DAO.SaveObjectsWithControlFields(vos, fields);
                    foreach (T item in vos)
                        InternalPosGravacao(op, item, dbctx);
                }
            }

            return listaErros;
        }

        public static ListaErros Salvar<T>(T vo, List<string> fields, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            GenericOP op = vo.GetOP();

            InternalNormalizarDados(op, vo, dbctx);
            if (InternalValidarPersistencia(op, vo, dbctx))
            {
                DAO.SaveObject(vo, fields);
                InternalPosGravacao(op, vo, dbctx);
            }
            return vo.Erros;
        }

        public static ListaErros Salvar<T>(T vo, SQLControlFields fields, DBContexto dbctx = null, bool lazy = false)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy);
            GenericOP op = vo.GetOP();

            InternalNormalizarDados(op, vo, dbctx);
            if (InternalValidarPersistencia(op, vo, dbctx))
            {
                DAO.SaveObjectWithControlField(vo, fields);
                InternalPosGravacao(op, vo, dbctx);
            }
            return vo.Erros;
        }

        private static void InternalNormalizarDados<T>(GenericOP op, T vo, DBContexto dbctx) where T : EcalcValueObjectDAO, new()
        {
            if (vo is INormalizarDados)
                ((INormalizarDados)vo).NormalizarDados(vo, dbctx);
            else if (op != null)
                op.NormalizarDados(vo, dbctx);
        }

        private static void InternalPosGravacao<T>(GenericOP op, T vo, DBContexto dbctx) where T : EcalcValueObjectDAO, new()
        {
            if (vo is IPosGravacao)
                ((IPosGravacao)vo).PosGravacao(vo, dbctx);
            else if (op != null)
                op.PosGravacao(vo, dbctx);
        }
        
        public static ListaErros SalvarTipo(Type tipo, EcalcValueObjectDAO obj, DBContexto dbctx)
        {
            Type genericClassType = typeof(GenericOP);
            MethodInfo methodInfo = genericClassType.GetMethods()
                .Where(x => x.Name == "Salvar")
                .Select(x => new { M = x, P = x.GetParameters() })
                .Where(x => x.P.Length == 3
                    && x.P[0].ParameterType.IsGenericParameter
                             && x.P[0].ParameterType.BaseType == typeof(EcalcValueObjectDAO)
                             && x.P[1].ParameterType == typeof(DBContexto)
                             && x.P[2].ParameterType == typeof(bool))
                .Select(x => x.M)
                .SingleOrDefault();

            Type[] genericArguments = new Type[] { tipo };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            object returnValue = genericMethodInfo.Invoke(null, new object[] { obj, dbctx, false });
            return (ListaErros)returnValue;
        }

        public static ListaErros ValidarVO<T>(T vo, DBContexto dbctx)
            where T : EcalcValueObjectDAO, new()
        {
            var listaErros = new ListaErros();
            GenericOP op = vo.GetOP();

            InternalNormalizarDados(op, vo, dbctx);
            InternalValidarPersistencia(op, vo, dbctx);
            listaErros.Errors.AddRange(vo.Erros.Errors);
            return listaErros;
        }

        public static T Carregar<T>(int key, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            return GenericOP.Carregar<T>(false, key, dbctx);
        }

        public static T Carregar<T>(int key, DBContexto dbctx, bool hasCache, bool lazy)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy, hasCache);
            T vo = DAO.GetObject(key);
            return vo;
        }

        public static T Carregar<T>(bool lazy, int key, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy);
            T vo = DAO.GetObject(key);
            return vo;
        }

        public static T Carregar<T>(List<ConditioningStrut> conditionList, DBContexto dbctx = null,
            bool lazy = false, bool hasCache = true)
            where T : EcalcValueObjectDAO, new()
        {
            return GenericOP.Carregar<T>(lazy, conditionList, dbctx, hasCache);
        }

        public static T Carregar<T>(bool lazy, List<ConditioningStrut> conditionList, DBContexto dbctx = null, bool hasCache = true)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy, hasCache);
            T vo = DAO.GetObject(conditionList);
            return vo;
        }

        public static EcalcValueObjectDAO CarregarTipo(Type tipo, int key, DBContexto dbctx, bool hasCache = false, bool lazy = false)
        {
            var parametros = new Type[] { typeof(int), typeof(DBContexto), typeof(bool), typeof(bool) };
            Type genericClassType = typeof(GenericOP);
            MethodInfo methodInfo = genericClassType.GetMethod("Carregar", parametros);

            var genericArguments = new Type[] { tipo };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            object returnValue = genericMethodInfo.Invoke(null, new object[] { key, dbctx, hasCache, lazy });

            return (EcalcValueObjectDAO)returnValue;
        }

        public static ListWithError<T> CarregarLista<T>(DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            UniversalDAO<T> DAO = new UniversalDAO<T>(dbctx);
            return ListWithErrorConverter<T>.ListObject2ListWithError(DAO.GetObjects());
        }

        public static ListWithError<T> CarregarLista<T>(bool lazy, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy);
            return ListWithErrorConverter<T>.ListObject2ListWithError(DAO.GetObjects());
        }

        public static ListWithError<T> CarregarLista<T>(List<ConditioningStrut> conditionList, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            return ListWithErrorConverter<T>.ListObject2ListWithError(DAO.GetObjects(conditionList));
        }

        public static ListWithError<T> CarregarLista<T>(bool lazy, List<ConditioningStrut> conditionList, DBContexto dbctx = null,
            bool hasCache = true)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx, lazy, hasCache);
            return ListWithErrorConverter<T>.ListObject2ListWithError(DAO.GetObjects(conditionList));
        }

        public static List<T> CarregarLista<T>(DBContexto dbctx, string script, List<ParameterData> parameters)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            return ListWithErrorConverter<T>.ListObject2ListWithError(DAO.GetObjects(script, parameters));
        }

        public static ListaErros Deletar<T>(T vo, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            return Deletar<T>(new List<T> { vo }, dbctx);
        }

        /// <summary>
        /// Antes de adicionar quaquer parâmetro nesse método verifique o DeletarTipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vos"></param>
        /// <param name="dbctx"></param>
        /// <returns></returns>
        public static ListaErros Deletar<T>(List<T> vos, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var listaErros = new ListaErros();
            if (vos.Count > 0)
            {
                var DAO = new UniversalDAO<T>(dbctx);
                GenericOP op = vos.First().GetOP();                
                foreach (T item in vos)
                {
                    if (!GenericOP.InternalValidarDeleta(op, item, dbctx))
                        listaErros.Errors.AddRange(item.Erros.Errors);
                }
                if (!listaErros.HasAnyError)
                    DAO.DeleteObjects(vos);
            }
            return listaErros;
        }

        public static ListaErros DeletarTipo(Type tipo, EcalcValueObjectDAO obj, DBContexto dbctx)
        {
            Type genericClassType = typeof(GenericOP);
            MethodInfo methodInfo = genericClassType.GetMethods()
                .Where(x => x.Name == "Deletar")
                .Select(x => new { M = x, P = x.GetParameters() })
                .Where(x => x.P.Length == 2
                    && x.P[0].ParameterType.IsGenericParameter
                             && x.P[0].ParameterType.BaseType == typeof(EcalcValueObjectDAO)
                             && x.P[1].ParameterType == typeof(DBContexto))
                .Select(x => x.M)
                .SingleOrDefault();

            Type[] genericArguments = new Type[] { tipo };
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            object returnValue = genericMethodInfo.Invoke(null, new object[] { obj, dbctx });

            return (ListaErros)returnValue;
        }

        public static int Count<T>(DBContexto dbctx, List<ConditioningStrut> conditions)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            return DAO.Count(conditions);
        }

        public static int Max<T>(DBContexto dbctx, List<ConditioningStrut> conditions, string nomeColuna)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            return DAO.Max(conditions, nomeColuna);
        }

        public static string GetFieldValue<T>(DBContexto dbctx, int id, string field)
                    where T : EcalcValueObjectDAO, new()
        {
            return new UniversalDAO<T>(dbctx).GetFieldValueCommand(id, field);
        }

        public static string GetFieldValue<T>(DBContexto dbctx, List<ConditioningStrut> conditions, string field)
                    where T : EcalcValueObjectDAO, new()
        {
            return new UniversalDAO<T>(dbctx).GetFieldValueCommand(conditions, field);
        }

        public static int GetFieldValueInt<T>(DBContexto dbctx, int id, string field)
                    where T : EcalcValueObjectDAO, new()
        {
            int number;
            bool isParsable = Int32.TryParse(GetFieldValue<T>(dbctx, id, field), out number);
            return isParsable ? number : 0;
        }

        public static decimal GetFieldValueDecimal<T>(DBContexto dbctx, int id, string field)
                    where T : EcalcValueObjectDAO, new()
        {
            decimal number;
            bool isParsable = decimal.TryParse(GetFieldValue<T>(dbctx, id, field), out number);
            return isParsable ? number : 0m;
        }

        public static decimal GetFieldValueDecimal<T>(DBContexto dbctx, List<ConditioningStrut> conditions, string field)
                    where T : EcalcValueObjectDAO, new()
        {
            decimal number;
            bool isParsable = decimal.TryParse(GetFieldValue<T>(dbctx, conditions, field), out number);
            return isParsable ? number : 0m;
        }

        private static bool InternalValidarPersistencia(GenericOP op, EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
            vo.Erros.Errors.Clear();
            bool persistenciaValida = true;
            if (vo is IValidarPersistencia)
                persistenciaValida = ((IValidarPersistencia)vo).ValidarPersistencia(vo, dbctx);
            else if (op != null)
                persistenciaValida = op.ValidarPersistencia(vo, dbctx);

            bool propriedadesValidas = ValidarPropriedades(vo);
            return (persistenciaValida && propriedadesValidas);
        }

        private static bool InternalValidarDeleta(GenericOP op, EcalcValueObjectDAO vo, DBContexto dbctx)
        {
            vo.Erros.Errors.Clear();
            if (vo is IValidarDeleta)
                return ((IValidarDeleta)vo).ValidarDeleta(vo, dbctx);
            else if (op != null)
                return op.ValidarDeleta(vo, dbctx);
            return true;
        }

        public static bool ValidarPropriedades(EcalcValueObjectDAO vo)
        {
            var ValidationResult = new List<ValidationResult>();
            var ValidationContext = new ValidationContext(vo, null, null);

            Validator.TryValidateObject(vo, ValidationContext, ValidationResult, true);

            foreach (ValidationResult e in ValidationResult)
            {
                vo.Erros.AddError(e.MemberNames.FirstOrDefault(), e.ErrorMessage, ErrorType.Error);
            }

            return (!vo.Erros.HasAnyError);
        }

        public static ListaErros Lock<T>(T vo, DBContexto dbctx = null)
            where T : EcalcValueObjectDAO, new()
        {
            var DAO = new UniversalDAO<T>(dbctx);
            DAO.Lock(vo);
            return vo.Erros;
        }

        #region métodos virtuais
        public virtual void NormalizarDados(EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
        }

        public virtual void PosGravacao(EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
        }

        public virtual bool ValidarPersistencia(EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
            return (!vo.Erros.HasAnyError);
        }

        public virtual bool ValidarDeleta(EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
            return (!vo.Erros.HasAnyError);
        }
        #endregion
    }
}
