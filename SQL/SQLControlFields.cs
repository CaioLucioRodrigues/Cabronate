using System;
using System.Collections.Generic;
using System.Linq;

namespace Cabronate.DAO.SQL
{
    public class SQLControlFields
    {
        public List<String> FieldsUpdate;
        public List<String> FieldsInsert;
        public List<String> FieldsGeneric
        {
            get
            {
                return FieldsInsert.Union(FieldsUpdate).ToList();
            }
        }

        public SQLControlFields()
        {
            this.FieldsInsert = new List<String>();
            this.FieldsUpdate = new List<String>();
        }
    }


}
