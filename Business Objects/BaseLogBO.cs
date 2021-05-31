using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Operations;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.Business_Objects
{
    [Serializable]
    public class BaseLogBO : EcalcValueObjectDAO
    {
        public BaseLogBO() {}

        [KeyField]
        [FieldName("ID")]
        public int Id { get; set; }

        [FieldName("Field")]
        public string Field { get; set; }

        [FieldName("OldValue")]
        public string OldValue { get; set; }

        [FieldName("NewValue")]
        public string NewValue { get; set; }

        [FieldName("Data")]
        public DateTime Date { get; set; }

        [FieldName("Usuario")]
        public string User { get; set; }

        [FieldName("Type")]
        public string Type { get; set; }

        [FieldName("Reason")]
        public string Reason { get; set; }

        [FieldName("Version")]
        public string Version { get; set; }
    }

    [Serializable]
    public sealed class BaseLogContainer
    {
        private readonly EcalcValueObjectDAO _vo;

        public BaseLogContainer(EcalcValueObjectDAO vo) 
        {
            Logs = new List<BaseLogBO>();
            _vo = vo;
        }

        public List<BaseLogBO> Logs { get; }

        public void Load(DBContexto dbctx)
        {
            var logs = new List<BaseLogBO>();
            logs.AddRange(LogOperationBO.CarregarLogs(dbctx, _vo));
            logs.AddRange(LogFlowBO.CarregarLogs(dbctx, _vo));
            logs.AddRange(LogTableBO.CarregarLogs(dbctx, _vo));

            Logs.Clear();
            Logs.AddRange(logs.OrderBy(l => l.Date)
                              .ToList());
        }
    }
}
