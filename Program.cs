#nullable disable

using System.Text.RegularExpressions;
using Question = (string, System.Collections.Generic.List<char>, System.Collections.Generic.List<char>, float);
// questions, answer chars, incorrect chars, points awarded for correct
using RGBColour = (int, int, int);
using Scoredict = System.Collections.Generic.Dictionary<char, float>;

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
            Scoredict highscores = [];
            (char, List<Question>) chosenDifficulty;

            ConsoleHelper.ClearFullConsole();
            //ask for name
            do
            {
                Console.Write($"Please enter your {StringHelper.Fancify("full", isBold: true, isUnderline: true)} name: ");
                name = StringHelper.Capitalize(Console.ReadLine().Trim());
                if (!StringHelper.ValidName(name))
                {
                    Console.WriteLine("Not a valid full name.\n");
                }
            } while (!StringHelper.ValidName(name));

            do
            {
                ConsoleHelper.ClearFullConsole();
                score = 0;
                //print difficulties, highscores, and what percent of questions were right from highscore
                Console.WriteLine("Welcome, {0}!\nChoose a difficulty:\n{7}Easy [E]\t(High Score: {1}, {4}% Correct)\n{8}Medium [M]\t(High Score: {2}, {5}% Correct)\n{9}Hard [H]\t(High Score: {3}, {6}% Correct)\n{10}Quit [Q]",
                                  name,
                                  GetHighscoreOrZero(highscores, 'E'),
                                  GetHighscoreOrZero(highscores, 'M'),
                                  GetHighscoreOrZero(highscores, 'H'),
                                  Math.Round((GetHighscoreOrZero(highscores, 'E') / GetQuizQuestions("E").Item2.Count) * 100),
                                  Math.Round((GetHighscoreOrZero(highscores, 'M') / GetQuizQuestions("M").Item2.Count) * 100),
                                  Math.Round((GetHighscoreOrZero(highscores, 'H') / GetQuizQuestions("H").Item2.Count) * 100),
                                  StringHelper.RGBIfy("", (0, 255, 0), reset: false),
                                  StringHelper.RGBIfy("", (255, 255, 0), reset: false),
                                  StringHelper.RGBIfy("", (255, 0, 0), reset: false),
                                  StringHelper.RGBIfy("", (123, 0, 217), reset: false)
                                  );

                //pick a difficulty
                do
                {
                    Console.Write($"{StringHelper.RGBIfy("Choice", (91, 217, 210))}: ");
                    chosenDifficulty = GetQuizQuestions(Console.ReadLine());
                    if (chosenDifficulty.Item2.Count == 0)
                    {
                        Console.WriteLine("Invalid choice.\n");
                    }
                } while (chosenDifficulty.Item2.Count == 0);

                ConsoleHelper.ClearFullConsole();

                // ask each question
                for (int i = 0; i < chosenDifficulty.Item2.Count(); i++)
                {
                    if (chosenDifficulty.Item1 != 'Q')
                    {
                        Console.Write(StringHelper.RGBIfy($"Question {i + 1}: ", (217, 72, 0)));
                        if (AskQuestion(chosenDifficulty.Item2[i])) { Console.WriteLine($"{StringHelper.Fancify("Correct!", colorNum: 32)}\n"); score++; }
                        else Console.WriteLine($"{StringHelper.Fancify("Incorrect!", colorNum: 31)}\n");
                    }
                    else
                    {
                        if (AskQuestion(chosenDifficulty.Item2[i])) { Environment.Exit(0); }
                    }
                }


                //quiz score/highscore related things
                if (chosenDifficulty.Item1 != 'Q')
                {
                    if (!highscores.ContainsKey(chosenDifficulty.Item1))
                    {
                        highscores.Add(chosenDifficulty.Item1, 0);
                    }
                    if (score > highscores[chosenDifficulty.Item1])
                    {
                        Console.WriteLine("New High Score!");
                        highscores[chosenDifficulty.Item1] = score;
                    }
                    Console.WriteLine($"Score: {score}\tPercent: {Math.Round((score / chosenDifficulty.Item2.Count()) * 100)}%");


                    //ask if replaying or not
                    do
                    {
                        Console.Write($"Would you like to replay (Y/N)?\n{StringHelper.RGBIfy("Option", (91, 217, 210))}: ");
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
                                Console.WriteLine("Invalid Option.\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Option.\n");
                        }
                    } while (!new List<string> { "Y", "N" }.Contains(replaychoice));
                }
                else {
                    replay = true;
                }
            } while (replay == true);
        }

        //return a high score, if there is none, return zero
        static float GetHighscoreOrZero(Scoredict scores, char difficulty) => scores.ContainsKey(difficulty) ? scores[difficulty] : 0;

        //returns quiz questions
        static (char, List<Question>) GetQuizQuestions(string diffi)
        {
            if (diffi.Length == 1)
            {
                return char.ToUpper(diffi[0]) switch
                {
                    'E' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D'], 1),
                        ("Who was the prime minister in 2026?\nA. Christopher Luxon\nB. Winston Peters\nC. Martin Luther King Jr.\nD. Joe Biden", ['A'], ['B', 'C', 'D'], 1),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], [], 1),
                    ]),
                    'M' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D'], 1),
                        ("What is the capital of New Zealand?\nA. Christchurch\nB. Wellington\nC. Auckland\nD. Hamilton", ['B'], ['A', 'C', 'D'], 1),
                        ("What is the longest name of a place in New Zealand?\nA. Taumata­whakatangihanga­koauau­o­tamatea­turi­pukaka­piki­maunga­horo­nuku­pokai­whenua­ki­tana­tahu\nB. Chargoggagoggmanchauggauggagoggchaubunagungamaugg\nC. Captain Cook Hawkes Bay Port\nD. Tane Mahuta Walk", ['A'], ['B', 'C', 'D'], 1),
                        ("What does aroha mean?\nA. Good\nB. Terrible\nC. Effort\nD. Love", ['D'], ['A', 'B', 'C'], 1),
                        ("True or False: The Treaty Of Waitangi was signed in 1845?\nT. True\nF. False", ['F'], ['T'], 1),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y', 'N'], [], 1),
                    ]),
                    'H' => (char.ToUpper(diffi[0]), [
                        ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D'], 1),
                        ("Did you enjoy?\nY. Yes\nN. No", ['Y'], ['N'], 1),
                    ]),
                    'Q' => (char.ToUpper(diffi[0]), [
                        ("Really Quit? (Y/N)", ['Y'], ['N'], 1)
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
                Console.Write($"{StringHelper.RGBIfy("Answer", (91, 217, 210))}: ");
                userInput = Console.ReadLine();
                if (userInput.Length != 1 || !questions.Item3.Contains(char.ToUpper(userInput[0])) && !questions.Item2.Contains(char.ToUpper(userInput[0])))
                {
                    Console.WriteLine("Invalid Answer.\n");
                }
            } while (userInput.Length != 1 || !questions.Item3.Contains(char.ToUpper(userInput[0])) && !questions.Item2.Contains(char.ToUpper(userInput[0])));
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
        public static string RGBIfy(string text, RGBColour col, bool reset = true)
        {
            string resulttext = "\x1b[38;2;" + col.Item1 + ";" + col.Item2 + ";" + col.Item3 + "m" + text;
            if (reset == true)
            {
                resulttext += "\e[0m";
            }
            return resulttext;
        }

        //resets formatting, either by outputting reset code or by returning the reset code to something
        public static string ResetFormatting(bool returnInstead)
        {
            if (returnInstead) { return "\e[0m"; }
            else { Console.WriteLine("\e[0m"); return ""; }
        }

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
