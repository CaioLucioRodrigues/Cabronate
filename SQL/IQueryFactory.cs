using Cabronate.Base;
using System;
using System.Collections.Generic;

namespace Cabronate.DAO.SQL
{
    /// <summary>
    /// Interface que define as montagens das querys CRUD a partir dos atributos dos Values Objects
    /// </summary>
    public interface IQueryFactory
    {
        // CRUD
        /// <summary>
        /// Método que monta uma query de criação de um novo registro
        /// </summary>
        /// <param name="o">Objeto que deve ser persistido no banco</param>
        /// <returns>Query 'insert' montada</returns>
        string createQuery(Object o, string provider, TipoBanco tipoBanco, List<string> fieldss = null, bool forcarID = false);

        /// <summary>
        /// Método que monta uma query de leitura de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser lido no banco</param>
        /// <returns>Query 'select' montada</returns>
        string readQuery(Object o, bool list, TipoBanco tipoBanco, List<ConditioningStrut> conditionList = null);

        /// <summary>
        /// Método que monta uma query de atualização de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser atualizado no banco</param>
        /// <returns>Query 'update' montada</returns>
        string updatetQuery(Object o, TipoBanco tipoBanco, List<string> fields = null);

        /// <summary>
        /// Método que monta uma query de deleção de um registro
        /// </summary>
        /// <param name="o">Objeto que deve ser deletado no banco</param>
        /// <returns>Query 'delete' montada</returns>
        string deleteQuery(Object o, TipoBanco tipoBanco);

        /// <summary>
        /// Método que monta uma query de count através de alguns parametros
        /// </summary>
        /// <param name="o">Objeto principal da consulta, é nesse objeto que faremos o count</param>
        /// <param name="conditions">Condições para o count</param>
        /// <returns></returns>
        string CountCommand(Object o, List<ConditioningStrut> conditions);

        /// <summary>
        /// Método que retorna o valor de um campo convertido para string
        /// </summary>
        /// <param name="o">Objeto principal da consulta, é nesse objeto que montaremos a consulta</param>        
        /// <param name="field">Campo a ser carregado</param>
        /// <param name="id">Chave da entidade no banco a ser consumida</param>
        /// <returns></returns>
        string GetFieldValueCommand(Object o, int id, string field);

        //Utils
        /// <summary>
        /// Método que monta a query responsável por trazer o próximo ID da tabela
        /// </summary>
        /// <param name="o">Objeto a ser persistido</param>
        /// <returns>Próximo ID da tabela</returns>
        string getNextIDQuery(Object o);

        /// <summary>
        /// sjdffsdjksfdsfjkda sfdnjklsfdajk
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        string getLockQuery(object o);
    }
}
