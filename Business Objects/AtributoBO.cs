using Cabronate.Base;
using Cabronate.Base.Attributes;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.UniversalDAO;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;

namespace Cabronate.DAO.Business_Objects
{
    [Serializable]
    [TableName("CADATRIB")]
    [Description("Atributos")]
    [Permissao(PermIDGlobal = "Geral.Cadastro.Atributo")]
    public class AtributoBO : EcalcValueObjectDAO, IDeleteChildren, IValidarDeleta
    {
        public AtributoBO() { }

        public enum RespostaAtributoEnum
        {
            [Display(Name = "Lista Fixa")]
            ListaFixa,

            [Display(Name = "Texto")]
            Texto,

            [Display(Name = "Número")]
            Numero,

            [Display(Name = "Data")]
            Data,

            [Display(Name = "Data e Hora")]
            DataHora
        }

        [KeyField]
        [Description("ID")]
        [Layout(Tamanho = 1)]
        [PesquisaGenerica(PesquisaCampoTipo.CampoChave)]
        [FieldName("CODSEQ")]
        public int Id { get; set; }

        /// <summary>
        /// Nome do atributo cadastrado em Cadastro>Clientes botão Atributos campo "Atributo".
        /// </summary>
        [Description("Atributo")]
        [Layout(Tamanho = 2)]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [FieldName("ATRIBUTO")]
        [StringLength(150)]
        public string Atributo { get; set; }

