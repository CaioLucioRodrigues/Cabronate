using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cabronate.DAO.Utils
{
    /// <summary>
    /// Classe feita para facilitar a manipulação de strings
    /// </summary>
    public sealed class StringUtils
    {
        /// <summary>
        /// Converte uma lista de strings para uma string unica, cujos elementos estão separados por vírgula
        /// </summary>
        /// <param name="list">Lista de strings</param>
        /// <returns>Elementos separados por vírgula</returns>
        public static string ListToString(List<string> list, string concate = "")
        {
            string result = string.Empty;
            for (int i = 0; i < list.Count(); i++)
            {
                result = result + ", " + concate + list[i];
            }
            result = result.Remove(0, (concate.Length + 1));
            return result;
        }

        public static string Normalizar(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();

            foreach (char letter in arrayText)
            {
                switch (CharUnicodeInfo.GetUnicodeCategory(letter))
                {
                    case UnicodeCategory.NonSpacingMark:
                        continue;
                    case UnicodeCategory.SpaceSeparator:
                        sbReturn.Append("_");
                        break;
                    case UnicodeCategory.ClosePunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.Control:
                    case UnicodeCategory.CurrencySymbol:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.EnclosingMark:
                    case UnicodeCategory.FinalQuotePunctuation:
                    case UnicodeCategory.Format:
                    case UnicodeCategory.InitialQuotePunctuation:
                    case UnicodeCategory.LetterNumber:
                    case UnicodeCategory.LineSeparator:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.MathSymbol:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.ModifierSymbol:
                    case UnicodeCategory.OpenPunctuation:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.OtherNotAssigned:
                    case UnicodeCategory.OtherNumber:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.OtherSymbol:
                    case UnicodeCategory.ParagraphSeparator:
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.Surrogate:
                    case UnicodeCategory.TitlecaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                        sbReturn.Append(letter);
                        break;
                }
            }
            return sbReturn.ToString();
        }
    }
}
