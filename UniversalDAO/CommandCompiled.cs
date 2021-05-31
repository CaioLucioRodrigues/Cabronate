using Cabronate.DAO.Mount;
using System.Data;

namespace Cabronate.DAO.UniversalDAO
{
    public class CommandCompiled
    {
        public IDbCommand UpdateCommand { get; set; }
        public IDbCommand InsertCommand { get; set; }

        public MountObjectMapper ObjectMapper { get; set; }

        internal void DisposeAll()
        {
            if (UpdateCommand != null)
            {
                UpdateCommand.Dispose();
            }

            if (InsertCommand != null)
            {
                InsertCommand.Dispose();
            }
        }
    }
}
