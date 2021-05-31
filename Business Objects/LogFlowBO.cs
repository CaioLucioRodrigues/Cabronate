using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Cabronate.DAO.Business_Objects
{
    [Serializable]
    [TableName("FLUXOSLOG")]
    public sealed class LogFlowBO : EcalcValueObjectDAO
    {
        public LogFlowBO() { }

        public LogFlowBO(string campo, string mensagem, DateTime data)
        {
            Data = data;
            Campo = campo;
            Mensagem = mensagem;
        }

        [KeyField]
        [FieldName("CODSEQ")]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required]
        [FieldName("DATA")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        private DateTime Data { get; set; }

        [Required]
        [FieldName("CAMPO")]
        [Display(Name = "Campo")]
        [StringLength(50, ErrorMessage = "O {0} deve ter no máximo {1}.")]
        public string Campo { get; set; }

        [Required]
        [FieldName("TABELA")]
        [Display(Name = "Tabela")]
        [StringLength(50, ErrorMessage = "O {0} deve ter no máximo {1}.")]
        public string Tabela { get; set; }

        [Required]
        [FieldName("CODTABELAORIGEM")]
        public int CodigoTabelaOrigem { get; set; }

        [Required]
        [FieldName("MENSAGEM")]
        [Display(Name = "Mensagem")]
        [StringLength(1000, ErrorMessage = "O {0} deve ter no máximo {1}.")]
        private string Mensagem { get; set; }

        [Required]
        [FieldName("CODUSUARIO")]
        [Display(Name = "Código Usuário")]
        public int CodigoUsuario { get; set; }

        public override GenericOP GetOP()
        {
            return null;
        }

        public static List<BaseLogBO> CarregarLogs(DBContexto dbctx, EcalcValueObjectDAO vo)
        {
            var query = new StringBuilder();
            query.AppendLine(" select");
            query.AppendLine("     f.data as data,");
            query.AppendLine("     s.ident as usuario,");
            query.AppendLine("     f.campo as field,");
            query.AppendLine("     f.mensagem as reason,");
            query.AppendLine("     'Fluxo' as type,");
            query.AppendLine("     '' as version");
            query.AppendLine(" from fluxoslog f");
            query.AppendLine(" left join senha s on (s.codseq = f.codusuario)");
            query.AppendLine(" where f.tabela = @entidade");
            query.AppendLine("   and f.codtabelaorigem = @entidadeId");

            string tableName = AttributeReaderSingleton.AttributeReader.getDescriptionTableName(vo);
            List<ParameterData> parameters = new List<ParameterData>()
            {
                new ParameterData { dbType = DbType.String, name = "entidade", value = tableName },
                new ParameterData { dbType = DbType.Int32, name = "entidadeId", value = vo.KeyFieldValue }
            };
            return GenericOP.CarregarLista<BaseLogBO>(dbctx, query.ToString(), parameters);
        }
    }
}

