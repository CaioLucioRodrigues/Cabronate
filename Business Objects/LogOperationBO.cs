using Cabronate.Base;
using Cabronate.Base.Attributes;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Value_Objects;
using Cabronate.DAO.UniversalDAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Data;

namespace Cabronate.DAO.Business_Objects
{
    [NotLogable]
    [Serializable]
    [TableName("REGISTROSDEOPERACAO")]
    [Description("Histórico")]
    [Permissao(PermIDGlobal = "geral.historico.consulta")]
    public sealed class LogOperationBO : EcalcValueObjectDAO, ILoadChildren, IPersistChildren, IDeleteChildren
    {
        public enum TipoOperacaoLog
        {
            [Display(Name = "Inclusão")]
            Inclusao = 1,

            [Display(Name = "Alteração")]
            Alteracao = 2,

            [Display(Name = "Exclusão")]
            Exclusao = 3
        };

        public LogOperationBO()
        {
            Detalhes = new List<LogOperationDetailBO>();
        }

        [KeyField]
        [FieldName("ID")]
        [PesquisaGenerica(PesquisaCampoTipo.CampoChave | PesquisaCampoTipo.MostradoNaPesquisa)]
        [Display(Name = "ID")]
        [LayoutAttribute("col-md-2")]
        public int ID { get; set; }

        [StringLength(50)]
        [FieldName("ENTIDADE")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Entidade")]
        [LayoutAttribute("col-md-2")]
        public string Entidade { get; set; }

        [FieldName("ENTIDADEID")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Entidade ID")]
        [LayoutAttribute("col-md-2")]
        public int EntidadeID { get; set; }

        [FieldName("TIPOOPERACAO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Operação")]
        [LayoutAttribute("col-md-2")]
        public TipoOperacaoLog TipoOperacao { get; set; }

        [FieldName("USUARIO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Usuário")]
        [LayoutAttribute("col-md-2")]
        public int Usuario { get; set; }

        [FieldName("DATA")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Data")]
        [LayoutAttribute("col-md-2")]
        public DateTime Data { get; set; }

        [FieldName("VERSAO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa | PesquisaCampoTipo.UsadoComoFiltro)]
        [Display(Name = "Versão")]
        [LayoutAttribute("col-md-2")]
        public string Versao { get; set; }

        [Display(Name = "Detalhes Histórico")]
        [Layout("col-md-12")]
        public List<LogOperationDetailBO> Detalhes { get; }

        public void LoadChildren(DBContexto dbctx = null)
        {
            Detalhes.Clear();
            Detalhes.AddRange(LogOperationDetailBO.CarregarListaPorRegistroID(dbctx, ID));
        }

        public ListaErros SaveChildren(int IDFather, DBContexto dbctx = null)
        {
            this.Detalhes.ForEach(d => d.RegistroID = IDFather);
            return GenericOP.Salvar(Detalhes, dbctx);
        }

        public ListaErros DeleteChildren(DBContexto dbctx = null)
        {
            return GenericOP.Deletar(Detalhes, dbctx);
        }

        public override GenericOP GetOP()
        {
            return null;
        }

        public static ListWithError<LogOperationBO> CarregarListaPorEntidade(DBContexto dbctx, 
            int entidadeID, string entidadeNome)
        {
            var conditionList = new List<ConditioningStrut>()
            {
                new ConditioningStrut(OperatorType.equalString, "ENTIDADE", entidadeNome),
                new ConditioningStrut(OperatorType.equal, "ENTIDADEID", entidadeID)
             };
            return GenericOP.CarregarLista<LogOperationBO>(conditionList, dbctx);
        }

        public static List<BaseLogBO> CarregarLogs(DBContexto dbctx, EcalcValueObjectDAO vo)
        {   
            var query = new StringBuilder();
            query.AppendLine(" select ");
            query.AppendLine("     r.versao as version,");            
            query.AppendLine("     r.data as data,");
            query.AppendLine("     s.ident as usuario,");
            query.AppendLine("     d.campo as field,");
            query.AppendLine("     d.valorantigo as oldValue,");
            query.AppendLine("     d.valornovo as newValue,");
            query.AppendLine("     case r.tipooperacao");
            query.AppendLine("         when 1 then 'Inclusão'");
            query.AppendLine("         when 2 then 'Alteração'");
            query.AppendLine("         when 3 then 'Exclusão'");
            query.AppendLine("         else 'Outros'");
            query.AppendLine("     end as type");                    
            query.AppendLine(" from registrosdeoperacao r");
            query.AppendLine(" join registrosdeoperacaodet d on (d.registroid = r.id)");
            query.AppendLine(" left join senha s on (s.codseq = r.usuario)");
            query.AppendLine(" where r.entidade = @entidade");
            query.AppendLine("   and r.entidadeid = @entidadeId");

            string tableName = AttributeReaderSingleton.AttributeReader.getDescriptionTableName(vo);
            List<ParameterData> parameters = new List<ParameterData>()
            {
                new ParameterData { dbType = DbType.String, name = "entidade", value = tableName },
                new ParameterData { dbType = DbType.Int32, name = "entidadeId", value = vo.KeyFieldValue }
            };            
            return GenericOP.CarregarLista<BaseLogBO>(dbctx, query.ToString(), parameters);
        }
    }

    [NotLogable]
    [TableName("REGISTROSDEOPERACAODET")]
    [Description("Detalhes Histórico")]
    [Permissao(PermIDGlobal = "geral.historico.consulta")]
    public class LogOperationDetailBO : EcalcValueObjectDAO
    {
        [KeyField]
        [FieldName("ID")]
        public int ID { get; set; }

        [FieldName("REGISTROID")]
        public int RegistroID { get; set; }

        [StringLength(500)]
        [FieldName("VALORANTIGO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa)]
        [Display(Name = "Valor Antigo")]
        [LayoutAttribute("col-md-2")]
        public string ValorAntigo { get; set; }

        [StringLength(500)]
        [FieldName("VALORNOVO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa)]
        [Display(Name = "Valor Novo")]
        [LayoutAttribute("col-md-2")]
        public string ValorNovo { get; set; }

        [FieldName("CAMPO")]
        [PesquisaGenerica(PesquisaCampoTipo.MostradoNaPesquisa)]
        [Display(Name = "Campo")]
        [LayoutAttribute("col-md-2")]
        public string Campo { get; set; }

        public override GenericOP GetOP()
        {
            return null;
        }

        public static List<LogOperationDetailBO> CarregarListaPorRegistroID(DBContexto dbctx, int regID)
        {
            var conditionList = new List<ConditioningStrut>() {
                new ConditioningStrut(OperatorType.equalString, "REGISTROID", regID)
            };
            return GenericOP.CarregarLista<LogOperationDetailBO>(conditionList, dbctx);
        }
    }
}
