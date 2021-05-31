using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.ReportBuilder.DTOs
{
    [Serializable]
    public class ColumnDTO
    {
        public string TableName { get; set; }

        public string TableFriendlyName { get; set; }

        public string FieldName { get; set; }

        public string FieldFriendlyName { get; set; }

        public int Position { get; set; }

        public string DefaultValue { get; set; }
    }
}
