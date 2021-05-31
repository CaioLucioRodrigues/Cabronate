namespace Cabronate.DAO.ReportBuilder.Types
{
    public enum MathematicalSign
    {
        Undefined = 0,
        Addition = 1,
        Subtraction = 2,
        Multiplication = 3,
        Division = 4
    }

    public static class MathematicalSignsConverter
    {
        public static string ToString(MathematicalSign operation)
        {
            switch (operation)
            {
                case MathematicalSign.Undefined:
                    return "";
                case MathematicalSign.Addition:
                    return "+";
                case MathematicalSign.Subtraction:
                    return "-";
                case MathematicalSign.Multiplication:
                    return "*";
                case MathematicalSign.Division:
                    return "/";                
                default:
                    return "";
            }
        }

        public static string ToName(MathematicalSign operation)
        {
            switch (operation)
            {
                case MathematicalSign.Undefined:
                    return "";
                case MathematicalSign.Addition:
                    return "Soma";
                case MathematicalSign.Subtraction:
                    return "Subtrai";
                case MathematicalSign.Multiplication:
                    return "Multiplica";
                case MathematicalSign.Division:
                    return "Divide";
                default:
                    return "";
            }
        }
    }
}
