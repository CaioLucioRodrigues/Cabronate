namespace Cabronate.DAO.ReportBuilder.DTOs
{
    public class FilterDTO
    {
        public FilterDTO() { }

        public string TableName { get; set; }

        public string TableFriendlyName { get; set; }

        public string FieldName { get; set; }

        public string FieldFriendlyName { get; set; }

        public object Value { get; set; }
    }
}