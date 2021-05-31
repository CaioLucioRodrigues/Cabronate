using System;
using System.Collections.Generic;
using System.Text;

namespace Cabronate.DAO.QueryBuilder.Statements
{
    public class Select
    {
        public List<Column> Columns { get; }
        public Table TableFrom { get; }
        public bool Distinct { get; set; }

        public Select()
        {
            this.TableFrom = new Table();
            this.Columns = new List<Column>();
        }

        public Select(Table from)
        {
            this.TableFrom = from;
            this.Columns = new List<Column>();
        }
    }
}
