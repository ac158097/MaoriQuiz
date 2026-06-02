using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Quiz = System.Collections.Generic.List<(string, char, System.Collections.Generic.List<char>)>;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name;
            Quiz chosenDifficulty;

            ConsoleHelper.ClearFullConsole();
            do
            {
                Console.Write($"Please enter your {StringHelper.Fancify("full", isBold: false, isUnderline: true, colorNum: 33)} name: ");
                name = StringHelper.Capitalize(Console.ReadLine()+"".Trim());
                if (!StringHelper.ValidName(name))
                {
                    Console.WriteLine("Not a valid full name!\n");
                }
            } while (!StringHelper.ValidName(name));

            ConsoleHelper.ClearFullConsole();
            Console.WriteLine($"Welcome, {name}!\n");
            Console.WriteLine("Choose a difficulty:\nEasy (E)\nMedium (M)\nHard (H)");

            do
            {
                Console.Write("\nChoice: ");
                chosenDifficulty = GetQuizQuestions(Console.ReadLine()+"");
                if (chosenDifficulty.Count == 0)
                {
                    Console.WriteLine("Invalid choice!");
                }
            } while (chosenDifficulty.Count == 0);

            ConsoleHelper.ClearFullConsole();
            for (int i = 0; i < chosenDifficulty.Count(); i++)
            {
                AskQuestion(chosenDifficulty[i]);
            }
        }

        static Quiz GetQuizQuestions(string diffi)
        {
            if (diffi.Length == 1)
            {
                return char.ToUpper(diffi[0]) switch
                {
                    'E' => [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", 'A', ['A', 'B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", 'Y', ['Y', 'N'])
                    ],
                    'M' => [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", 'A', ['A', 'B', 'C', 'D']),
                        ("What does aroha mean?\nA. Good.\nB. Terrible.\nC. Effort.\nD. Love.", 'D', ['A', 'B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", 'Y', ['Y', 'N'])
                    ],
                    'H' => [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", 'A', ['A', 'B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", 'Y', ['Y', 'N'])
                    ],
                    _ => []
                };
            }
            else return [];
        }

        static bool AskQuestion((string, char, System.Collections.Generic.List<char>) questions)
        {
            string userInput;
            Console.WriteLine(questions.Item1);
            do
            {
                Console.Write("Answer: ");
                userInput = Console.ReadLine();
                if (userInput.Length != 1)
                {
                    Console.WriteLine("Invalid Answer!\n");
                    userInput = "♣";
                }
            } while (!questions.Item3.Contains(userInput[0]));
            return userInput[0] == questions.Item2;
        }
    }

    public static class StringHelper
    {
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
            string formatting, resetstring;

            switch (isBold, isUnderline)
            {
                case (false, false): formatting = $"\e[0;{colorNum}m"; break;
                case (false, true): formatting = $"\e[4;{colorNum}m"; break;
                case (true, false): formatting = $"\e[1;{colorNum}m"; break;
                case (true, true): formatting = $"\e[1;{colorNum}m\e[4;{colorNum}m"; break;
            }
            if (reset == true) resetstring = $"\e[0m";
            else resetstring = "";
            return $"{formatting}{stringToApplyTo}{resetstring}";
        }

        public static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"([a-z]+\.? *)()+", RegexOptions.IgnoreCase);
            if (string.Join("", nameRegex.Matches(nameToTest)).Length == nameToTest.Length && nameToTest != "" && nameToTest.Length <= 52 && nameToTest.Contains(" "))
            {
                int count = nameToTest.Count(c => c == ' ');
                if (count + 1 == nameRegex.Matches(nameToTest).Count()) { return true; }
            }
            return false;
        }

        public static string Capitalize(string stringToCapitalize)
        {
            string returnString = Regex.Replace(stringToCapitalize, @"\b[a-z]", delegate (Match match)
            {
                return match.ToString().ToUpper();
            }, RegexOptions.IgnoreCase);

            returnString = Regex.Replace(returnString, @"(?!\b)[A-Z]", delegate (Match match)
            {
                return match.ToString().ToLower();
            });

            return returnString;
        }
    }

    public static class ConsoleHelper
    {
        public static void ClearFullConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J");
        }
    }
}
