using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.DTOs;
using Cabronate.DAO.ReportBuilder.Interfaces;
using Cabronate.DAO.ReportBuilder.Types;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.ReportBuilder.Structures.BaseCommands
{
    public class StructureBaseCommandNonQuery : StructureBaseCommand, IBaseCommand
    {
        public string BaseCommand(StructureContainer structureContainer, TipoBanco db, List<ColumnDTO> columns = null)
        {
            StringBuilder command = new StringBuilder();

            command.Append("SELECT ");

            switch (structureContainer.OperationType)
            {
                case Operation.Undefined:
                    throw new System.Exception("Operador não definido");
                case Operation.Sum:
                    command.AppendLine($"SUM({GetFieldWithNick(new StructureJsons.JsonFieldStructure())})");
                    break;
                case Operation.Max:
                    command.AppendLine($"MAX({GetFieldWithNick(new StructureJsons.JsonFieldStructure())})");
                    break;
                case Operation.Min:
                    command.AppendLine($"MIN({GetFieldWithNick(new StructureJsons.JsonFieldStructure())})");
                    break;
                case Operation.Count:
                    command.AppendLine($"COUNT({GetFieldWithNick(new StructureJsons.JsonFieldStructure())})");
                    break;
                case Operation.Average:
                    command.AppendLine($"AVG({GetFieldWithNick(new StructureJsons.JsonFieldStructure())})");
                    break;
                default:
                    break;
            }

            return command.ToString();
        }
    }   
}
