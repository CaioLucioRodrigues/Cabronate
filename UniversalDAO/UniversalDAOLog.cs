using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Business_Objects;
using Cabronate.DAO.Operations;
using Cabronate.DAO.Utils;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cabronate.DAO.UniversalDAO
{
    public class UniversalDAOLog<T>
        where T : EcalcValueObject
    {
        private readonly DBContexto _dbctx;
        private readonly ServiceDAO<T> _logService;

        public UniversalDAOLog(DBContexto dbctx)
        {
            _dbctx = dbctx;
            _logService = new ServiceDAO<T>(dbctx);
        }

        public void SaveDeleteInLog(T obj)
        {
            try
            {
                if (AttributeReaderSingleton.AttributeReader.getTableLogable(obj))
                {
                    LogOperationBO reg = CreateNewLog(obj, LogOperationBO.TipoOperacaoLog.Exclusao);                    
                    GenericOP.Salvar(reg, _dbctx);
                }
            }
            catch (Exception e)
            {
                EcalcLog.LogError<UniversalDAOLog<T>>(e, $"Erro no UniversalDAO.saveDeleteInLog: {e.Message}");                
                throw e;
            }
        }

        public void SaveChangesInLog(T obj)
        {
            try
            {
                if (AttributeReaderSingleton.AttributeReader.getTableLogable(obj))
                {
                    using (DBContexto dbctxLocal = DBContexto.getDbctx().Open().WithTransaction())
                    {
                        LogOperationBO reg = CreateNewLog(obj, LogOperationBO.TipoOperacaoLog.Alteracao);

                        UniversalDAO<T> DAOLocal = new UniversalDAO<T>(dbctxLocal, true, false);
                        T local = DAOLocal.GetObject(_logService.GetKeyFieldValue(obj));

                        if (_logService.GetKeyFieldValue(local) > 0)
                        {
                            Array.ForEach(typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), prop =>
                            {
                                object value = prop.GetValue(obj, null);
                                if (!(value is EcalcValueObject) && (AttributeReaderSingleton.AttributeReader.getDisplayFieldName(prop) != ""))
                                {
                                    object valor = prop.GetValue(obj, null);
                                    string valorNovo = (valor == null) ? "" : valor.ToString();
                                    valor = prop.GetValue(local, null);
                                    string valorAnterior = (valor == null) ? "" : valor.ToString();

                                    if (valorAnterior != valorNovo)
                                    {
                                        reg.Detalhes.Add(new LogOperationDetailBO()
                                        {
                                            ValorAntigo = valorAnterior,
                                            ValorNovo = valorNovo,
                                            Campo = AttributeReaderSingleton.AttributeReader.getDisplayFieldName(prop)
                                        });
                                    }
                                }
                            });
                            GenericOP.Salvar(reg, _dbctx);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EcalcLog.LogError<UniversalDAOLog<T>>(e, $"Erro no UniversalDAO.saveChangesInLog: {e.Message}");
                throw e;
            }
        }

        public void SaveFlowLog(T obj)
        {
            if (obj.LogFlowBO.Lista.Count > 0)
            {
                var fluxosLog = new List<LogFlowBO>();
                obj.LogFlowBO.Lista.ForEach(fluxo =>
                {
                    fluxosLog.Add(new LogFlowBO(fluxo.Campo, fluxo.Mensagem, fluxo.Data)
                    {
                        CodigoTabelaOrigem = _logService.GetKeyFieldValue(obj),
                        CodigoUsuario = _dbctx.UserID,
                        Tabela = AttributeReaderSingleton.AttributeReader.getDescriptionTableName(obj)
                    });
                });
                GenericOP.Salvar(fluxosLog, _dbctx);
            }
        }

        private LogOperationBO CreateNewLog(T obj, LogOperationBO.TipoOperacaoLog tipoOperacao)
        {
            int Id = (obj as EcalcValueObjectDAO).KeyFieldValue;
            return new LogOperationBO
            {
                Entidade = AttributeReaderSingleton.AttributeReader.getDescriptionTableName(obj),
                EntidadeID = Id,
                TipoOperacao = tipoOperacao,
                Data = DateTime.Now,
                Usuario = _dbctx.UserID,
                Versao = _dbctx.versao
            };
        }
    }
}
