using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.Structures
{
    public class StructureValidador
    {
        public StructureValidador() { }

        public ListaErros HasDuplicateTables(List<JsonTableStructure> structure)
        {
            ListaErros erros = new ListaErros();

            structure.Select(t => t.TableName)
                     .GroupBy(g => g)
                     .Where(g => g.Count() > 1)
                     .ToList()
                     .ForEach(t => { erros.AddError($"O nome da tabela '{t.Key}' está duplicada", ErrorType.Error); });

            structure.Select(t => t.TableFriendlyName)
                     .GroupBy(x => x)
                     .Where(group => group.Count() > 1)
                     .ToList()
                     .ForEach(action: d => { erros.AddError($"O nome amigável da tabela '{d.Key}' está duplicada", ErrorType.Error); });
            return erros;
        }

        public ListaErros HasDuplicateFields(List<JsonTableStructure> structure)
        {
            ListaErros erros = new ListaErros();

            structure.ForEach(t =>
            {
                t.Fields.Select(f => f.FieldName)
                        .GroupBy(g => g)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .ToList()
                        .ForEach(f => { erros.AddError($"O nome do campo '{f.Key}' da tabela {t.ToString()} está duplicado", ErrorType.Error); });

                t.Fields.Select(f => f.FieldFriendlyName)
                        .GroupBy(g => g)
                        .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                        .ToList()
                        .ForEach(f => { erros.AddError($"O nome do campo '{f.Key}' da tabela {t.ToString()} está duplicado", ErrorType.Error); });
            });
            return erros;
        }

        public ListaErros HasUnnamedTables(List<JsonTableStructure> structure)
        {
            ListaErros erros = new ListaErros();

            bool error = (structure.Where(t => string.IsNullOrWhiteSpace(t.ToString()))
                                   .Count() > 0);

            if (error) erros.AddError("Existem tabelas sem nome", ErrorType.Error);
            return erros;
        }

        public ListaErros HasUnnamedFields(List<JsonTableStructure> structure)
        {
            ListaErros erros = new ListaErros();

            structure.ForEach(t =>
            {
                t.Fields.Where(f => string.IsNullOrWhiteSpace(f.ToString()))
                        .ToList()
                        .ForEach(f => { erros.AddError($"Existem campos sem nome na tabela {t.ToString()}", ErrorType.Error); });
            });
            return erros;
        }
    }
}
