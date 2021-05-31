using Cabronate.Base.Interfaces;
using Cabronate.DAO.ReportBuilder.Processed;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Cabronate.DAO.ReportBuilder.Types;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureContainer : ISelfManagingCacheObject
    {       

        public StructureContainer(ProcessedJson processedJson)
        {
            ProcessedJson = processedJson;
            Joins = new List<StructureConnectionJoin>();
            Fields = new List<JsonFieldStructure>();            
            Filters = new List<StructureFilter>();
            OrderBys = new List<string>();
            GroupBys = new List<string>();
            NeedGroupBy = false;
        }

        public ProcessedJson ProcessedJson { get; set; }

        public Report ReportType { get; set; }        

        public List<JsonFieldStructure> Fields { get; set; }        

        public List<StructureFilter> Filters { get; set; }

        public List<StructureConnectionJoin> Joins { get; set; }

        public List<JsonTableStructure> Tables => Joins.Select(j => j.TableOrign)
                                                       .ToList();

        public List<string> OrderBys { get; set; }

        public List<string> GroupBys { get; set; }

        public bool NeedGroupBy { get; set; }

        public int RowsCount { get; set; }

        #region fields NonQuery
        public Operation OperationType { get; set; }

        public JsonFieldStructure OperationField { get; set; }
        #endregion

        public string SerializeContainer()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            });
        }

        public double GetTimeOutInMinutes()
        {
            return 5;
        }

        public List<StructureFilter> NonFixedFilters => Filters.Where(f => !f.Fixed)
                                                               .ToList();
    }
}
