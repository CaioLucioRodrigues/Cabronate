namespace Cabronate.DAO.ReportBuilder.Types
{
    public enum Operation
    {
        Undefined = 0,
        Sum = 1,
        Max = 2,
        Min = 3,
        Count = 4,
        Average = 5,
        Formula = 6
    }

    public static class OperationConverter
    {
        public static string ToString(Operation operation)
        {
            switch (operation)
            {
                case Operation.Undefined:
                    return "";
                case Operation.Sum:
                    return "Soma";
                case Operation.Max:
                    return "Valor Máximo";
                case Operation.Min:
                    return "Valor Mínimo";
                case Operation.Count:
                    return "Contagem";
                case Operation.Average:
                    return "Média";
                case Operation.Formula:
                    return "Fórmula";
                default:
                    return "";
            }
        }
    }
}
