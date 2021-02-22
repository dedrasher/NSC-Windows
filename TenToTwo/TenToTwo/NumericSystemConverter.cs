using System;

namespace NumericSystemConverterApp
{
 public static class NumericSystemConverter
    {
        private static readonly string Keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static char GetKeyFromIndex(int index)
        {
            if(index < 10 && index >36 )
            {
                throw new ArgumentException("Invalid argument");
            }
            return Keys[index - 10];
        }
        private static int GetIntFromChar(char value)
        {
            return Keys.IndexOf(value) + 10;
        }
        public static string Convert(int From, int To, string input)
        {
            if (From <= 36 && To <= 36 && From > 1 && To > 1)
            {
                string Input = input;
                if (!IsValidInput(Input, From)) throw new ArgumentException("Input is not correct");
                if (Input == "0") throw new ArgumentException("Input is zero");
                bool Isminus = Input.Contains("-");
                if (Isminus) Input = Input.Remove(0, 1);
                long result = 0L;
                int pow = 0;
                foreach (char i in Reverse(Input))
                {
                    if (char.IsLetter(i) && From > 10)
                    {
                        result += System.Convert.ToInt64(GetIntFromChar(i)) * System.Convert.ToInt64(Math.Pow(From, pow));
                    }
                    else
                    {
                        result += long.Parse(i.ToString()) * System.Convert.ToInt64(Math.Pow(From, pow));
                   }                 
                    pow++;
                }
                string MinusString = Isminus ? "-" : null;
                long a = long.Parse(MinusString + result.ToString());
                if (a == 0L) throw new ArgumentException("Decimal can't be zero");
                bool isminus = a < 0L;
                if (isminus) a = -a;
                string answer = null;
                while (true)
                {
                    long o = a % System.Convert.ToInt64(To);
                    if (o > 9L && To > 10)
                    {
                        answer += GetKeyFromIndex((int)o).ToString();
                    } else
                    {
                        answer += o.ToString();
                    }
                    a /= System.Convert.ToInt64(To);
                    if (a < 1L)
                        break;
                }
                string addd = isminus ? "-" : null;
                return addd + Reverse(answer);
            } else
            {
                throw new ArgumentException("Incorrect numeric system!");
            }
        }
        private static string Reverse(string A) { 
            string ret = null;
            for (int i = A.Length - 1; i >= 0; i--)
            {
                ret += A[i];
            }
            return ret;
        }
        private static bool IsValidInput(string check, int system)
        {
            try
            {

                if (check[0] == '0') return false;
                if (check.LastIndexOf("-") > 0) return false;
                foreach (char i in check)
                {
                    if (system > 10 && !char.IsDigit(i))
                    {
                        if (i != '-' && GetIntFromChar(i) > system - 1) return false;
                    }
                    else
                    {
                        if (i != '-' && System.Convert.ToInt32(i.ToString()) > system - 1) return false;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
