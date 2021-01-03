using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace new_idea_math_calculation_
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "9*6";
            string input2 = "9*6+4";
            string input3 = "(9+66)^6";
            string input4 = "(9^6)^(5*44)";
            string input5 = "9*6*45^5+6-5/5";
            string input6 = "6^5^5";
            string input7 = "1^2^3^4^5";
            string input8 = "5^2*(5*3)";
            Console.WriteLine(calc(input8));
        }



        static string calc(string input)
        {
            List<string> bracketNums = new List<string>();

            if (input.Contains("(") && input.Contains(")"))
            {
                bracketNums.AddRange(brackets(input.ToCharArray()));

                input = buildNewExpression(input, bracketNums);
            }

            StringBuilder sb = new StringBuilder();

            if (input.Contains("^"))
            {
                sb.Append(phase2(input));
            }
            else
            {
                sb.Append(compute(input));
            }

            return sb.ToString();
        }

        private static string phase2(string input)
        {
            string[] w = input.Split("^");

            List<string> calcs = new List<string>();

            StringBuilder sb = new StringBuilder();

            foreach (var item in w)
            {
                sb.Append(compute(item));
                if (w.ToList().IndexOf(item) != w.Length - 1)
                {
                    sb.Append("^");
                }
            }

            if (input.Split("^").Length == 2)
            {
                return power(sb.ToString());
            }
            else
            {

                char[] t = sb.ToString().ToCharArray();

                StringBuilder numberMaker = new StringBuilder();
                List<string> a = new List<string>();

                for (int i = 0; i < t.Length; i++)
                {
                    if (Regex.IsMatch(t[i].ToString(), @"[\+\-\*\/\^]"))
                    {
                        a.Add(numberMaker.ToString());
                        a.Add(t[i].ToString());
                        numberMaker.Clear();
                    }
                    else
                    {
                        numberMaker.Append(t[i]);
                    }

                    if (i == t.Length - 1)
                    {
                        a.Add(numberMaker.ToString());
                        a.Add(t[i].ToString());
                        numberMaker.Clear();
                    }
                }

                decimal result = decimal.Zero;

                for (int i = 0; i < a.Count; i++)
                {
                    if (Regex.IsMatch(a[i].ToString(), @"[\+\-\*\/\^]"))
                    {
                        if (Regex.IsMatch(a[i].ToString(), @"[\+\-\*\/]"))
                        {
                            string ex = null;
                            if (result == 0)
                            {
                                ex = $"{a[i - 1]}{a[i]}{a[i + 1]}";
                            }
                            else
                            {
                                ex = $"{result}{a[i]}{a[i + 1]}";
                            }
                            
                            result = Convert.ToDecimal(compute(ex));
                        }
                        else if (Regex.IsMatch(a[i].ToString(), @"[\^]"))
                        {
                            string ex = null;
                            if (result == 0)
                            {
                                ex = $"{a[i - 1]}{a[i]}{a[i + 1]}";
                            }
                            else
                            {
                                ex = $"{result}{a[i]}{a[i + 1]}";
                            }
                            result = Convert.ToDecimal(power(ex));
                        }
                    }
                }


                return result.ToString();

            }
        }

        static string buildNewExpression(string original, List<string> updated)
        {
            StringBuilder sb = new StringBuilder();

            int length = original.ToCharArray().Length;
            int done = 0;
            int openingIndex = 0;
            int closingIndex = 0;
            int oldOpeningIndex = -1;
            int oldClosingIndex = -1;
            bool first = true;

            

            for (int i = 0; i < length; i++)
            {
                if (original[i] == '(')
                {
                    if (!first)
                    {
                        oldOpeningIndex = openingIndex;
                    }
                    else
                    {
                        first = false;
                    }
                    openingIndex = i;
                }
                if (original[i] == ')')
                {
                    oldClosingIndex = closingIndex;
                    closingIndex = i;
                }
                if ((openingIndex < closingIndex) && openingIndex != oldOpeningIndex && closingIndex != oldClosingIndex)
                {
                    if (original[i] == '+' || original[i] == '-' || original[i] == '/' || original[i] == '*' || original[i] == '^' || original[i] == '%' || Regex.IsMatch(original[i].ToString(), @"\d+"))
                    {
                        if (Regex.IsMatch(original[i].ToString(), @"\d+"))
                        {
                            string e = original[i].ToString();

                            e = Regex.Replace(
                                e,
                                @"\d+(\.\d+)?",
                                m => {
                                    var x = m.ToString();
                                    return x.Contains(".") ? x : string.Format("{0}.0", x);
                                });

                            sb.Append(e);
                        }
                        else
                        {
                            sb.Append(original[i]);
                        }                        
                    }
                    else
                    {
                        sb.Append(updated[done]);
                        done += 1;
                    }
                }
            }

            return sb.ToString();
        }

        static string compute(string e)
        {
            e = Regex.Replace(
                e,
                @"\d+(\.\d+)?",
                m => {
                    var x = m.ToString();
                    return x.Contains(".") ? x : string.Format("{0}.0", x);
                });

            var loDataTable = new DataTable();
            var result = loDataTable.Compute(e, string.Empty);

            return result.ToString();
        }

        static string power(string e)
        {
            string[] s = e.Split("^");

            string p = "1";
            decimal n = decimal.Parse(s[0]);

            for (int i = 0; i < decimal.Parse(s[1]); i++)
            {
                try
                {
                    p = (Convert.ToDecimal(p) * n).ToString().TrimEnd(new char[] { '0' }).TrimStart(new char[] { '0' });
                }
                catch (Exception)
                {
                    return "Kill";
                }
            }

            return p;
        }

        static List<string> brackets(char[] a)
        {
            List<string> returnVal = new List<string>();

            int opening = 0;
            int closing = 0;

            //find how many brackets their are
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == '(')
                {
                    opening = opening + 1;
                }
                if (a[i] == ')')
                {
                    closing = closing + 1;
                }
            }
            //end 

            
            int done = 0;
            int openingIndex = 0;
            int closingIndex = 0;
            int oldOpeningIndex = -1;
            int oldClosingIndex = -1;
            bool first = true;

            for (int i = 0; i < a.Length; i++)
            {                
                if (a[i] == '(')
                {
                    if (!first)
                    {
                        oldOpeningIndex = openingIndex;
                    }
                    else
                    {
                        first = false;
                    }
                    openingIndex = i;
                }
                if (a[i] == ')')
                {
                    oldClosingIndex = closingIndex;
                    closingIndex = i;
                }
                if ((openingIndex < closingIndex) && openingIndex != oldOpeningIndex && closingIndex != oldClosingIndex)
                {
                    if (a[i] == '+' || a[i] == '-' || a[i] == '/' || a[i] == '*' || a[i] == '^' || a[i] == '%')
                    {
                        
                    }   
                    else
                    {
                        if (done < opening)
                        {
                            
                            StringBuilder sb = new StringBuilder();
                            i = openingIndex + 1;
                            do
                            {
                                sb.Append(a[i]);
                                i = i + 1;
                            } while (i > openingIndex && i < closingIndex);
                            //for (int q = openingIndex + 1; q < closingIndex - openingIndex; q++)
                            //{
                                
                            //}

                            if (sb.ToString().Contains("^"))
                            {
                                returnVal.Add(power(sb.ToString()));
                            }
                            else
                            {
                                returnVal.Add(compute(sb.ToString()));
                            }
                            done = done + 1;
                        }
                    }
                }
            }

            return returnVal;

        }
    }
}
