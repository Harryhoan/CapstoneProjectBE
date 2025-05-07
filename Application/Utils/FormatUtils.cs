using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class FormatUtils
    {
        public static string TrimSpacesPreserveSingle(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

    }
}
