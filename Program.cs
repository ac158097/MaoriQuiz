using System.Text.RegularExpressions;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name;
            do
            {
                Console.Write($"Enter Your {Fancify("Full", isBold: true)} Name: ");
                name = Console.ReadLine().Trim();
            } while (!ValidName(name));
        }

        static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"^[a-z] [a-z]+$", RegexOptions.IgnoreCase);
            if (nameRegex.Match(nameToTest).Length == nameToTest.Length && nameToTest.Length > 3 && nameToTest.Length < 20) {
                return true;
            }
            return false; 
        }

        /*
        color numbers:
        0: white
        30: black
        31: red
        32: green
        33: yellow
        34: blue
        35: purple
        36: cyan
        37: white
        method for ansi formatting but somewhat simpler
        */
        static string Fancify(string stringToApplyTo, bool isBold = false, bool isUnderline = false, int colorNum = 37) {
            string stringANSIFormatted = $"\e[{Convert.ToInt16(isBold)};{colorNum}{stringToApplyTo}\e[0m";
            //string stringANSIFormatted = $"{stringToApplyTo}";
            return stringANSIFormatted;
        }
    }
}
