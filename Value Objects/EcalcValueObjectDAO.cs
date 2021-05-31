using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Business_Objects;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Value_Objects
{
    [Serializable]
    public abstract class EcalcValueObjectDAO : EcalcValueObject
    {
        public EcalcValueObjectDAO()
        {
            PersistWithErrors = false;
            SetToExclude = false;
            Logs = new List<BaseLogBO>();
            LogContainer = new BaseLogContainer(this);
            AttributesContainer = new AttributeContainer(this);            
        }

        [JsonIgnore]
        public int KeyFieldValue
        {
            get 
            {
                object[] objArray = new object[0];
                PropertyInfo property = (PropertyInfo)this.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(this));
                if (property.CanRead)
                    return Convert.ToInt32(property.GetValue(this, objArray));
                else
                    throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_PROPERTY, this.GetType().ToString()), DateTime.Now);
            }
        }

        [JsonIgnore]
        public List<BaseLogBO> Logs { get; }

        public bool IdGreaterThanZero
        {
            get
            {
                return this.KeyFieldValue > 0;
            }            
        }

        public bool New
        {
            get
            {
                return this.KeyFieldValue == 0;
            }
        }

        public bool PersistWithErrors { get; set; }

        /// <summary>
        /// Utilizado para marcar se o objeto é um filho que deve ser excluido no SaveChildren de seu pai
        /// </summary>
        public bool SetToExclude { get; set; }

        public BaseLogContainer LogContainer { get; }

        public AttributeContainer AttributesContainer { get; }

        public virtual GenericOP GetOP()
        {
            // A OP é opcional. Se estiver NULL, usaremos as interfaces IValidarDeleta, IValidarPersistencia, IPosGravao e INormalizarDados diretamento no VO
            return null;
        }

        public virtual List<AcaoGenerica> GetAcoes()
        {
            return new List<AcaoGenerica>();
        }

        /// <summary>
        /// Método que retorna um item do ENUM(com atributo "ValueOnDataBase") conforme Valor(String).
        /// Caso ocorra falha na conversão, retorna o index "0" do ENUM
        /// </summary>
        /// <typeparam name="T">Definição do ENUM</typeparam>
        /// <param name="value">Valor do campo no Banco de Dados (String) que deverá ser convertido para valor ENUM</param>
        public T ValueOnDataBaseToEnum<T>(object value)
            where T : Enum
        {
            //fields com OrderBy pra garantir que teremos a mesma ordem declarada no ENUM (pra poder pegar o index "0" como default)
            var fields = typeof(T).GetFields().OrderBy(f => f.MetadataToken);
            var enumIndex0 = fields.ToArray()[1]; //index "0" do ENUM é a posição "1" do Reflection (pois no "0" sempre vem "Int32 value__")

            foreach (var fi in fields)
            {
                //com o field carregado, localizar o atributo "ValueOnDataBase"
                ValueOnDataBaseAttribute[] attributes =
                      (ValueOnDataBaseAttribute[])fi.GetCustomAttributes(
                      typeof(ValueOnDataBaseAttribute), false);

                if ((attributes.Length > 0) && (attributes[0].Description == Convert.ToString(value)))
                {
                    var enumParse = Enum.Parse(typeof(T), fi.Name);
                    return (T)enumParse;
                }
            }

            //se não localizar, retorna o index0
            return (T)Enum.Parse(typeof(T), enumIndex0.Name); ;
        }

        /// <summary>
        /// Método que retorna um valor(String) do atributo "ValueOnDataBase" conforme item do ENUM
        /// Caso ocorra falha na conversão, retorna o atributo do index "0" do ENUM
        /// </summary>
        /// <typeparam name="T">Definição do ENUM</typeparam>
        /// <param name="valueEnum">Valor ENUM que deverá ser convertido para valor do campo usado no Banco de Dados</param>
        public string EnumToValueOnDataBase<T>(T valueEnum)
            where T : Enum
        {
            //fields com OrderBy pra garantir que teremos a mesma ordem declarada no ENUM (pra poder pegar o index "0" como default)
            var fields = typeof(T).GetFields().OrderBy(f => f.MetadataToken);
            var fi = fields.ToArray()[1]; //index "0" do ENUM é a posição "1" do Reflection (pois no "0" sempre vem "Int32 value__")

            //caso o item "valueEnum" realmente existir, ele será utilizado no field
            if ((valueEnum != null) && (Enum.IsDefined(typeof(T), valueEnum)))
                fi = typeof(T).GetField(valueEnum.ToString());

            //com o field carregado, localizar o atributo "ValueOnDataBase"
            ValueOnDataBaseAttribute[] attributes =
                  (ValueOnDataBaseAttribute[])fi.GetCustomAttributes(
                  typeof(ValueOnDataBaseAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.ATTRIB_VALUEDB_MISS, valueEnum, typeof(T)), DateTime.Now); //esta Excessão sempre deve ser exibida caso o ENUM atual não tenha o atributo preenchido
        }

        public void SetKeyFieldValue(object value)
        {
            //object[] objArray = new object[0];
            PropertyInfo property = (PropertyInfo)this.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(this));
            if (property.CanWrite)
                property.SetValue(this, (int) value);
            else
                throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_PROPERTY, this.GetType().ToString()), DateTime.Now);
        }

        #region Homelend
        public virtual void SetarValor(DBContexto dbctx, ref EcalcValueObjectDAO obj, string titulo, object valor)
        {
            // Poderá ser implementado nos filhos que tivermos importação de dados
        }

        public virtual void ProcessamentoImportacao<T>(DBContexto dbctx, List<T> itensAImportar)
        {
            // Poderá ser implementado nos filhos que tivermos importação de dados
        }

        public virtual void PosProcessamentoImportacao<T>(DBContexto dbctx, List<T> itensAImportar)
        {
            // Poderá ser implementado nos filhos que tivermos importação de dados
        }
        #endregion
    }
}
