using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.SQL
{
    /// <summary>
    /// Classe responsável para validar o cascade
    /// </summary>
    public static class CascadeValidator
    {
        public static ListaErros Validate(DBContexto dbctx, object obj, int ID)
        {
            ListaErros erros = new ListaErros();
            List<ChildCascadeVO> childs = AttributeReaderSingleton.AttributeReader.getPropertiesFKField(obj);

            childs.ForEach(c =>
            {
                string sql = $"SELECT {c.FieldCaption} AS FIELDCAPTION FROM {c.TableTarget} WHERE {c.FieldTarget} = {ID}";
                DataTable dt = dbctx.GetDataTable(sql.ToString());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        erros.AddError($"Registro em uso na entidade {c.Caption} '{dbctx.GetFieldAsString(row["FIELDCAPTION"])}'.");
                    }
                }
            });

            return erros;
        }

        public static void ValidadeFields(DBContexto dbctx, object obj)
        {
            ListaErros erros = new ListaErros();

            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                object[] attributes = property.GetCustomAttributes(typeof(FKAttribute), false);
                if (attributes.Count() > 0)
                    erros.AddErrors(CascadeValidator.ValidadeField(dbctx, property, Convert.ToInt32(property.GetValue(obj, null))));
            }

            if (erros.HasAnyError)
                throw new Exception(erros.ToString());
        }

        private static ListaErros ValidadeField(DBContexto dbctx, object field, int ID)
        {
            ListaErros erros = new ListaErros();
            List<ChildCascadeVO> childs = AttributeReaderSingleton.AttributeReader.getPropertiesFKField(field);

            childs.ForEach(c =>
            {
                string sql = $"SELECT {c.FieldTarget} AS FIELDCAPTION FROM {c.TableTarget} WHERE {c.FieldTarget} = {ID}";
                DataTable dt = dbctx.GetDataTable(sql.ToString());

                if (dt.Rows.Count == 0)
                    erros.AddError($"Nenhum registro localizado com o ID {ID} na entidade {c.TableTarget}.");
            });

            return erros;
        }
    }
}
