using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class StringExtensions
    {

        public static int OnlyNumbers(this String str)
        {
            return Int32.Parse(string.Concat(str.Where(char.IsNumber)));
        }

        public static String OnlyLetters(this String str)
        {
            return string.Concat(str.Where(char.IsLetter));
        }
    }
}
