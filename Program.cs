using System.Text.RegularExpressions;
using System.Linq;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> EASYQUESTIONS = new List<string>() { "" };
            string name;
            do
            {
                Console.Write($"Please enter your {StringHelper.Fancify("full", isBold: false, isUnderline: true, colorNum: 33)} name: ");
                name = Console.ReadLine().Trim();
                if (!StringHelper.ValidName(name)) {
                    Console.WriteLine("Not a valid full name!\n");
                }
            } while (!StringHelper.ValidName(name));
        }

        
    }

    public static class StringHelper {
        /*
        color numbers:
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

        public static string Fancify(string stringToApplyTo, bool isBold = false, bool isUnderline = false, int colorNum = 37, bool reset = true)
        {
            string bolding, underlining, resetstring;

            if (reset == true) resetstring = $"\e[0m";
            else resetstring = "";

            if (isBold == true) bolding = $"\e[1;{colorNum}m";
            else bolding = "";

            if (isUnderline == true) underlining = $"\e[4;{colorNum}m";
            else underlining = "";

            return $"{underlining}{bolding}{stringToApplyTo}{resetstring}";
        }

        public static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"([a-z]{2,12} *)()+", RegexOptions.IgnoreCase);
            if (string.Join("", nameRegex.Matches(nameToTest)).Length == nameToTest.Length && nameToTest != "" && nameToTest.Length <= 30 && nameToTest.Contains(" "))
            {
                int count = nameToTest.Count(c => c == ' ');
                if (count + 1 == nameRegex.Matches(nameToTest).Count()) { return true; }
            }
            return false;
        }
    }
}