        /// <summary>
        /// Tipo de resposta.
        /// </summary>
        [Description("Tipo de resposta")]
        [Layout(Tamanho = 4)]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa)]
        [FieldName("TIPORESP")]
        public int TipoResp { get; set; }

        public RespostaAtributoEnum TipoResposta { get; set; }

        /// <summary>
        /// Origem do atributo.
        /// </summary>
        [FieldName("ORIGEM")]
        public string Origem { get; set; }

        /// <summary>
        /// Identifica se o atributo e obrigatório.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("OBRIGATORIO")]
        public bool Obrigatorio { get; set; }

        /// <summary>
        /// Empresa do atributo (geralcad.empresa).
        /// </summary>
        [FieldName("EMPRESA")]
        public int Empresa { get; set; }

        /// <summary>
        /// Cor do atributo.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("COR")]
        public bool Cor { get; set; }

        /// <summary>
        /// Vínculo. (Usado em customizações).
        /// </summary>
        [FieldName("CODVINCULO")]
        public int VinculoId { get; set; }

        /// <summary>
        /// Indica se o atributo pode ter múltiplas respostas.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("ATRIBRESPMULT")]
        public bool PermiteMultiplasRespostas { get; set; }

        /// <summary>
        /// Indica, se mostra todas as respostas de uma só vez, na WEB.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("ATRIBTODASRESP")]
        public bool MostrarTodasRespostas { get; set; }

        /// <summary>
        /// Grupo (grupoatributo.codseq).
        /// </summary>
        [FieldName("GRUPO")]
        public int GrupoId { get; set; }

        /// <summary>
        /// Flag que indica se deverá logar ações de inserir, alterar e deletar respostas do atributo.
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("REGISTRALOG")]
        public bool RegistraLog { get; set; }

        /// <summary>
        /// Exibir atributos dos cálculos na ficha de cálculo (T/F).
        /// </summary>
        [TypeDetail(new FieldTypeDetail[] { FieldTypeDetail.BoolChar })]
        [FieldName("FICHACALC")]
        public bool MostraFichaCalculo { get; set; }

        /// <summary>
        /// Guarda os códigos dos eventos que deverão responder ao respectivo atributo obrigatório.
        /// </summary>
        [FieldName("DETALHAMENTO")]
        [StringLength(200)]
        public string Detalhamento { get; set; }         

        public ListaErros DeleteChildren(DBContexto dbctx = null)
        {
            var erros = new ListaErros();
            if (Id > 0)
            {
                List<AtributoRespostaBO> respostas = AtributoRespostaBO.CarregarRespostasPorAtributoId(dbctx, Id);
                erros.AddErrors(GenericOP.Deletar(respostas, dbctx));
            }
            return erros;
        }

        public static List<AtributoBO> CarregarPorOrigem(DBContexto dbctx, string origem)
        {
            var filtros = new List<ConditioningStrut>() {
                new ConditioningStrut(OperatorType.equalString, "ORIGEM", origem)
            };
            return GenericOP.CarregarLista<AtributoBO>(filtros, dbctx);
        }

        public static List<string> CarregarRespostasDoAtributoListaFixa(DBContexto dbctx, int atributoId)
        {
            var results = new List<ResultQueryWithType>();
            var parameters = new List<ParameterData>() {
                new ParameterData { dbType = DbType.Int32, name = "codAtributo", value = atributoId }
            };

            string query = "select (codseq || '_' || resposta || '_' || codigoatrib) as result from respatrb where codigoatrib = @codAtributo";
            
            UniversalDAO<ResultQueryWithType> dao = new UniversalDAO<ResultQueryWithType>(dbctx, false);
            results = dao.GetObjects(query, parameters);

            return results.Select(r => Convert.ToString(r.resultQuery))
                                              .ToList();
        }

        public static ListWithError<AtributoBO> CarregarPorOrigemEDetalhamento(DBContexto dbctx, string detalhamento, string origem)
        {
            var filtros = new List<ConditioningStrut>()
            {
                new ConditioningStrut(OperatorType.like, "DETALHAMENTO", detalhamento),
                new ConditioningStrut(OperatorType.equalString, "ORIGEM", origem)
            };

            return GenericOP.CarregarLista<AtributoBO>(false, filtros, dbctx, false);
        }
        
        public bool ValidarDeleta(EcalcValueObjectDAO vo, DBContexto dbctx = null)
        {
            var atributo = (AtributoBO)vo;            
            List<AtributoRespondidoBO> respostasAtributo = AtributoRespondidoBO.CarregaListaPorAtributoId(dbctx, atributo.Id);
            if (respostasAtributo.Count > 0)
            {
                vo.Erros.AddError($"Atributo {atributo.Atributo} ({atributo.Id}) possui {respostasAtributo.Count} respostas por isto não pode ser apagado", ErrorType.Error);
            }
            return (!vo.Erros.HasAnyError);
        }

        public static List<string> CarregarCodSeqDeRespostas(int codAtributo, DBContexto dbctx)
        {
            List<ResultQueryWithType> results;
            List<ParameterData> parameters = new List<ParameterData>();
            parameters.Add(new ParameterData { dbType = DbType.Int32, name = "codAtributo", value = codAtributo });

            string query = "select codseq as result from respatrb where codigoatrib = @codAtributo";

            UniversalDAO<ResultQueryWithType> dao = new UniversalDAO<ResultQueryWithType>(dbctx, false);
            results = dao.GetObjects(query, parameters);

            List<string> listRespostas = results.Select(r => Convert.ToString(r.resultQuery)).ToList();
            return listRespostas;
        }

        public static ListWithError<AtributoBO> CarregarPorOrigemECodEvento(string codEvento, string origem, DBContexto dbctx = null)
        {
            List<ConditioningStrut> conditions = new List<ConditioningStrut>();
            conditions.Add(new ConditioningStrut(OperatorType.like, "DETALHAMENTO", codEvento));
            conditions.Add(new ConditioningStrut(OperatorType.equalString, "ORIGEM", origem));
            return GenericOP.CarregarLista<AtributoBO>(false, conditions, dbctx, false);
        }

        public static List<string> CarregarRespostasDoAtributoListaFixa(int codAtributo, DBContexto dbctx = null)
        {
            List<ResultQueryWithType> results;
            List<ParameterData> parameters = new List<ParameterData>();
            parameters.Add(new ParameterData { dbType = DbType.Int32, name = "codAtributo", value = codAtributo });

            string query = "select (codseq || '_' || resposta || '_' || codigoatrib) as result from respatrb where codigoatrib = @codAtributo";

            UniversalDAO<ResultQueryWithType> dao = new UniversalDAO<ResultQueryWithType>(dbctx, false);
            results = dao.GetObjects(query, parameters);

            List<string> listRespostas = results.Select(r => Convert.ToString(r.resultQuery)).ToList();
            return listRespostas;
        }

        public static List<AtributoBO> CarregarPorCodAtributo(string listCodAtrib, string origem, DBContexto dbctx = null)
        {
            List<ConditioningStrut> conditions = new List<ConditioningStrut>();
            conditions.Add(new ConditioningStrut(OperatorType.equalString, "ORIGEM", origem));
            conditions.Add(new ConditioningStrut(OperatorType._in, "CODSEQ", listCodAtrib));

            return GenericOP.CarregarLista<AtributoBO>(false, conditions, dbctx, false);
        }
    }
}

