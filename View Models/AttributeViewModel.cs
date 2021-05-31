using System;

namespace Cabronate.DAO.View_Models
{
    [Serializable]
    public class AttributeViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AttributeId { get; set; }        

        public string Answer { get; set; }

        public int AnswerId { get; set; }

        public string Observation { get; set; }
    }
}
