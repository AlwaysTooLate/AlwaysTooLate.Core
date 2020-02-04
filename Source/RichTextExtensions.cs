// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;
using System.Text;

namespace AlwaysTooLate.Core
{
    public static class RichTextExtensions
    {
        public static string ColorInnerString(string baseString, string coloredString, string color)
        {
            if (coloredString.Length == 0)
                return baseString;

            var sb = new StringBuilder();

            // Insert background color
            var startIdx = baseString.IndexOf(coloredString, StringComparison.InvariantCultureIgnoreCase);
            var endIdx = startIdx + coloredString.Length;

            if (startIdx >= 0)
            {
                // Insert color start and end
                sb.Append(baseString.Substring(0, startIdx));
                sb.Append($"<color={color}>");
                sb.Append(baseString.Substring(startIdx, coloredString.Length));
                sb.Append("</color>");
                sb.Append(baseString.Substring(endIdx, baseString.Length - endIdx));
            }
            else
            {
                sb.Append(baseString);
            }

            return sb.ToString();
        }
    }
}