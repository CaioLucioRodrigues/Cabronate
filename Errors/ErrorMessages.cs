namespace Cabronate.DAO.Errors
{
    public class ErrorMessages
    {
        public const string INVALID_VO_TABLE_NAME = "Value Object {0} não obedece as especificações mínimas, nome da tabela não foi identificado";
        public const string INVALID_VO_PRIMARY_KEY = "Value Object {0} não obedece as especificações mínimas, primary key não foi identificada";
        public const string INVALID_VO_FIELD_NAMEPK = "Value Object {0} não obedece as especificações mínimas, nome do campo PK não foi identificado";
        public const string INVALID_VO_SECOND_FIELD = "Value Object {0} não obedece as especificações mínimas, SEGUNDO campo não foi identificado";
        public const string INVALID_VO_SECOND_FIELD_NAME = "Value Object {0} não obedece as especificações mínimas, nome do SEGUNDO campo não foi identificado";
        public const string INVALID_VO_FIELD_NAME = "Value Object {0} não obedece as especificações mínimas, nenhum campo foi identificado";
        public const string INVALID_KEY_PROPERTY = "Value Object {0} não obedece as especificações mínimas, a propriedade referente a chave primária não pode ser lida";
        public const string INVALID_KEY_TYPE = "Value Object {0} não obedece as especificações mínimas, a propriedade referente a chave primária deve ser um inteiro";
        public const string INVALID_INHERITANCE = "Value Object {0} não obedece as especificações mínimas, a classe deve herdar do BaseVO";
        public const string INVALID_VO_CONSTRUCTOR = "Value Object {0} não obedece as especificações mínimas, a classe deve possuir um constructor sem argumentos";
        public const string INVALID_OBJECT_KEY = "Objeto {0}, chave {1}, não existe no banco de dados";
        public const string PROPERTY_ERROR = "Erro na propriedade {0} da classe {1}";
        public const string PROPERTY_CLUSTERED_ERROR = "Erro na propriedade {0} da classe {1} [Carregando agrupado]";
        public const string PROVIDER_ERROR = "Não foi possível localizar o provider utilizado";
        public const string DB_PACKAGE_ERROR = "Banco logado não suporta itens do tipo Package";
        public const string DB_UDF_ERROR = "Banco logado não suporta itens do tipo UDF";
        public const string DB_MISS_PROCEDURE = "A Procedure {0} não existe no banco logado";
        public const string DB_MISS_VIEW = "A View {0} não existe no banco logado";
        public const string DB_MISS_PACKAGE = "A Package {0} não existe no banco logado";
        public const string DB_MISS_UDF = "A UDF {0} não existe no banco logado";
        public const string DB_MISS_FIELD = "O Campo {0} não existe na Tabela {1} do banco logado";
        public const string DB_MISS_TABLE = "A Tabela {0} não existe no banco logado";
        public const string INVALID_DB_ITEMS = "Não foi possível identificar o item solicitado ({0})";
        public const string INVALID_RANGE_GREATER_ERROR = "Valor {0} setado não é maior que o valor {2} (GreaterThanAttribute)";
        public const string ATTRIB_VALUEDB_MISS = "Atributo [ValueOnDataBase] não preenchido para o item ({0}) do Enum ({1})";
    }
}
