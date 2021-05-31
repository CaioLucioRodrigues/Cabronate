using Cabronate.DAO.ReportBuilder.StructureJsons;

namespace Cabronate.DAO.ReportBuilder.Structures.BaseCommands
{
    public class StructureBaseCommand
    {
        public string GetFieldWithNick(JsonFieldStructure field)
        {
            return $"{field.TableName}.{field.FieldName}";
        }
    }
}
