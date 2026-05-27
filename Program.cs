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
                Console.Write($"Enter Your {Fancify("Full", isBold: false, isUnderline: true, colorNum: 33)} Name: ");
                name = Console.ReadLine().Trim();
            } while (!ValidName(name));
        }

        static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"([a-z]* [a-z]*){3,20}", RegexOptions.IgnoreCase);
            if (nameRegex.Match(nameToTest).Length == nameToTest.Length) {
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
            string bolding, underlining;
            if (isBold == true) bolding = $"\e[1;{colorNum}m"; else bolding = "";
            if (isUnderline == true) underlining = $"\e[4;{colorNum}m"; else underlining = "";
            return $"{underlining}{bolding}{stringToApplyTo}\e[0m";
        }
    }
}
