#nullable disable

using System.Text.RegularExpressions;
using Question = (string, System.Collections.Generic.List<char>, System.Collections.Generic.List<char>);
using RGBColour = (int, int, int);

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(/*string[] args*/)
        {
            string name;
            float score;
            bool replay = false;
            string replaychoice;
            Dictionary<char, float> highscores = [];
            (char, List<Question>) chosenDifficulty;

            ConsoleHelper.ClearFullConsole();
            do
            {
                Console.Write($"Please enter your {StringHelper.Fancify("full", isBold: false, isUnderline: true, colorNum: 33)} name: ");
                name = StringHelper.Capitalize(Console.ReadLine().Trim());
                if (!StringHelper.ValidName(name))
                {
                    Console.WriteLine("Not a valid full name!\n");
                }
            } while (!StringHelper.ValidName(name));

            do
            {
                ConsoleHelper.ClearFullConsole();
                score = 0;
                Console.WriteLine("Welcome, {0}!\n\nChoose a difficulty:\nEasy [E] (High Score: {1})\nMedium [M] (High Score: {2})\nHard [H] (High Score: {3})", name, highscores.ContainsKey('E') ? highscores['E'] : 0, highscores.ContainsKey('M') ? highscores['M'] : 0, highscores.ContainsKey('H') ? highscores['H'] : 0);

                //pick a difficulty
                do
                {
                    Console.Write("\nChoice: ");
                    chosenDifficulty = GetQuizQuestions(Console.ReadLine());
                    if (chosenDifficulty.Item2.Count == 0)
                    {
                        Console.WriteLine("Invalid choice!");
                    }
                } while (chosenDifficulty.Item2.Count == 0);

                ConsoleHelper.ClearFullConsole();

                // ask each question
                for (int i = 0; i < chosenDifficulty.Item2.Count(); i++)
                {
                    Console.Write($"Question {i + 1}: ");
                    if (AskQuestion(chosenDifficulty.Item2[i])) { Console.WriteLine($"{StringHelper.Fancify("Correct!", colorNum: 32)}\n"); score++; }
                    else Console.WriteLine($"{StringHelper.Fancify("Incorrect!", colorNum: 31)}\n");
                }

                Console.WriteLine($"Score: {score}\tPercent: {Math.Round((score / chosenDifficulty.Item2.Count()) * 100)}%");
                if (!highscores.ContainsKey(chosenDifficulty.Item1))
                {
                    highscores.Add(chosenDifficulty.Item1, 0);
                }
                if (score > highscores[chosenDifficulty.Item1])
                {
                    Console.WriteLine("New High Score!");
                    highscores[chosenDifficulty.Item1] = score;
                }


                do
                {
                    Console.Write("Would you like to replay (Y/N)?\nOption: ");
                    replaychoice = Console.ReadLine();
                    if (replaychoice.Length == 1)
                    {
                        if (char.ToUpper(replaychoice[0]) == 'Y')
                        {
                            replay = true;
                        }
                        else if (char.ToUpper(replaychoice[0]) == 'N')
                        {
                            replay = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Option!\n");
                            replaychoice = "♣";
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Option!\n");
                        replaychoice = "♣";
                    }
                } while (replaychoice == "♣");
            } while (replay == true);
        }

        static (char, List<Question>) GetQuizQuestions(string diffi)
        {
            if (diffi.Length == 1)
            {
                return char.ToUpper(diffi[0]) switch
                {
                    'E' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", ['A'], ['B', 'C', 'D']),
                        ("Who was the prime minister in 2026?\nA. Christopher Luxon\n B. Winston Peters\n C. Martin Luther King Jr.\n D. Joe Biden", ['A'], ['B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], [])
                    ]),
                    'M' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", ['A'], ['B', 'C', 'D']),
                        ("What is the capital of New Zealand?\nA. Christchurch.\nB. Wellington.\nC. Auckland.\nD. Hamilton", ['B'], ['A', 'C', 'D']),
                        ("What does aroha mean?\nA. Good.\nB. Terrible.\nC. Effort.\nD. Love.", ['D'], ['A', 'B', 'C']),
                        ("True or False: The Treaty Of Waitangi was signed in 1845?\nT. True\nF. False", ['F'], ['T']),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], [])
                    ]),
                    'H' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello.\nB. Good Morning.\nC. Good Night.\nD. I'm Hungry.", ['A'], ['B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y'], ['N'])
                    ]),
                    _ => ('♣', [])
                };
            }
            return ('♣', []);
        }

        static bool AskQuestion(Question questions)
        {
            string userInput;
            Console.WriteLine(questions.Item1);
            do
            {
                Console.Write(StringHelper.RGBIfy("Answer: ", (50, 200 ,25)));
                userInput = Console.ReadLine();
                if (userInput.Length != 1 || !questions.Item3.Contains(char.ToUpper(userInput[0])) && !questions.Item2.Contains(char.ToUpper(userInput[0])))
                {
                    Console.WriteLine("Invalid Answer!\n");
                    userInput = "♣";
                }
            } while (userInput == "♣");
            return questions.Item2.Contains(char.ToUpper(userInput[0]));
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

        public static string Fancify(string text, bool isBold = false, bool isUnderline = false, int colorNum = 37, bool reset = true)
        {
            string formatting, resetstring;

            switch (isBold, isUnderline)
            {
                case (false, false): formatting = $"\e[0;{colorNum}m"; break;
                case (true, false): formatting = $"\e[1;{colorNum}m"; break;
                case (false, true): formatting = $"\e[4;{colorNum}m"; break;
                case (true, true): formatting = $"\e[1;{colorNum}m\e[4;{colorNum}m"; break;
            }
            resetstring = (reset == true) ? $"\e[0m" : "";
            return $"{formatting}{text}{resetstring}";
        }

        public static string RGBIfy(string text, RGBColour col) => "\x1b[38;2;" + col.Item1 + ";" + col.Item2 + ";" + col.Item3 + "m" + text + "\e[0m";

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

        public static string Capitalize(string stringToCapitalize) => Regex.Replace(Regex.Replace(stringToCapitalize, @"\b[a-z]", delegate (Match match)
            {
                return match.ToString().ToUpper();
            }, RegexOptions.IgnoreCase), @"(?!\b)[A-Z]", delegate (Match match)
            {
                return match.ToString().ToLower();
            });
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
