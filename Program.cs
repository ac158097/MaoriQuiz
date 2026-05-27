using System.Text.RegularExpressions;
using System.Linq;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name;
            do
            {
                Console.Write($"Enter Your {Fancify("Full", isBold: false, isUnderline: true, colorNum: 33)} Name: ");
                name = Console.ReadLine().Trim();
            } while (!ValidName(name));
        }

        static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"([a-z]{2,12} *)()+", RegexOptions.IgnoreCase);
            if (string.Join("", nameRegex.Matches(nameToTest)).Length == nameToTest.Length && nameToTest != "" && nameToTest.Length <= 30) {
                int count = nameToTest.Count(c => c == ' ');
                Console.WriteLine(count+1);
                Console.WriteLine(nameRegex.Matches(nameToTest).Count());
                if (count+1 == nameRegex.Matches(nameToTest).Count()) { return true; }
            }
            return false; 
        }

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
        static string Fancify(string stringToApplyTo, bool isBold = false, bool isUnderline = false, int colorNum = 37, bool reset = true) {
            string bolding, underlining, resetstring;

            if (reset == true) resetstring = $"\e[0m";
            else resetstring = "";

            if (isBold == true) bolding = $"\e[1;{colorNum}m";
            else bolding = "";

            if (isUnderline == true) underlining = $"\e[4;{colorNum}m";
            else underlining = "";

            return $"{underlining}{bolding}{stringToApplyTo}{resetstring}";
        }
    }
}
