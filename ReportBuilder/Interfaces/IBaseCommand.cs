using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.DTOs;
using Cabronate.DAO.ReportBuilder.Structures;
using System.Collections.Generic;

namespace Cabronate.DAO.ReportBuilder.Interfaces
{
    public interface IBaseCommand
    {
        string BaseCommand(StructureContainer structureContainer, TipoBanco db, List<ColumnDTO> columns = null);
    }
}
