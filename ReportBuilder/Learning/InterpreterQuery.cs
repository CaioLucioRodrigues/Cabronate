using Cabronate.Base;
using Cabronate.DAO.ReportBuilder.StructureJsons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.ReportBuilder.Learning
{
    public class InterpreterQuery
    {
        private string _fromSession;
        private List<string> _joinsSession;
        private List<string> _tables;

        public List<JsonTableStructure> AllTablesInterpreted { get; private set; }

        public InterpreterQuery()
        {
            _tables = new List<string>();
            _joinsSession = new List<string>();
            AllTablesInterpreted = new List<JsonTableStructure>();
        }

        public List<JsonTableStructure> Interpret(string query)
        {
            List<JsonTableStructure> currentTables = new List<JsonTableStructure>();
            List<string> querySymbols = new List<string>() { "(", "=", ")", "." };
            query = query.ToUpper();

            // DIVIDINDO AS SESSIONS
            //------------------------------------

            int fromInitialPosition = query.IndexOf("FROM");
            int fromFinalPosition = query.IndexOf("JOIN");

            _fromSession = query.Slice(fromInitialPosition, fromFinalPosition);

            List<int> allJoinsIndexes = EcalcString.IndexOfAllIndexes(query, "JOIN");

            for (int i = 0; i <= allJoinsIndexes.Count - 1; i++)
            {
                if (i + 1 < allJoinsIndexes.Count)
                    _joinsSession.Add(query.Slice(allJoinsIndexes[i], allJoinsIndexes[i + 1]));
                else
                    _joinsSession.Add(query.Substring(allJoinsIndexes[i], query.Length - allJoinsIndexes[i]).IncludeSeparators(querySymbols, " "));
            }

            // PEGANDO A TABELA DO FROM
            //------------------------------------

            List<string> fromProcessedList = _fromSession.Split(' ')
                                                         .ToList()
                                                         .RemoveEmptyElements();

            if ((fromProcessedList[0] != "FROM") && (fromProcessedList.Count() <= 1))
                throw new Exception("Erro ao converter cláusula FROM");

            string tableFrom = fromProcessedList[1];

            // MONTANDO AS TABELAS DOS JOINS
            //------------------------------------

            List<List<string>> joinsProcessedList = new List<List<string>>();
            _joinsSession.ForEach(j =>
            {
                joinsProcessedList.Add(j.Split(' ')
                                        .ToList()
                                        .RemoveEmptyElements());
            });

            // MONTANDO A LIGACAO DO FROM
            //------------------------------------

            joinsProcessedList.ForEach(join =>
            {
                string tableTarget = join[join.IndexOf("=") + 1];
                if (tableTarget == tableFrom)
                {
                    currentTables.Add(CreateJsonTableStructureFromFrom(tableFrom, join));
                }
            });

            // MONTANDO A LIGACAO DO FROM
            //------------------------------------
            joinsProcessedList.ForEach(join =>
            {
                string tableOrigin = join[join.IndexOf("JOIN") + 1];
                string tableTarget = join[join.IndexOf("=") + 1];
                if (tableTarget == tableFrom)
                {
                    currentTables.Add(CreateJsonTableStructureFromJoin(join, tableFrom));
                }
            });

            //Adicionar currentTables em AllTablesInterpreted;
            return currentTables;
        }

        private JsonTableStructure CreateJsonTableStructureFromFrom(string table, List<string> join)
        {
            string tableForTarget = join[join.IndexOf("(") + 1];
            JsonTableStructure jsonTableStructure = new JsonTableStructure();
            jsonTableStructure.TableName = table;

            JsonRelationshipStructure jsonRelationship = new JsonRelationshipStructure(tableForTarget);
            JsonLinksStructure jsonLinkStructure = new JsonLinksStructure();
            jsonLinkStructure.TableTargetConnectoinField = join[join.IndexOf("=") - 1];
            jsonLinkStructure.SelfConnectionField = join[join.IndexOf(table) + 2];

            jsonRelationship.Links.Add(jsonLinkStructure);
            jsonTableStructure.Relationships.Add(jsonRelationship);

            return jsonTableStructure;
        }

        private JsonTableStructure CreateJsonTableStructureFromJoin(List<string> join, string tableForTarget)
        {
            string tableTarget = tableForTarget;
            JsonTableStructure jsonTableStructure = new JsonTableStructure();
            jsonTableStructure.TableName = join[join.IndexOf("(") + 1];

            JsonRelationshipStructure jsonRelationship = new JsonRelationshipStructure(tableTarget);
            JsonLinksStructure jsonLinkStructure = new JsonLinksStructure();
            jsonLinkStructure.TableTargetConnectoinField = join[join.IndexOf(tableTarget) + 2];

            jsonLinkStructure.SelfConnectionField = join[join.IndexOfSpecificIncidence(jsonTableStructure.TableName, 2) + 2];


            jsonRelationship.Links.Add(jsonLinkStructure);
            jsonTableStructure.Relationships.Add(jsonRelationship);

            return jsonTableStructure;
        }
    }
}
