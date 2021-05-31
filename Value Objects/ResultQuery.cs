using Cabronate.Base;
using Cabronate.DAO.Attributes;

namespace Cabronate.DAO.Value_Objects
{
    public class ResultQuery : EcalcValueObject
    {
        [FieldName("result")]
        public object resultQuery { get; set; }
    }

    public class ResultQueryChaveValor : EcalcValueObject
    {
        [FieldName("chave")]
        public object Chave { get; set; }

        [FieldName("valor")]
        public object Valor { get; set; }
    }


    public class ResultQueryWithType : EcalcValueObject
    {
        [FieldName("result")]
        public object resultQuery { get; set; }

        [FieldName("type")]
        public int type { get; set; }
    }

    public class ResultQueryJson : EcalcValueObject
    {
        [FieldName("jsonKey")]
        public string key { get; set; }

        [FieldName("jsonValue")]
        public string value { get; set; }
    }
}
