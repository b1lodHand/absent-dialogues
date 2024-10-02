using System.Collections.Generic;
using System.Text;

namespace com.absence.dialoguesystem.internals
{
    public static class Utilities
    {
        public static class Texts
        {
            public static string ColorizeString(string stringToColorize, string colorHex)
            {
                StringBuilder sb = new();
                sb.Append($"<color={colorHex}>");
                sb.Append(stringToColorize);
                sb.Append("</color>");

                return sb.ToString();
            }
        }
        
        public static class Comparison
        {
            public static string GetConditionString(List<NodeVariableComparer> comparers, VBProcessType processType, bool richText = false)
            {
                bool isAnd = (processType == VBProcessType.All);
                StringBuilder sb = new();

                if (!richText) sb.Append("[");
                else sb.Append(Utilities.Texts.ColorizeString("[", GetBracketHex(processType)));

                sb.Append(" ");

                comparers.ForEach(comparer =>
                {
                    sb.Append(comparer.GetConditionString(richText));

                    sb.Append(" ");

                    if (comparers.IndexOf(comparer) != (comparers.Count - 1))
                    {
                        sb.Append(isAnd ? GetAndSymbol(richText) : GetOrSymbol(richText));
                        sb.Append(" ");
                    }
                });

                if (!richText) sb.Append("]");
                else sb.Append(Utilities.Texts.ColorizeString("]", GetBracketHex(processType)));

                return sb.ToString();

                string GetAndSymbol(bool richText = false)
                {
                    if (!richText) return "&&";
                    else return Utilities.Texts.ColorizeString("&&", Constants.Tooltips.AND_HEX);
                }

                string GetOrSymbol(bool richText = false)
                {
                    if (!richText) return "||";
                    else return Utilities.Texts.ColorizeString("&&", Constants.Tooltips.OR_HEX);
                }

                string GetBracketHex(VBProcessType processType)
                {
                    if (processType == VBProcessType.All) return Constants.Tooltips.AND_HEX;
                    else if (processType == VBProcessType.Any) return Constants.Tooltips.OR_HEX;

                    return null;
                }
            }
            public static string GetComparisonTypeIcon(NodeVariableComparer.ComparisonType comparisonType)
            {
                switch (comparisonType)
                {
                    case variablesystem.BaseVariableComparer.ComparisonType.LessThan:
                        return "<";
                    case variablesystem.BaseVariableComparer.ComparisonType.LessOrEqual:
                        return "≤";
                    case variablesystem.BaseVariableComparer.ComparisonType.EqualsTo:
                        return "=";
                    case variablesystem.BaseVariableComparer.ComparisonType.NotEquals:
                        return "≠";
                    case variablesystem.BaseVariableComparer.ComparisonType.GreaterOrEqual:
                        return "≥";
                    case variablesystem.BaseVariableComparer.ComparisonType.GreaterThan:
                        return ">";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}