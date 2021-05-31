using Cabronate.Base;
using Cabronate.Base.Caching;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.DB;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Mount;
using Cabronate.DAO.Operations;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Utils;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.UniversalDAO
{
    public sealed class UniversalDAO<T> : IGetObjectDAO<T>, IPersistence<T>, IDelete<T>
        where T : EcalcValueObject
    {
        private readonly bool _lazy = false;
        private readonly bool _hasCache = true;
        private readonly DBContexto _dbctx;
        private readonly UniversalDAOLog<T> _logDAO;
        private readonly ServiceDAO<T> _serviceDAO;
        private readonly IQueryFactory _query;
        private readonly IMountObject<T> _mount;
        private readonly ReaderControl _dao;
        private readonly bool _logging;
        private readonly bool _loggingData;
        static readonly object padlock = new object();

        public bool ForceID { get; set; }

        public UniversalDAO(DBContexto dbctx)
        {
            _dbctx = dbctx;
            _query = new QueryFactory();
            _mount = new MountObject<T>();
            _dao = new ReaderControl(_dbctx);
            _logDAO = new UniversalDAOLog<T>(_dbctx);
            _serviceDAO = new ServiceDAO<T>(_dbctx);
            _logging = (EcalcConfig.Config.LogDAO == 1);
            _loggingData = (EcalcConfig.Config.DontLogDataDAO == 0);
            ForceID = false;
        }

        public UniversalDAO(DBContexto dbctx, bool lazy)
            : this(dbctx)
        {
            _lazy = lazy;
        }

        public UniversalDAO(DBContexto dbctx, bool lazy, bool hasCache)
            : this(dbctx, lazy)
        {
            _hasCache = hasCache;
        }

        #region Select
        public T GetObject(string script, List<ParameterData> listParameters, T obj = null)
        {
            if (!_mount.hasEmptyConstructor(typeof(T)))
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_CONSTRUCTOR, typeof(T)
                    .ToString()), DateTime.Now);

            if (obj == null)
                obj = Activator.CreateInstance<T>();

            if (obj is EcalcValueObject)
            {
                string sql = script;

                try
                {
                    if (_logging)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        using (IDataReader dr = _dao.executeReader(sql, listParameters))
                        {
                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (SELECT) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(sql);
                                if ((listParameters != null) && (listParameters.Count() > 0))
                                {
                                    sb.AppendLine("Parâmetros:");
                                    listParameters.ForEach(p => sb.AppendLine(string.Format("{0}: {1}", p.name, p.value)));
                                }

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                            return _mount.mountObject(dr, obj, this._lazy, _dbctx);
                        }

                    }
                    else
                    {
                        using (IDataReader dr = _dao.executeReader(sql, listParameters))
                            return _mount.mountObject(dr, obj, this._lazy, _dbctx);
                    }
                }
                catch (Exception e)
                {
                    string erro = $"Erro no UniversalDAO.getObject: {e.Message};";
                    EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {sql}");
                    throw new Exception(erro, e);
                }
            }
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_INHERITANCE, obj.GetType()
                    .ToString()), DateTime.Now);
        }

        public T GetObject(int key)
        {
            if (!_mount.hasEmptyConstructor(typeof(T)))
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_CONSTRUCTOR, typeof(T)
                    .ToString()), DateTime.Now);

            T obj = Activator.CreateInstance<T>();
            if (obj is EcalcValueObject)
            {
                T local = null;
                if ((obj.HasCache) && (_hasCache))
                {
                    local = Clone.DeepClone<T>(
                        Manager.Cache.GetObjCache<T>(
                            CacheManager.TypeCache.Global,
                            CacheManager.MountKey(typeof(T).ToString(), key)));
                }

                if (local != null)
                    return local;
                else
                {
                    string sql = _query.readQuery(obj, false, _dbctx.tipoBanco);
                    try
                    {
                        if (_logging)
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            using (IDataReader dr = _dao.executeReader(sql, AttributeReaderSingleton.AttributeReader.getKeyField(obj), key))
                            {
                                local = _mount.mountObject(dr, obj, _lazy, _dbctx);

                                sw.Stop();
                                long ms = sw.ElapsedMilliseconds;
                                if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                                {
                                    string elapsedTimeText = string.Format("Comando (SELECT) {0}: {1} ms", EcalcLog.commandCount, ms);
                                    StringBuilder sb = new StringBuilder();
                                    sb.AppendLine(elapsedTimeText);
                                    sb.AppendLine(sql);

                                    EcalcLog.logQuerys.Debug(sb.ToString());
                                    EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                                }
                            }
                        }
                        else
                        {
                            using (IDataReader dr = _dao.executeReader(sql, AttributeReaderSingleton.AttributeReader.getKeyField(obj), key))
                                local = _mount.mountObject(dr, obj, _lazy, _dbctx);
                        }

                        if (local == null)
                            throw new RecordNotFoundException(string.Format(ErrorMessages.INVALID_OBJECT_KEY, obj.GetType()
                                .ToString(), key.ToString()), DateTime.Now);
                    }
                    catch (RecordNotFoundException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        string erro = $"Erro no UniversalDAO.getObject: {e.Message};";
                        EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {sql}");
                        throw new Exception(erro, e);
                    }
                }

                if (!_lazy)
                    Manager.Cache.SetObjCache(
                        CacheManager.TypeCache.Global,
                        CacheManager.MountKey(typeof(T).ToString(), key),
                        local);

                local.LazyObject = _lazy;
                return local;
            }
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_INHERITANCE, obj.GetType()
                    .ToString()), DateTime.Now);
        }

        public T GetObject(List<ConditioningStrut> conditioningList = null)
        {
            T obj = Activator.CreateInstance<T>();
            var conditionsCacheKey = string.Empty;
            var mainCacheKey = string.Empty;

            if ((obj.HasCache) && (_hasCache))
            {
                conditionsCacheKey = mountConditionsCacheKey(conditioningList, obj);
                mainCacheKey = Manager.Cache.GetObjCache<string>(CacheManager.TypeCache.Global, conditionsCacheKey);

                if (!string.IsNullOrEmpty(mainCacheKey))
                {
                    T local = Clone.DeepClone<T>(
                        Manager.Cache.GetObjCache<T>(
                            CacheManager.TypeCache.Global, mainCacheKey));

                    if (local != null)
                        return local;
                }
            }

            List<T> list = GetObjects(conditioningList);
            if ((obj.HasCache) && (_hasCache))
            {
                if (list.Count == 1)
                {
                    mainCacheKey = CacheManager.MountKey(
                        typeof(T).ToString(),
                        _serviceDAO.GetKeyFieldValue(list[0]));

                    Manager.Cache.SetObjCache(
                        CacheManager.TypeCache.Global, conditionsCacheKey, mainCacheKey);
                }
            }
            return (list.Count > 0 ? list[0] : Activator.CreateInstance<T>());
        }

        private static string mountConditionsCacheKey(List<ConditioningStrut> conditioningList, T obj)
        {
            string conditionsCacheKey;
            var conditionsKey = new StringBuilder();
            conditioningList.OrderBy(c => c.Field)
                            .ToList()
                            .ForEach(condition =>
                                conditionsKey.Append($"{condition.Field}_{condition.Value}_"));

            conditionsCacheKey = CacheManager.MountKey(
                typeof(T).ToString(),
                conditionsKey.ToString());

            return conditionsCacheKey;
        }

        public List<T> GetObjects(List<ConditioningStrut> conditioningList = null)
        {
            if (!_mount.hasEmptyConstructor(typeof(T)))
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_CONSTRUCTOR, typeof(T)
                    .ToString()), DateTime.Now);

            T obj = Activator.CreateInstance<T>();
            if (obj is EcalcValueObject)
            {
                string sql = _query.readQuery(obj, true, _dbctx.tipoBanco, conditioningList);

                List<T> list = new List<T>();
                try
                {
                    if (_logging)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        using (IDataReader dr = _dao.executeReader(sql, AttributeReaderSingleton.AttributeReader.getKeyField(obj), 0))
                        {
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (SELECT) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(sql);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                    }
                    else
                    {
                        using (IDataReader dr = _dao.executeReader(sql, AttributeReaderSingleton.AttributeReader.getKeyField(obj), 0))
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);
                    }

                    if (!_lazy)
                    {
                        foreach (T item in list)
                        {
                            if ((item.HasCache) && (_hasCache))
                            {
                                string cacheKey =
                                CacheManager.MountKey(
                                    typeof(T).ToString(),
                                    _serviceDAO.GetKeyFieldValue(item));

                                Manager.Cache.SetObjCache(
                                    CacheManager.TypeCache.Global, cacheKey, item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string erro = $"Erro no UniversalDAO.getObjects: {e.Message};";
                    EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {sql}");
                    throw new Exception(erro, e);
                }

                return list;
            }
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_INHERITANCE, obj.GetType()
                    .ToString()), DateTime.Now);
        }

        public List<T> GetObjects(string script, List<ParameterData> listParameters)
        {
            if (!_mount.hasEmptyConstructor(typeof(T)))
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_CONSTRUCTOR, typeof(T)
                    .ToString()), DateTime.Now);

            T obj = Activator.CreateInstance<T>();
            if (obj is EcalcValueObject)
            {
                string sql = script;

                List<T> list = new List<T>();

                try
                {
                    if (_logging)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        using (IDataReader dr = _dao.executeReader(sql, listParameters))
                        {
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);
                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (SELECT) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(sql);
                                if ((listParameters != null) && (listParameters.Count() > 0))
                                {
                                    sb.AppendLine("Parâmetros:");
                                    listParameters.ForEach(p => sb.AppendLine(string.Format("{0}: {1}", p.name, p.value)));
                                }
                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                    }
                    else
                    {
                        using (IDataReader dr = _dao.executeReader(sql, listParameters))
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);
                    }
                }
                catch (Exception e)
                {
                    string erro = $"Erro no UniversalDAO.getObject: {e.Message};";
                    EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {sql}");
                    throw new Exception(erro, e);
                }

                return list;
            }
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_INHERITANCE, obj.GetType()
                    .ToString()), DateTime.Now);
        }

        public List<T> GetObjects(IDbCommand command)
        {
            if (!_mount.hasEmptyConstructor(typeof(T)))
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_CONSTRUCTOR, typeof(T)
                    .ToString()), DateTime.Now);

            T obj = Activator.CreateInstance<T>();
            if (obj is EcalcValueObject)
            {
                List<T> list = new List<T>();

                try
                {
                    if (_logging)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        using (IDataReader dr = _dao.executeCommand(command))
                        {
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (SELECT) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(command.CommandText);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                    }
                    else
                    {
                        using (IDataReader dr = _dao.executeCommand(command))
                            list = _mount.mountObjects(dr, obj, _lazy, _dbctx);
                    }
                }
                catch (Exception e)
                {
                    string erro = $"Erro no UniversalDAO.getObject: {e.Message};";
                    EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {command.CommandText}");
                    throw new Exception(erro, e);
                }

                return list;
            }
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_INHERITANCE, obj.GetType()
                    .ToString()), DateTime.Now);
        }
        #endregion

        #region Save
        public int SaveObject(T obj, List<string> fields = null, CommandCompiled cmdCompiled = null, bool? novo = null)
        {
            ListaErros erros = new ListaErros();
            DBContexto dbctx = _dbctx;
            int result;

            int valueID = _serviceDAO.GetKeyFieldValue(obj);

            bool isnew;

            if ((novo != null) && (novo.Value))
                isnew = true;
            else
                isnew = (valueID > 0 ? false : true);

            bool comandoCriadoLocal = false;

            CascadeValidator.ValidadeFields(dbctx, obj);

            // se não estamos usando o cache
            if (cmdCompiled == null)
            {
                comandoCriadoLocal = true;
                cmdCompiled = new CommandCompiled();
                if (isnew)
                    CompileInsertCommand(obj, cmdCompiled, fields);
                else
                    CompileUpdateCommand(obj, cmdCompiled, fields);
            }

            IDbCommand command;
            if (isnew)
                command = cmdCompiled.InsertCommand;
            else
                command = cmdCompiled.UpdateCommand;

            try
            {
                lock (padlock)
                {
                    if ((isnew) && (valueID <= 0))
                    {
                        if (AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).keyFieldInInsert())
                        {
                            valueID = _serviceDAO.GetNewID(obj);
                            obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj))
                                .SetValue(obj, valueID, new object[0]);
                        }
                    }
                    _serviceDAO.SetParams(obj, command, cmdCompiled.ObjectMapper, isnew, fields);

                    try
                    {
                        if ((!isnew) && (_loggingData))
                            _logDAO.SaveChangesInLog(obj);

                        if (_logging)
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            result = command.ExecuteNonQuery();

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando {0} {1}: {2} ms", (isnew ? "(INSERT)" : "(UPDATE)"), EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(command.CommandText);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                        else
                            result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (command.Parameters != null)
                        {
                            sb.AppendLine("Parâmetros:");
                            foreach (IDataParameter prm in command.Parameters)
                            {
                                sb.AppendLine(string.Format("{0}: {1}", prm.ParameterName, prm.Value));
                            }
                        }

                        string erro = $"Erro no UniversalDAO.saveObject: {e.Message};";
                        EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {command.CommandText}\n{sb.ToString()}");
                        throw new Exception(erro, e);
                    }

                    if (!AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).keyFieldInInsert() && (isnew))
                    {
                        valueID = _serviceDAO.GetID(obj);
                        obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj)).
                            SetValue(obj, valueID, null);
                    }

                    if ((obj is IPersistChildren) && (!_lazy))
                    {
                        erros.AddErrors((obj as IPersistChildren).SaveChildren(valueID, _dbctx));
                        if (erros.HasAnyError)
                        {
                            throw new Exception(erros.ErrorMsg());
                        }
                    }

                    _logDAO.SaveFlowLog(obj);

                    if (!obj.LazyObject)
                        Manager.Cache.SetObjCache(
                            CacheManager.TypeCache.Global,
                            CacheManager.MountKey(typeof(T).ToString(), valueID), obj);
                    else
                        Manager.Cache.RemoveObjCache(
                            CacheManager.TypeCache.Global,
                            CacheManager.MountKey(typeof(T).ToString(), valueID));
                }
            }
            finally
            {
                if (comandoCriadoLocal)
                    command.Dispose();
            }
            return result;
        }

        public int SaveObjects(List<T> objs, List<string> fields = null)
        {
            if (objs.Count == 0)
                return 0;

            DBContexto dbctx = _dbctx;
            try
            {
                int result = 0;
                CommandCompiled cmdCompiled = new CommandCompiled();
                try
                {
                    CompileCommands(objs[0], cmdCompiled, fields);

                    objs.ForEach(obj => result += SaveObject(obj, fields, cmdCompiled));

                }
                finally
                {
                    cmdCompiled.DisposeAll();
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int SaveObjectWithControlField(T obj, SQLControlFields ControlFields = null, CommandCompiled cmdCompiled = null)
        {
            ListaErros erros = new ListaErros();
            DBContexto dbctx = _dbctx;
            int result;

            int valueID = _serviceDAO.GetKeyFieldValue(obj);
            bool isnew = (valueID > 0 ? false : true);
            bool comandoCriadoLocal = false;

            CascadeValidator.ValidadeFields(dbctx, obj);

            List<String> fieldsInsert = null;
            List<String> fieldsUpdate = null;
            if (ControlFields != null)
            {
                fieldsInsert = (ControlFields.FieldsInsert.Count > 0) ? ControlFields.FieldsInsert : null;
                fieldsUpdate = (ControlFields.FieldsUpdate.Count > 0) ? ControlFields.FieldsUpdate : null;
            }

            // se não estamos usando o cache
            if (cmdCompiled == null)
            {
                comandoCriadoLocal = true;
                cmdCompiled = new CommandCompiled();
                if (isnew)
                    CompileInsertCommand(obj, cmdCompiled, fieldsInsert);
                else
                    CompileUpdateCommand(obj, cmdCompiled, fieldsUpdate);
            }

            IDbCommand command;
            if (isnew)
                command = cmdCompiled.InsertCommand;
            else
                command = cmdCompiled.UpdateCommand;

            try
            {
                lock (padlock)
                {
                    if (isnew)
                    {
                        if (AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).keyFieldInInsert())
                        {
                            valueID = _serviceDAO.GetNewID(obj);
                            obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj))
                                .SetValue(obj, valueID, new object[0]);
                        }
                    }
                    if (isnew)
                        _serviceDAO.SetParams(obj, command, cmdCompiled.ObjectMapper, isnew, fieldsInsert);
                    else
                        _serviceDAO.SetParams(obj, command, cmdCompiled.ObjectMapper, isnew, fieldsUpdate);

                    try
                    {
                        if ((!isnew) && (_loggingData))
                            _logDAO.SaveChangesInLog(obj);

                        if (_logging)
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            result = command.ExecuteNonQuery();

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando {0} {1}: {2} ms", (isnew ? "(INSERT)" : "(UPDATE)"), EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(command.CommandText);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                        else
                            result = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (command.Parameters != null)
                        {
                            sb.AppendLine("Parâmetros:");
                            foreach (IDataParameter prm in command.Parameters)
                            {
                                sb.AppendLine(string.Format("{0}: {1}", prm.ParameterName, prm.Value));
                            }
                        }
                        string erro = $"Erro no UniversalDAO.saveObject: {e.Message};";
                        EcalcLog.LogError<GenericOP>(e, $"{erro}\n sql = {command.CommandText}\n{sb.ToString()}");
                        throw new Exception(erro, e);

                    }

                    if (!AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).keyFieldInInsert() && (isnew))
                    {
                        valueID = _serviceDAO.GetID(obj);
                        obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj)).
                            SetValue(obj, valueID, null);
                    }

                    if ((obj is IPersistChildren) && (!_lazy))
                    {
                        erros.AddErrors((obj as IPersistChildren).SaveChildren(valueID, _dbctx));
                        if (erros.HasAnyError)
                            throw new Exception(erros.ErrorMsg());
                    }

                    _logDAO.SaveFlowLog(obj);

                    if (!obj.LazyObject)
                        Manager.Cache.SetObjCache(
                            CacheManager.TypeCache.Global,
                            CacheManager.MountKey(typeof(T).ToString(), valueID), obj);
                    else
                        Manager.Cache.RemoveObjCache(
                            CacheManager.TypeCache.Global,
                            CacheManager.MountKey(typeof(T).ToString(), valueID));
                }
            }
            finally
            {
                if (comandoCriadoLocal)
                    command.Dispose();
            }

            return result;
        }

        public int SaveObjectsWithControlFields(List<T> objs, SQLControlFields ControlFields = null)
        {
            if (objs.Count == 0)
                return 0;

            DBContexto dbctx = _dbctx;
            int result = 0;
            CommandCompiled cmdCompiled = new CommandCompiled();
            try
            {
                CompileCommands(objs[0], cmdCompiled, ControlFields);

                objs.ForEach(obj => result += SaveObjectWithControlField(obj, ControlFields, cmdCompiled));

            }
            finally
            {
                cmdCompiled.DisposeAll();
            }
            return result;
        }
        #endregion

        #region Delete
        public void DeleteObject(T obj)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                using (IDbCommand command = dbctx.CreateCommand(""))
                {
                    if (obj is IDeleteChildren)
                    {
                        ListaErros erros = (obj as IDeleteChildren).DeleteChildren(_dbctx);

                        if (erros.HasAnyError)
                        {
                            throw new Exception(erros.ToString());
                        }
                    }

                    command.Connection = _dbctx.GetConnection;
                    command.CommandText = _query.deleteQuery(obj, _dbctx.tipoBanco);


                    string propertyKeyField = AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj);
                    string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(obj);

                    int ID = Convert.ToInt32(obj.GetType().GetProperty(propertyKeyField).GetValue(obj, new object[0]));
                    _dbctx.CreateParameter(command, keyField, ID);
                    try
                    {
                        if (_logging)
                        {
                            Stopwatch sw = Stopwatch.StartNew();

                            obj.Erros.AddErrors(CascadeValidator.Validate(dbctx, obj, ID));
                            if (!obj.Erros.HasAnyError)
                                command.ExecuteNonQuery();

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (DELETE) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(command.CommandText);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                        else
                        {
                            obj.Erros.AddErrors(CascadeValidator.Validate(dbctx, obj, ID));
                            if (!obj.Erros.HasAnyError)
                                command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        var erro = $"Erro no UniversalDAO.deleteObject: {e.Message};";
                        EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {command.CommandText}");
                        throw new Exception(erro, e);
                    }
                }

                if (!obj.Erros.HasAnyError)
                {
                    Manager.Cache.RemoveObjCache(
                        CacheManager.TypeCache.Global,
                        CacheManager.MountKey(typeof(T).ToString(), _serviceDAO.GetKeyFieldValue(obj)));

                    if (_loggingData)
                        _logDAO.SaveDeleteInLog(obj);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteObjects(List<T> list)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                list.ForEach(DeleteObject);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteObjects(string script)
        {
            _dao.executeNonQuery(script);
        }
        #endregion

        #region Commands
        public void Lock(T obj)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                using (IDbCommand command = dbctx.CreateCommand(""))
                {

                    command.Connection = _dbctx.GetConnection;
                    command.CommandText = _query.getLockQuery(obj);

                    string propertyKeyField = AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj);
                    string keyField = AttributeReaderSingleton.AttributeReader.getKeyField(obj);

                    int ID = Convert.ToInt32(obj.GetType().GetProperty(propertyKeyField).GetValue(obj, new object[0]));
                    _dbctx.CreateParameter(command, keyField, ID);
                    try
                    {
                        if (_logging)
                        {
                            Stopwatch sw = Stopwatch.StartNew();

                            if (!obj.Erros.HasAnyError)
                                command.ExecuteNonQuery();

                            sw.Stop();
                            long ms = sw.ElapsedMilliseconds;
                            if (ms > EcalcConfig.Config.LogDAOMinimumMilliseconds)
                            {
                                string elapsedTimeText = string.Format("Comando (UPDATE) {0}: {1} ms", EcalcLog.commandCount, ms);
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine(elapsedTimeText);
                                sb.AppendLine(command.CommandText);

                                EcalcLog.logQuerys.Debug(sb.ToString());
                                EcalcLog.logQuerysTime.Debug(elapsedTimeText);
                            }
                        }
                        else
                        {
                            if (!obj.Erros.HasAnyError)
                                command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        string erro = $"Erro no UniversalDAO.LockObject: {e.Message};";
                        EcalcLog.LogError<UniversalDAO<T>>(e, $"{erro}\n sql = {command.CommandText}");
                        throw new Exception(erro, e);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public int Count(List<ConditioningStrut> conditions)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                T obj = Activator.CreateInstance<T>();
                IDbCommand command = dbctx.CreateCommand(new QueryFactory().CountCommand(obj, conditions));
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception e)
            {
                string erro = $"Erro no UniversalDAO.Count: {e.Message};";
                EcalcLog.LogError<UniversalDAO<T>>(erro);
                throw e;
            }
        }


        public int Max(List<ConditioningStrut> conditions, string nomeColuna)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                T obj = Activator.CreateInstance<T>();
                IDbCommand command = dbctx.CreateCommand(new QueryFactory().MaxCommand(obj, conditions, nomeColuna));
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch (Exception e)
            {
                string erro = $"Erro no UniversalDAO.Max: {e.Message};";
                EcalcLog.LogError<UniversalDAO<T>>(erro);
                throw e;
            }
        }

        public string GetFieldValueCommand(int id, string field)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                T obj = Activator.CreateInstance<T>();
                IDbCommand command = dbctx.CreateCommand(new QueryFactory().GetFieldValueCommand(obj, id, field));
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                return Convert.ToString(dt.Rows[0][0]);
            }
            catch (Exception e)
            {
                string erro = $"Erro no UniversalDAO.GetFieldValueCommand: {e.Message};";
                EcalcLog.LogError<UniversalDAO<T>>(erro);
                throw e;
            }
        }

        public string GetFieldValueCommand(List<ConditioningStrut> conditions, string field)
        {
            DBContexto dbctx = _dbctx;
            try
            {
                T obj = Activator.CreateInstance<T>();
                IDbCommand command = dbctx.CreateCommand(new QueryFactory().GetFieldValueCommand(obj, conditions, field));
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());

                if (dt.Rows.Count > 1)
                    throw new Exception("Error: Query with more than one result");
                else if (dt.Rows.Count == 0)
                    return "";
                else
                    return Convert.ToString(dt.Rows[0][0]);
            }
            catch (Exception e)
            {
                string erro = $"Erro no UniversalDAO.GetFieldValueCommand: {e.Message};";
                EcalcLog.LogError<UniversalDAO<T>>(erro);
                throw e;
            }
        }
        #endregion

        #region CompiledCommand
        public void CompileInsertCommand(T obj, CommandCompiled cmdCompiled, List<string> fields = null)
        {
            IQueryFactory queryLocal = new QueryFactory();
            cmdCompiled.InsertCommand = _dbctx.CreateCommand("", 10);
            cmdCompiled.InsertCommand.Connection = _dbctx.GetConnection;
            cmdCompiled.InsertCommand.CommandText = queryLocal.createQuery(obj, _dbctx.Provider, _dbctx.tipoBanco, fields, ForceID);
            cmdCompiled.InsertCommand.Prepare();
            if (cmdCompiled.ObjectMapper == null)
            {
                cmdCompiled.ObjectMapper = new MountObjectMapper();
                cmdCompiled.ObjectMapper.BuildTypeForSave(obj.GetType(), fields, true);
                cmdCompiled.ObjectMapper.BuildParamsInsert(_dbctx, cmdCompiled.InsertCommand, ForceID);
            }
        }

        public void CompileUpdateCommand(T obj, CommandCompiled cmdCompiled, List<string> fields = null)
        {
            IQueryFactory queryLocal = new QueryFactory();
            cmdCompiled.UpdateCommand = _dbctx.CreateCommand("", 10);
            cmdCompiled.UpdateCommand.Connection = _dbctx.GetConnection;
            cmdCompiled.UpdateCommand.CommandText = _query.updatetQuery(obj, _dbctx.tipoBanco, fields);
            cmdCompiled.UpdateCommand.Prepare();
            if (cmdCompiled.ObjectMapper == null)
            {
                cmdCompiled.ObjectMapper = new MountObjectMapper();
                cmdCompiled.ObjectMapper.BuildTypeForSave(obj.GetType(), fields, false);
                cmdCompiled.ObjectMapper.BuildParamsUpdate(_dbctx, cmdCompiled.UpdateCommand);

            }
        }

        public void CompileCommands(T obj, CommandCompiled cmdCompiled, List<string> fields = null)
        {
            cmdCompiled.ObjectMapper = new MountObjectMapper();
            // Mapear para Insert
            cmdCompiled.ObjectMapper.BuildTypeForSave(obj.GetType(), fields, true);
            // E copiar mapeamento para o update
            cmdCompiled.ObjectMapper.FieldsUpdate = new List<MountObjectMapperField>();
            cmdCompiled.ObjectMapper.FieldsUpdate.AddRange(cmdCompiled.ObjectMapper.FieldsInsert);

            CompileInsertCommand(obj, cmdCompiled, fields);
            CompileUpdateCommand(obj, cmdCompiled, fields);

            cmdCompiled.ObjectMapper.BuildParamsInsert(_dbctx, cmdCompiled.InsertCommand, ForceID);
            cmdCompiled.ObjectMapper.BuildParamsUpdate(_dbctx, cmdCompiled.UpdateCommand);
        }

        public void CompileCommands(T obj, CommandCompiled cmdCompiled, SQLControlFields Controlfields = null)
        {
            List<String> fieldsInsert = null;
            List<String> fieldsUpdate = null;
            cmdCompiled.ObjectMapper = new MountObjectMapper();
            if (Controlfields != null)
            {
                fieldsInsert = (Controlfields.FieldsInsert.Count > 0) ? Controlfields.FieldsInsert : null;
                fieldsUpdate = (Controlfields.FieldsUpdate.Count > 0) ? Controlfields.FieldsUpdate : null;
            }
            cmdCompiled.ObjectMapper.BuildTypeForSave(obj.GetType(), fieldsInsert, true);
            cmdCompiled.ObjectMapper.BuildTypeForSave(obj.GetType(), fieldsUpdate, false);

            CompileInsertCommand(obj, cmdCompiled, fieldsInsert);
            CompileUpdateCommand(obj, cmdCompiled, fieldsUpdate);

            cmdCompiled.ObjectMapper.BuildParamsInsert(_dbctx, cmdCompiled.InsertCommand, ForceID);
            cmdCompiled.ObjectMapper.BuildParamsUpdate(_dbctx, cmdCompiled.UpdateCommand);
        }
        #endregion
    }
}
