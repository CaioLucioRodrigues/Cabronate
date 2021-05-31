using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Cabronate.DAO.Business_Objects
{
    public enum TipoLogTabela
    {
        logTabIndefinido = 0, logTabOrcamento = 47, logTabOS = 48, logTabPedidos = 82, logTabRequisicao = 83, logTabChapa = 124, logTabPadrao = 127, 
        logTabExcecao = 128, logTabFeriados = 129, logTabIndice = 145, logTabAcoes = 164
    }

    public enum TipoLogAcao { Incluir = 1, Alterar = 2, Excluir = 3 }

    [NotLogable]
    [Serializable]
    [TableName("LOGTABELA")]
    public sealed class LogTableBO : EcalcValueObjectDAO, ILoadChildren, IPersistChildren, IDeleteChildren
    {
        public LogTableBO()
        {
            Itens = new List<LogTabelaItemBO>();
        }

        /// <summary>
        /// Log genérico de tabelas.
        /// </summary>
        [KeyField]
        [FieldName("CODSEQ")]
        public int Id { get; set; }

        /// <summary>
        /// Tipo de log.
        /// </summary>
        [FieldName("TIPO")]
        public TipoLogTabela Tipo { get; set; }

        /// <summary>
        /// Data de gravação do log.
        /// </summary>
        [FieldName("DATA")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Usuário (senha.codseq).
        /// </summary>
        [FieldName("USUARIO")]
        public int Usuario { get; set; }

        /// <summary>
        /// Código sequencial do registro da tabela "logada".
        /// </summary>
        [FieldName("CODSEQTAB")]
        public int CodseqTab { get; set; }

        /// <summary>
        /// Campo alterado.
        /// </summary>
        [StringLength(20)]
        [FieldName("CAMPO")]
        public string Campo { get; set; }

        /// <summary>
        /// Valor antigo.
        /// </summary>
        [FieldName("OLDVALOR")]
        public string ValorAntigo { get; set; }

        /// <summary>
        /// Valor novo.
        /// </summary>
        [FieldName("NEWVALOR")]
        public string ValorNovo { get; set; }

        /// <summary>
        /// Sub-Tipo do log (0 ou 1 – Tabela/Geral, 2- Crédito cedido ao cliente).
        /// </summary>
        [FieldName("SUBTIPO")]
        public int SubTipo { get; set; }

        /// <summary>
        /// Tipo de Ação (1-Incluir, 2-Excluir, 3-Alterar).
        /// </summary>
        [FieldName("ACAO")]
        public TipoLogAcao Acao { get; set; }

        /// <summary>
        /// Justificativa para alteração dos campos da tabela, exclusão do registro ou indicação de “Novo registro”.
        /// </summary>
        [FieldName("MOTIVO")]
        public string Motivo { get; set; }

        public List<LogTabelaItemBO> Itens { get; }

        public override GenericOP GetOP()
        {
            return null;
        }

        public ListaErros DeleteChildren(DBContexto dbctx = null)
        {
            return GenericOP.Deletar(Itens, dbctx);
        }

        public ListaErros SaveChildren(int IDFather, DBContexto dbctx = null)
        {
            Itens.ForEach(d => d.LogId = IDFather);
            return GenericOP.Salvar(Itens, dbctx);
        }

        public void LoadChildren(DBContexto dbctx = null)
        {
            Itens.Clear();
            Itens.AddRange(LogTableBO.CarregarItensPorLogId(dbctx, Id));
        }

        public static ListaErros SaveAlterWithItem(DBContexto Dbctx, TipoLogTabela Tipo, int Usuario, int CodSeqTab,
            string Motivo, string Campo, string OldValor, string NewValor, DateTime Data)
        {
            var listaErros = new ListaErros();
            LogTableBO log = new LogTableBO()
            {
                Tipo = Tipo,
                Usuario = Usuario,
                Campo = "",
                ValorAntigo = OldValor,
                ValorNovo = NewValor,
                Acao = TipoLogAcao.Alterar,
                Data = Data,
                CodseqTab = CodSeqTab,
                Motivo = Motivo
            };

            log.Itens.Add(new LogTabelaItemBO(log.Id, Campo, OldValor, NewValor));
            listaErros = GenericOP.Salvar(log, Dbctx);
            return listaErros;
        }

        public static List<LogTabelaItemBO> CarregarItensPorLogId(DBContexto dbctx, int logId)
        {
            var conditionList = new List<ConditioningStrut>() {
                new ConditioningStrut(OperatorType.equalString, "CODLOG", logId)
            };
            return GenericOP.CarregarLista<LogTabelaItemBO>(conditionList, dbctx);
        }

        public static List<BaseLogBO> CarregarLogs(DBContexto dbctx, EcalcValueObjectDAO vo)
        {
            var query = new StringBuilder();
            query.AppendLine(" select");
            query.AppendLine("     l.versao as version,");
            query.AppendLine("     l.data as data,");
            query.AppendLine("     s.ident as usuario,");
            query.AppendLine("     i.campo as field,");
            query.AppendLine("     i.oldvalor as oldValue,");
            query.AppendLine("     i.newvalor as newValue,");
            query.AppendLine("     l.motivo as reason,");
            query.AppendLine("     case l.acao");
            query.AppendLine("         when 1 then 'Inclusão'");
            query.AppendLine("         when 2 then 'Alteração'");
            query.AppendLine("         when 3 then 'Exclusão'");
            query.AppendLine("         else 'Outros'");
            query.AppendLine("     end as type");
            query.AppendLine(" from logtabela l");
            query.AppendLine(" join logtabelaitem i on (i.codlog = l.codseq)");
            query.AppendLine(" left join senha s on (s.codseq = l.usuario)");
            query.AppendLine(" where l.tipo = @entidade");
            query.AppendLine("   and l.codseqtab = @entidadeId");

            TipoLogTabela tipoLog = AttributeReaderSingleton.AttributeReader.GetLogTable(vo);
            if (tipoLog != TipoLogTabela.logTabIndefinido)
            {
                List<ParameterData> parameters = new List<ParameterData>()
                {
                    new ParameterData { dbType = DbType.Int32, name = "entidade", value = (int)tipoLog },
                    new ParameterData { dbType = DbType.Int32, name = "entidadeId", value = vo.KeyFieldValue }
                };
                return GenericOP.CarregarLista<BaseLogBO>(dbctx, query.ToString(), parameters);
            }
            else
                return new List<BaseLogBO>();
        }
    }

    [NotLogable]
    [Serializable]
    [TableName("LOGTABELAITEM")]
    public class LogTabelaItemBO : EcalcValueObjectDAO
    {
        public LogTabelaItemBO() { }

        public LogTabelaItemBO(int logId, string campo, string valorAntigo, string valorNovo)
        {
            this.LogId = logId;
            this.Campo = campo;
            this.ValorAntigo = valorAntigo;
            this.ValorNovo = valorNovo;
        }

        /// <summary>
        /// Detalhamento do log genérico de tabelas.
        /// </summary>
        [KeyField]
        [FieldName("CODSEQ")]
        public int Id { get; set; }

        /// <summary>
        /// Código do log (logtabela.CodSeq).
        /// </summary>
        [FieldName("CODLOG")]
        public int LogId { get; set; }

        /// <summary>
        /// Nome do campo alterado.
        /// </summary>
        [FieldName("CAMPO")]
        public string Campo { get; set; }

        /// <summary>
        /// Valor anterior do campo alterado.
        /// </summary>
        [FieldName("OLDVALOR")]
        public string ValorAntigo { get; set; }

        /// <summary>
        /// Novo valor do campo alterado.
        /// </summary>
        [FieldName("NEWVALOR")]
        public string ValorNovo { get; set; }
    }
}

