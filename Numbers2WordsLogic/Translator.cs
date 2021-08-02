using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Numbers2WordsLogic
{
    public class Translator
    {
        private static bool hasAndBeenAdded;

        public static string Translate(string stringNumber)
        {
            var result = AddMinusIfNeeded(ref stringNumber);

            if (stringNumber.Contains("."))
            {
                return TranslateFractionalNumber(stringNumber, result);
            }
            else
            {
                return TranslateIntegerNumber(stringNumber, result);
            }
        }

        private static string TranslateIntegerNumber(string stringNumber, string result)
        {
            var integerToTranslate = Int64.Parse(stringNumber);
            result += ConvertNumber(integerToTranslate, "dollar");
            return result;
        }

        private static string TranslateFractionalNumber(string stringNumber, string result)
        {
            var numbers = stringNumber.Split('.');

            long integerPart = Int64.Parse(numbers[0]);

            result += ConvertNumber(integerPart, "dollar", includeAnd: true);

            long fractionalPart = Int64.Parse(numbers[1]);

            result += ConvertNumber(fractionalPart, "cent");

            return result;
        }

        private static string ConvertNumber(long number, string textToAdd, bool includeAnd = false)
        {
            string result = TranslateNumber(number);
            hasAndBeenAdded = false;
            result += AddSpaceIfNedded(result);
            result += AddDollarsOrCents(number, textToAdd, includeAnd);
            result = removeExtraSpaces(result);
            return result;
        }

        private static string TranslateNumber(long numberToTranslate)
        {
            var result = String.Empty;
            bool includeAnd = false;

            if(numberToTranslate == 0)
            {
                return TranslateSmallerThan20(numberToTranslate);
            }

            while(numberToTranslate > 20)
            {
                double lowerPowerOfTen = (long)Math.Pow(10, numberToTranslate.ToString().Length - 1);
                result += CheckForLargeNumbers(ref numberToTranslate, lowerPowerOfTen, ref includeAnd);
            }

            if (numberToTranslate < 20 && numberToTranslate > 0)
            {
                AddAndIfNeeded(ref result, ref includeAnd);
                result += AddSpaceIfNedded(result);
                return result += TranslateSmallerThan20(numberToTranslate);
            }

            result += CheckForLargeNumbers(ref numberToTranslate, 10, ref includeAnd);

            result += AddSpaceIfNedded(result);

            result += CheckForLargeNumbers(ref numberToTranslate, 1, ref includeAnd);

            return result;
        }

        private static void AddAndIfNeeded(ref string result, ref bool includeAnd)
        {
            if (includeAnd)
            {
                if(!hasAndBeenAdded)
                {
                    hasAndBeenAdded = true;
                    result += " and ";
                }
                else
                {
                    result += " ";
                }
                includeAnd = false;
            }
        }

        private static string AddDollarsOrCents(long number, string textToAdd, bool includeAnd = false)
        {
            var result = String.Empty;
            result += textToAdd;

            if (number > 1 || number == 0)
            {
                result += "s";
            }

            AddAndIfNeeded(ref result, ref includeAnd);

            return result;
        }

        private static string AddMinusIfNeeded(ref string stringNumber)
        {
            if (stringNumber[0] == '-')
            {
                stringNumber = stringNumber.Substring(1, stringNumber.Length-1);
                return "minus ";
            }
            return String.Empty;
        }

        private static string AddSpaceIfNedded(string result)
        {
            if (result.Length > 1 && result[result.Length - 1] != ' ')
            {
                return " ";
            }

            return String.Empty;
        }

        public static string CheckForLargeNumbers(ref long numberToTranslate, double divisor, ref bool includeAnd)
        {
            double divisionResult = 0;
            string originalNumberToTranslate = numberToTranslate.ToString();
            bool smallerThan20 = false;
            bool addThousandOrMillion = true;

            if ((divisor == 10000  || divisor == 10000000) && originalNumberToTranslate[1] != '0')
            {
                //In this case take the first two numbers
                divisionResult = (numberToTranslate / divisor)*10;

                if(divisionResult >= 20)
                {
                    addThousandOrMillion = false;
                    divisionResult = (int)(numberToTranslate / divisor);
                }
                else
                {
                    smallerThan20 = true;
                }
            }
            else
            {
                divisionResult = (int)(numberToTranslate / divisor);
            }

            if (divisionResult >= 1)
            {
                var result = String.Empty;

                AddAndIfNeeded(ref result, ref includeAnd);

                int integerDivison = (int)divisionResult;

                if(divisionResult > 10)
                {
                    numberToTranslate = ((long)(numberToTranslate - (integerDivison * divisor / 10)));
                }
                else
                {
                    numberToTranslate = ((long)(numberToTranslate - (integerDivison * divisor)));
                }

                switch (divisor)
                {
                    case 1:
                        return result + TranslateSmallerThan20(integerDivison);
                    case 10:
                        return result + TranslateTens(integerDivison);
                    case 100:
                        includeAnd = true;
                        return result + TranslateSmallerThan20(integerDivison) + " hundred";
                    case 1000:
                        includeAnd = true;
                        return result + TranslateSmallerThan20(integerDivison) + " thousand";
                    case 10000:
                        includeAnd = true;

                        if (smallerThan20)
                        {
                            result += TranslateSmallerThan20(integerDivison) + " ";
                        }
                        else
                        {
                            result += TranslateTens(integerDivison) + " ";
                        }

                        if(addThousandOrMillion)
                        {
                            result += "thousand";
                        }

                        return result;


                    case 100000:
                        includeAnd = true;
                        result += TranslateSmallerThan20(integerDivison) + " hundred";
                        if (numberToTranslate == 0)
                        {
                            result += " thousand";
                        }
                        return result;
                    case 1000000:
                        includeAnd = true;
                        return result + TranslateSmallerThan20(integerDivison) + " million";
                    case 10000000:
                        includeAnd = false;

                        if (smallerThan20)
                        {
                            result += TranslateSmallerThan20(integerDivison) + " ";
                        }
                        else
                        {
                            result += TranslateTens(integerDivison) + " ";
                        }

                        if (addThousandOrMillion)
                        {
                            result += " million ";
                        }

                        return result;
                    case 100000000:
                        includeAnd = true;
                        result += TranslateSmallerThan20(integerDivison) + " hundred";
                        if (addThousandOrMillion)
                        {
                            result += " million ";
                        }
                        return result;
                    case 1000000000:
                        includeAnd = true;
                        return result + TranslateSmallerThan20(integerDivison) + " billion";
                }
            }
            return String.Empty;
        }

        public static string removeExtraSpaces(string result)
        {
            Regex trimmer = new Regex(@"\s\s+");
            return trimmer.Replace(result, " ");
        }

        public static string TranslateSmallerThan20(long numberToTranslate)
        {
            switch (numberToTranslate)
            {
                case 0:
                    return "zero";
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eigth";
                case 9:
                    return "nine";
                case 10:
                    return "ten";
                case 11:
                    return "eleven";
                case 12:
                    return "twelve";
                case 13:
                    return "therteen";
                case 14:
                    return "fourteen";
                case 15:
                    return "fifteen";
                case 16:
                    return "sixteen";
                case 17:
                    return "seventeen";
                case 18:
                    return "eighteen";
                case 19: 
                    return "nineteen";
                default:
                    return String.Empty;
                    
            }
        }

        public static string TranslateTens(int numberToTranslate)
        {
            switch (numberToTranslate)
            {
                case 1:
                    return "ten";
                case 2:
                    return "twenty";
                case 3:
                    return "thirty";
                case 4:
                    return "forty";
                case 5:
                    return "fifty";
                case 6:
                    return "sixty";
                case 7:
                    return "seventy";
                case 8:
                    return "eighty";
                case 9:
                    return "ninety";
                default:
                    return String.Empty;
            }
        }
    }
}
