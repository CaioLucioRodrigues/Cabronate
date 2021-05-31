using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.SQL
{
    /// <summary>
    /// Classe responsável para montar as querys de cada Value Object que sigam as especificações básicas
    /// </summary>
    public class QueryFactory : IQueryFactory
    {
        /// <summary>
        /// Método que monta uma query de criação de um novo registro
        /// </summary>
        /// <param name="o">Objeto que deve ser persistido no banco</param>
        /// <returns>Query 'insert' montada</returns>
        public string createQuery(Object o, string provider, TipoBanco tipoBanco, List<string> fieldss = null, bool forcarID = false)
        {
            List<string> fields = AttributeReaderSingleton.AttributeReader.getFieldsName(o, fieldss);

            if ((!forcarID) && (!AutoIncrementBehaviorFactory.getAutoIncrementBehavior(provider).keyFieldInInsert()))
            {
                string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);
                fields.Remove(keyField);
            }

            string listFields = StringUtils.ListToString(fields);
            string listParams;

            if (tipoBanco == TipoBanco.Oracle)
                listParams = StringUtils.ListToString(fields, ":");
            else
                listParams = StringUtils.ListToString(fields, "@");

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.INSERT);
            query.AppendLine(ANSIWords.INTO);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ANSIWords.LEFTBRACKET);
            query.AppendLine(listFields);
            query.AppendLine(ANSIWords.RIGTHBRACKET);
            query.AppendLine(ANSIWords.VALUES);
            query.AppendLine(ANSIWords.LEFTBRACKET);
            query.AppendLine(listParams);
            query.AppendLine(ANSIWords.RIGTHBRACKET);
            return query.ToString();
        }

        /// <summary>
        /// Método que monta uma query de leitura de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser lido no banco</param>
        /// <returns>Query 'select' montada</returns>
        public string readQuery(Object o, bool list, TipoBanco tipoBanco, List<ConditioningStrut> conditionList = null)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);

            if (AttributeReaderSingleton.AttributeReader.getTableName(o).IndexOf('.') < 0)
                query.AppendLine(StringUtils.ListToString(AttributeReaderSingleton.AttributeReader.getFieldsNameWithNick(o)));
            else
                query.AppendLine(StringUtils.ListToString(AttributeReaderSingleton.AttributeReader.getFieldsName(o)));

            query.AppendLine(ANSIWords.FROM);

            if (AttributeReaderSingleton.AttributeReader.getTableName(o).IndexOf('.') < 0)
                query.AppendLine(string.Format("{0} {1}",
                    AttributeReaderSingleton.AttributeReader.getTableName(o),
                    AttributeReaderSingleton.AttributeReader.getTableName(o)));
            else
                query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));

            if (conditionList != null)
            {
                foreach (ConditioningStrut condition in conditionList)
                    query.AppendLine(ConditioningFactory.GetJoin(condition));
            }

            if ((conditionList == null) && (!list))
            {
                query.AppendLine(ANSIWords.WHERE);
                query.AppendLine(keyField);
                query.AppendLine(ANSIWords.EQUAL);
                if (tipoBanco == TipoBanco.Oracle)
                    query.AppendLine(string.Format(ANSIWords.ORACLE_PARAM, keyField));
                else
                    query.AppendLine(string.Format(ANSIWords.PARAM, keyField));
            }

            if ((conditionList != null) && (list) && (!this.hasOnlyJoins(conditionList)))
            {
                query.AppendLine(ConditioningFactory.Where());
                foreach (ConditioningStrut condition in conditionList)
                    query.AppendLine(ConditioningFactory.GetCondition(condition));
            }


            //ordenação
            if (conditionList != null)
            {
                bool OrderBy = false;
                foreach (ConditioningStrut condition in conditionList.Where(c => c.Operator == OperatorType.orderBy))
                {
                    if (!OrderBy)
                    {
                        query.AppendLine(ANSIWords.ORDERBY); OrderBy = true;
                    }
                    else
                        query.Append(ANSIWords.COMMA);

                    query.Append(ConditioningFactory.GetOrderBy(condition));
                }

                ConditioningStrut limit = conditionList.Find(c => c.Operator == OperatorType.limit);
                if (limit != null)
                    query.Append(ConditioningFactory.GetLimit(limit));
            }
            return query.ToString();
        }

        /// <summary>
        /// Método que monta uma query de atualização de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser atualizado no banco</param>
        /// <returns>Query 'update' montada</returns>
        public string updatetQuery(Object o, TipoBanco tipoBanco, List<string> fields = null)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);
            List<string> fieldsNoPK = AttributeReaderSingleton.AttributeReader.getFieldsName(o, fields);
            fieldsNoPK.Remove(keyField);

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.UPDATE);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ANSIWords.SET);
            foreach (string field in fieldsNoPK)
            {
                query.AppendLine(field);
                query.AppendLine(ANSIWords.EQUAL);

                if (tipoBanco == TipoBanco.Oracle)
                    query.AppendLine(string.Format(ANSIWords.ORACLE_PARAM, field));
                else
                    query.AppendLine(string.Format(ANSIWords.PARAM, field));

                query.AppendLine(ANSIWords.COMMA);
            }
            query.Remove((query.Length - 3), 1);//Remove última vírgula
            query.AppendLine(ANSIWords.WHERE);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.EQUAL);

            if (tipoBanco == TipoBanco.Oracle)
                query.AppendLine(string.Format(ANSIWords.ORACLE_PARAM, keyField));
            else
                query.AppendLine(string.Format(ANSIWords.PARAM, keyField));

            return query.ToString();
        }

        /// <summary>
        /// Método que monta uma query de deleção de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser deletado no banco</param>
        /// <returns>Query 'delete' montada</returns>
        public string deleteQuery(Object o, TipoBanco tipoBanco)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.DELETE);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ANSIWords.WHERE);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.EQUAL);

            if (tipoBanco == TipoBanco.Oracle)
                query.AppendLine(string.Format(ANSIWords.ORACLE_PARAM, keyField));
            else
                query.AppendLine(string.Format(ANSIWords.PARAM, keyField));

            return query.ToString();
        }

        /// <summary>
        /// Método que monta a query responsável por trazer o próximo ID da tabela
        /// </summary>
        /// <param name="o">Objeto a ser persistido</param>
        /// <returns>Próximo ID da tabela</returns>
        public string getNextIDQuery(Object o)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);
            query.AppendLine(ANSIWords.MAX);
            query.AppendLine(ANSIWords.LEFTBRACKET);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.RIGTHBRACKET);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            return query.ToString();
        }

        private bool hasOnlyJoins(List<ConditioningStrut> conditionList)
        {
            foreach (ConditioningStrut condition in conditionList)
            {
                if ((condition.Operator != OperatorType.join) && (condition.Operator != OperatorType.leftJoin))
                    return false;
            }
            return true;
        }

        public string getLockQuery(object o)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.UPDATE);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ANSIWords.SET);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.GetFirstNotKeyField(o));
            query.AppendLine(ANSIWords.EQUAL);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.GetFirstNotKeyField(o));
            query.AppendLine(ANSIWords.WHERE);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getKeyField(o));
            query.AppendLine(ANSIWords.EQUAL);
            query.AppendLine(string.Format(ANSIWords.PARAM, AttributeReaderSingleton.AttributeReader.getKeyField(o)));
            return query.ToString();
        }

        public string CountCommand(object o, List<ConditioningStrut> conditions)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);
            
            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);
            query.AppendLine(ANSIWords.COUNT);
            query.AppendLine(ANSIWords.LEFTBRACKET);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.RIGTHBRACKET);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ConditioningFactory.Where());
            foreach (ConditioningStrut condition in conditions)
                query.AppendLine(ConditioningFactory.GetCondition(condition));
            return query.ToString();
        }

        public string MaxCommand(object o, List<ConditioningStrut> conditions, string nomeColuna)
        {
            string keyField = nomeColuna;

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);
            query.AppendLine(ANSIWords.MAX);
            query.AppendLine(ANSIWords.LEFTBRACKET);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.RIGTHBRACKET);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ConditioningFactory.Where());
            foreach (ConditioningStrut condition in conditions)
                query.AppendLine(ConditioningFactory.GetCondition(condition));
            return query.ToString();
        }

        public string GetFieldValueCommand(object o, int id, string field)
        {
            string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(o);

            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);
            query.AppendLine(field);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ANSIWords.WHERE);
            query.AppendLine(keyField);
            query.AppendLine(ANSIWords.EQUAL);
            query.AppendLine(id.ToString());
            return query.ToString();
        }

        public string GetFieldValueCommand(object o, List<ConditioningStrut> conditions, string field)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(ANSIWords.SELECT);
            query.AppendLine(field);
            query.AppendLine(ANSIWords.FROM);
            query.AppendLine(AttributeReaderSingleton.AttributeReader.getTableName(o));
            query.AppendLine(ConditioningFactory.Where());
            foreach (ConditioningStrut condition in conditions)
                query.AppendLine(ConditioningFactory.GetCondition(condition));
            return query.ToString();
        }
    }
}
