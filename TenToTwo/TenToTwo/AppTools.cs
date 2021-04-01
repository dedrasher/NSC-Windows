using System;
using System.Linq;

namespace NumericSystemConverterApp
{
    public struct AppTools
    {
        public static string GetSmallNumber(int value)
        {
            return new string[] { "₂","₃", "₄","₅", "₆","₇", "₈", "₉", "₁₀", "₁₁", "₁₂", "₁₃", "₁₄", "₁₅",
            "₁₆","₁₇","₁₈","₁₉","₂₀","₂₁","₂₂","₂₃","₂₄","₂₅","₂₆","₂₇","₂₈","₂₉","₃₀","₃₁","₃₂","₃₃","₃₄","₃₅","₃₆"}[value - 2];
        }
        public static ParsedExpression ParseExpression(string value) {
            var SmallNumbers = new string[] { "₁", "₂", "₃", "₄", "₅", "₆", "₇", "₈", "₉", "₀" };
            bool IsSmallRegisterNumber(string number)
            {
                return Array.IndexOf(SmallNumbers, number) > -1;
            }
            string UpperNumber(string SmallNumber)
            {
                var UpperNumbers = "1234567890";
                string result = null;
                for (int i = 0; i < SmallNumber.Length; i++)
                {
                    result += UpperNumbers[Array.IndexOf(SmallNumbers, SmallNumber[i].ToString())];
                }
                return result;
            }
            var item = value;
            var input = string.Join("", item.TakeWhile(s => !IsSmallRegisterNumber(s.ToString())).ToArray());
            var FromNC = UpperNumber(string.Join("", item.Remove(0, input.Length).TakeWhile(s => IsSmallRegisterNumber(s.ToString())).ToArray()));
            var Result = string.Join("", item.Remove(0, input.Length + FromNC.Length + 3).TakeWhile(s => !IsSmallRegisterNumber(s.ToString())).ToArray());
            var ToNC = UpperNumber(item.Remove(0, input.Length + FromNC.Length + 3 + Result.Length).ToUpper());
            return new ParsedExpression(ToNC, FromNC, input, Result);
        }


    }
}
