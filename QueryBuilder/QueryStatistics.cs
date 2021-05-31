using Cabronate.Base.Caching;
using Cabronate.Base.Interfaces;
using System;

namespace Cabronate.DAO.QueryBuilder
{
    public class QueryStatistics : ISelfManagingCacheObject
    {
        public int Value { get; set; }        

        public QueryStatistics(string sessionKey)
        {   
            WGlobal.Set(sessionKey, this);
        }

        public double GetTimeOutInMinutes()
        {
            return 5;
        }
    }
}
