using Cabronate.Base;
using Cabronate.Base.Attributes;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Utils;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cabronate.DAO.Business_Objects
{
    [Serializable]
    [TableName("RESPATRB")]
    [Description("Resposta de Atributos")]
    [Permissao(PermIDGlobal = "Geral.Cadastro.AtributoResposta")]
    public class AtributoRespostaBO : EcalcValueObjectDAO
    {
        /// <summary>
        /// Tabela das respostas de atributos (hoje usado para clientes e prospects).
        /// </summary>
        public AtributoRespostaBO() { }

        [KeyField]
        [FieldName("CODSEQ")]
        public int Id { get; set; }

        /// <summary>
        /// Código do Atributo do Cadastro de Cliente.
        /// </summary>
        [FieldName("CODIGOATRIB")]
        public int AtributoId { get; set; }

        /// <summary>
        /// Descrição da Resposta do Atributo do Cadastro de Cliente.
        /// </summary>
        [FieldName("RESPOSTA")]
        [StringLength(150)]
        public string Resposta { get; set; }

        /// <summary>
        /// Indica se a resposta do atributo é vista como crítica.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("CRITICO")]
        public bool Critico { get; set; }

        /// <summary>
        /// Código para exportação (integração com outros sistemas).
        /// </summary>
        [FieldName("CODEXPO")]
        [StringLength(30)]
        public string CodExpo { get; set; }

        /// <summary>
        /// Cor do atributo.
        /// </summary>
        [FieldName("COR")]
        public int Cor { get; set; }

        /// <summary>
        /// <br>Campo SELECTED da tabela RESPATRB, Integer.</br>
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("SELECTED")]
        public bool Selected { get; set; }

        public static List<AtributoRespostaBO> CarregarRespostasPorAtributoId(DBContexto dbctx, int atributoId)
        {
            var filtrost = new List<ConditioningStrut>(){
                new ConditioningStrut(OperatorType.equal, "CodigoAtrib", atributoId)
            };
            return GenericOP.CarregarLista<AtributoRespostaBO>(filtrost, dbctx);
        }

        public static List<AtributoRespostaBO> CarregarListaAtributo(DBContexto dbctx, int CodSeqAtributo)
        {
            List<ConditioningStrut> conditionList = new List<ConditioningStrut>();
            conditionList.Add(new ConditioningStrut(OperatorType.equal, "CodigoAtrib", CodSeqAtributo));
            return GenericOP.CarregarLista<AtributoRespostaBO>(conditionList, dbctx).OrderBy(x => x.Resposta).ToList();
        }

        public static List<AtributoRespostaBO> CarregarListaAtributo(DBContexto dbctx, List<int> listIdAtribRespondido)
        {
            List<ConditioningStrut> conditionList = new List<ConditioningStrut>();
            conditionList.Add(new ConditioningStrut(OperatorType._in, "CodigoAtrib", string.Join(",", listIdAtribRespondido)));
            return GenericOP.CarregarLista<AtributoRespostaBO>(conditionList, dbctx).OrderBy(x => x.Resposta).ToList();
        }
    }
}

