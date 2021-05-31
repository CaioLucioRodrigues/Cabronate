using Cabronate.Base;
using Cabronate.Base.Attributes;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cabronate.DAO.Business_Objects
{
    public enum TipoDeResposta { Lista = 0, Texto = 1, Numerico = 2, Data = 3, DataEHora = 4 };

    [Serializable]
    [TableName("CLIATRB")]
    [Description("Atributos dos Clientes")]
    [Permissao(PermIDGlobal = "Geral.Cadastro.AtributoRespondido")]
    public class AtributoRespondidoBO : EcalcValueObjectDAO
    {
        /// <summary>
        /// Tabela dos atributos dos clientes.
        /// </summary>
        public AtributoRespondidoBO() { }

        [KeyField]
        [FieldName("CODSEQ")]
        public int Id { get; set; }

        /// <summary>
        /// Código do cliente.
        /// <br>SQL: CLIENTES.CCODIGO = CLIATRB.CODCLIENTE</br>
        /// </summary>
        [FieldName("CODCLIENTE")]
        [StringLength(30)]
        public string EntidadeId { get; set; }

        /// <summary>
        /// Código do atributo.
        /// <br>SQL: CADATRIB.CODSEQ = CLIATRB.ATRIBUTO</br>
        /// </summary>
        [FieldName("ATRIBUTO")]
        public int AtributoId { get; set; }

        /// <summary>
        /// Resposta do atributo.
        /// <br>SQL: RESPATRB.CODSEQ = CLIATRB.RESPOSTA</br>
        /// </summary>
        [FieldName("RESPOSTA")]
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.CanNull })]
        public int RespostaId { get; set; }

        /// <summary>
        /// Observações dos atributos do cliente.
        /// </summary>
        [FieldName("OBS")]
        [StringLength(2000)]
        public string Observacao { get; set; }

        /// <summary>
        /// Código origem do atributo. Ver lista atualizada de possíveis códigos na unit uCadAtrib.
        /// </summary>
        [FieldName("ORIGEM")]
        public string Origem { get; set; }

        /// <summary>
        /// Tipo de resposta.
        /// </summary>
        [FieldName("TIPORESP")]
        public TipoDeResposta TipoResposta { get; set; }

        /// <summary>
        /// Resposta, para o caso de pergunta TEXTO.
        /// </summary>
        [FieldName("RESPTEXT")]
        [StringLength(500)]
        public string RespostaTexto { get; set; }

        /// <summary>
        /// Resposta para o caso de perguntas do tipo Numéricas.
        /// </summary>
        [FieldName("RESPNUM")]
        public double RespostaNumero { get; set; }

        /// <summary>
        /// Resposta para o caso de perguntas do tipo Data.
        /// </summary>
        [FieldName("RESPDATA")]
        public DateTime RespostaData { get; set; }

        /// <summary>
        /// Guarda data de inclusão do registro.
        /// </summary>
        [FieldName("DATAINCLUSAO")]
        public DateTime DataInclusaoRegistro { get; set; }

        public static void SetarAtributos(string atributos, string cliente, string origem, DBContexto dbctx, bool editando = false)
        {
            List<AtributoRespondidoBO> listAtributo = new List<AtributoRespondidoBO>();

            string[] arrayAtributos = atributos.Split('£');

            string entidadeId = "";
            if (cliente != "")
                entidadeId = cliente;

            for (int i = 0; i < arrayAtributos.Length; i++)
            {
                AtributoRespondidoBO atributo = new AtributoRespondidoBO();
                string[] _atributo = arrayAtributos[i].Split('§');
                //4 no modo criação de atributos, 6 no modo edição de atributos, pois vem os dados do codseq da cliatrb
                if (_atributo.Length == 4 || _atributo.Length == 5 || _atributo.Length == 6)
                {
                    //se for do tipo lista e não foi tratado no client, ou, passou de alguma forma trata aki
                    _atributo[1] = Convert.ToString(_atributo[1]).IndexOf('_') > -1 ? _atributo[1].Split('_')[0] : _atributo[1];

                    if (editando)
                        atributo = GenericOP.Carregar<AtributoRespondidoBO>(Convert.ToInt32(_atributo[4]), dbctx);

                    //se não for modo edição salva a entidade
                    if (entidadeId != "")
                    {
                        atributo.EntidadeId = entidadeId;
                        atributo.Origem = origem;
                    }

                    atributo.AtributoId = Convert.ToInt32(_atributo[0]);
                    atributo.Observacao = _atributo[3];
                    TipoDeResposta tipoAtributo = (TipoDeResposta)Convert.ToInt32(_atributo[1]);                    
                    atributo.DataInclusaoRegistro = DateTime.Now;
                    switch (tipoAtributo)
                    {
                        case TipoDeResposta.Lista:
                            atributo.RespostaId = Convert.ToInt32(_atributo[2]);
                            break;
                        case TipoDeResposta.Texto:
                            atributo.RespostaTexto = _atributo[2];
                            break;
                        case TipoDeResposta.Numerico:
                            atributo.RespostaNumero = Convert.ToInt32(_atributo[2]);
                            break;
                        case TipoDeResposta.Data:
                            atributo.RespostaData = Convert.ToDateTime(_atributo[2]);
                            break;
                        case TipoDeResposta.DataEHora:
                            atributo.RespostaData = Convert.ToDateTime(_atributo[2]);
                            break;
                    }
                    listAtributo.Add(atributo);
                }
            }
            GenericOP.Salvar(listAtributo, dbctx);
        }

        public static List<AtributoRespondidoBO> CarregaListaPorAtributo(DBContexto dbctx, int CodSeqAtributo)
        {
            List<ConditioningStrut> conditionList = new List<ConditioningStrut>();
            conditionList.Add(new ConditioningStrut(OperatorType.equal, "Atributo", CodSeqAtributo));
            return GenericOP.CarregarLista<AtributoRespondidoBO>(conditionList, dbctx).OrderBy(x => x.Id).ToList();
        }

        public static List<AtributoRespondidoBO> CarregaListaPorMultiplasEntidade(DBContexto dbctx, string Origem, List<string> CodEntidade)
        {
            List<ConditioningStrut> conditionList = new List<ConditioningStrut>();
            conditionList.Add(new ConditioningStrut(OperatorType.equalString, "Origem", Origem));
            conditionList.Add(new ConditioningStrut(OperatorType._in, "CodCliente", EcalcCSV.ListaToCSV(CodEntidade)));
            return GenericOP.CarregarLista<AtributoRespondidoBO>(conditionList, dbctx).OrderBy(x => x.Id).ToList();
        }

        public static List<AtributoRespondidoBO> CarregarPorOrigemEntidadeEAtributo(DBContexto dbctx, string origem, string entidadeId,
                    int atributoId)
        {
            var filtros = new List<ConditioningStrut>()
            {
                new ConditioningStrut(OperatorType.equalString, "Origem", origem),
                new ConditioningStrut(OperatorType.equalString, "CodCliente", entidadeId),
                new ConditioningStrut(OperatorType.equalString, "Atributo", atributoId)
            };
            return GenericOP.CarregarLista<AtributoRespondidoBO>(filtros, dbctx);
        }

        public static List<AtributoRespondidoBO> CarregarPorEntidadeEAtributo(DBContexto dbctx, string entidadeId,
            int atributoId)
        {
            var filtros = new List<ConditioningStrut>()
            {
                new ConditioningStrut(OperatorType.equalString, "CODCLIENTE", entidadeId),
                new ConditioningStrut(OperatorType.equalString, "ATRIBUTO", atributoId)
            };
            return GenericOP.CarregarLista<AtributoRespondidoBO>(filtros, dbctx);
        }

        public static List<AtributoRespondidoBO> CarregaListaPorAtributoId(DBContexto dbctx, int atributoId)
        {
            var filtros = new List<ConditioningStrut>() {
                new ConditioningStrut(OperatorType.equal, "Atributo", atributoId)
            };
            return GenericOP.CarregarLista<AtributoRespondidoBO>(filtros, dbctx);
        }

        public static List<AtributoRespondidoBO> CarregaListaPorEntidade(DBContexto dbctx, string origem, string entidadeId)
        {
            var filtros = new List<ConditioningStrut>()
            {
                new ConditioningStrut(OperatorType.equalString, "Origem", origem),
                new ConditioningStrut(OperatorType.equalString, "CodCliente", entidadeId)
            };
            return GenericOP.CarregarLista<AtributoRespondidoBO>(filtros, dbctx);
        }
    }
}

