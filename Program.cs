#nullable disable

using System.Text.RegularExpressions;
using Question = (string, System.Collections.Generic.List<char>, System.Collections.Generic.List<char>);
using Scoretable = System.Collections.Generic.Dictionary<char, float>;
using RGBColour = (int, int, int);

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(/*string[] args*/)
        {
            //initialize vars
            string name;
            float score;
            bool replay = false;
            string replaychoice;
            Scoretable highscores = [];
            (char, List<Question>) chosenDifficulty;

            ConsoleHelper.ClearFullConsole();
            //ask for name
            do
            {
                Console.Write($"Please enter your {StringHelper.Fancify("full", isBold: true, isUnderline: true)} name: ");
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
                Console.WriteLine("Welcome, {0}!\n\nChoose a difficulty:\nEasy [E] (High Score: {1}, {}%)\nMedium [M] (High Score: {2}, {5}%)\nHard [H] (High Score: {3}, {6}%)", name, GetHighscoreOrZero(highscores, 'E'), GetHighscoreOrZero(highscores, 'M'), GetHighscoreOrZero(highscores, 'H'), Math.Round(GetHighscoreOrZero(highscores, 'E') / GetQuizQuestions("E").Item2.Count()) * 100, Math.Round(GetHighscoreOrZero(highscores, 'M') / GetQuizQuestions("M").Item2.Count()) * 100, Math.Round(GetHighscoreOrZero(highscores, 'H'), GetQuizQuestions("H").Item2.Count()) * 100);

                //pick a difficulty
                do
                {
                    Console.Write(StringHelper.RGBIfy("Choice: ", (91, 217, 210)));
                    chosenDifficulty = GetQuizQuestions(Console.ReadLine());
                    if (chosenDifficulty.Item2.Count == 0)
                    {
                        Console.WriteLine("Invalid choice!\n");
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


                //quiz score related things
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


                //ask if replaying or not
                do
                {
                    Console.Write($"Would you like to replay (Y/N)?\n{StringHelper.RGBIfy("Option: ", (91, 217, 210))}");
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

        static float GetHighscoreOrZero(Scoretable scores, char difficulty) {
            return scores.ContainsKey(difficulty) ? scores[difficulty] : 0;
        }

        //returns quiz questions
        static (char, List<Question>) GetQuizQuestions(string diffi)
        {
            if (diffi.Length == 1)
            {
                return char.ToUpper(diffi[0]) switch
                {
                    'E' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D']),
                        ("Who was the prime minister in 2026?\nA. Christopher Luxon\nB. Winston Peters\nC. Martin Luther King Jr.\nD. Joe Biden", ['A'], ['B', 'C', 'D']),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], []),
                    ]),
                    'M' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D']),
                        ("What is the capital of New Zealand?\nA. Christchurch\nB. Wellington\nC. Auckland\nD. Hamilton", ['B'], ['A', 'C', 'D']),
                        ("What does aroha mean?\nA. Good\nB. Terrible\nC. Effort\nD. Love", ['D'], ['A', 'B', 'C']),
                        ("True or False: The Treaty Of Waitangi was signed in 1845?\nT. True\nF. False", ['F'], ['T']),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], []),
                    ]),
                    'H' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D']),
                        ("What is the longest name of a place in New Zealand?\nA. Taumata­whakatangihanga­koauau­o­tamatea­turi­pukaka­piki­maunga­horo­nuku­pokai­whenua­ki­tana­tahu\nB. Llanfair­pwllgwyngyll­gogery­chwyrn­drobwll­llan­tysilio­gogo­goch\nC. Captain Cook Hawkes Bay Port\nD. Tane Mahuta Walk", ['A'], []),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y'], ['N']),
                    ]),
                    _ => ('♣', [])
                };
            }
            return ('♣', []);
        }

        //gets the users answer for a question and checks if its correct
        static bool AskQuestion(Question questions)
        {
            string userInput;
            Console.WriteLine(questions.Item1);
            do
            {
                Console.Write(StringHelper.RGBIfy("Answer: ", (91, 217, 210)));
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

        //colours text by taking rgb input
        public static string RGBIfy(string text, RGBColour col) => "\x1b[38;2;" + col.Item1 + ";" + col.Item2 + ";" + col.Item3 + "m" + text + "\e[0m";

        //checks if a name is within 52 chars long, is at least 2 words, doesnt have double spacebars, doesnt have numbers, and any full stops must come after words rather than in or before
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

        //capitalize first letters of words in string, otherwise decapitalizes them
        public static string Capitalize(string stringToCapitalize) => Regex.Replace(Regex.Replace(stringToCapitalize, @"\b[a-z]", delegate (Match match)
            {
                return match.ToString().ToUpper();
            }, RegexOptions.IgnoreCase), @"(?!\b)[A-Z]", delegate (Match match)
            {
                return match.ToString().ToLower();
            });
    }

    //clears console (do i have to explain)
    public static class ConsoleHelper
    {
        public static void ClearFullConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J");
        }
    }
}
