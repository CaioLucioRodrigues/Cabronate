using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureReader
    {
        private readonly StructureValidador _validador;
        private readonly string _file;
        private List<JsonTableStructure> _structure;

        public StructureReader(string file)
        {
            _validador = new StructureValidador();
            _file = file;
        }

        public (List<JsonTableStructure>, ListaErros) Load()
        {
             ListaErros erros = new ListaErros();            
            _structure = JsonConvert.DeserializeObject<List<JsonTableStructure>>(_file);
            erros.AddErrors(Validate());
            return (_structure, erros);
        }

        public StructureContainer LoadContainer(string dataJson)
        {
            return JsonConvert.DeserializeObject<StructureContainer>(dataJson);
        }

        private ListaErros Validate()
        {
            ListaErros erros = new ListaErros();
            erros.AddErrors(_validador.HasDuplicateTables(_structure));
            erros.AddErrors(_validador.HasDuplicateFields(_structure));
            erros.AddErrors(_validador.HasUnnamedTables(_structure));
            erros.AddErrors(_validador.HasUnnamedFields(_structure));
            return erros;
        }
    }
}