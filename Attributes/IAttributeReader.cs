using System;
using System.Collections.Generic;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Interface que define o contrato para ler os atributos de VO's previamente preparados para o UniversalDAO
    /// </summary>
    interface IAttributeReader
    {
        /// <summary>
        /// Método usado para ler o nome da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a tabela referente </param>
        /// <returns>Nome da tabela responsável no banco</returns>
        string getTableName(Object o);

        /// <summary>
        /// Método usado para ler a primary key da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a primary key referente </param>
        /// <returns>Nome do campo que é primary key responsável na tabela</returns>
        string getKeyField(Object o);

        /// <summary>
        /// Método usado para retornar a property respectiva da primery key
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a propriedade referente a  primary key</param>
        /// <returns>Nome da propriedade referente a primary key responsável na tabela</returns>
        string getPropertyKeyField(Object o);

        /// <summary>
        /// Método usado para ler os campos da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber os campos referentes </param>
        /// <returns>Nome os campos que representam as propriedades da nossa classe</returns>
        List<string> getFieldsName(Object o, List<string> fields = null);
    }
}
