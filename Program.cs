#nullable disable

using System.Text.RegularExpressions;

using Question = (string QuestionString, System.Collections.Generic.List<char> CorrectAnswers, System.Collections.Generic.List<char> IncorrectAnswers, float Points);
// questions, answer chars, incorrect chars, points awarded for correct
using RGBColour = (int R, int G, int B);
using Scoredict = System.Collections.Generic.Dictionary<char, float>;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(/*string[] args*/)
        {
            //initialize vars
            string name;
            bool isCorrect;
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
                Console.WriteLine("""
                    Welcome, {0}!
                    Choose a difficulty:
                    {2}
                    {3}
                    {4}
                    {5}{1}
                    """,
                    name,
                    StringHelper.RGBIfy("Quit [Q]", (123, 0, 217), reset: true),
                    StringHelper.RGBIfy($"Easy [E]\t(High Score: {GetHighscoreOrZero(highscores, 'E')}/{GetTotalQuizPoints(GetQuizQuestions("E").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'E') / GetTotalQuizPoints(GetQuizQuestions("E").Item2)) * 100)}% Correct)", (0, 255, 0), reset: true),
                    StringHelper.RGBIfy($"Medium [M]\t(High Score: {GetHighscoreOrZero(highscores, 'M')}/{GetTotalQuizPoints(GetQuizQuestions("M").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'M') / GetTotalQuizPoints(GetQuizQuestions("M").Item2)) * 100)}% Correct)", (255, 255, 0), reset: true),
                    StringHelper.RGBIfy($"Hard [H]\t(High Score: {GetHighscoreOrZero(highscores, 'H')}/{GetTotalQuizPoints(GetQuizQuestions("H").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'H') / GetTotalQuizPoints(GetQuizQuestions("H").Item2)) * 100)}% Correct)", (255, 0, 0), reset: true),
                    (highscores.ContainsKey('S')) ? StringHelper.RGBIfy($"Secret [S]\t(High Score: {GetHighscoreOrZero(highscores, 'S')}/{GetTotalQuizPoints(GetQuizQuestions("S").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'S') / GetTotalQuizPoints(GetQuizQuestions("S").Item2)) * 100)}% Correct)\n", (199, 0, 255), reset: true) : ""
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
                for (int i = 0; i < chosenDifficulty.Item2.Count; i++)
                {
                    if (chosenDifficulty.Item1 != 'Q')
                    {
                        Console.Write(StringHelper.RGBIfy($"Question {i + 1}: ", (217, 72, 0)));
                        isCorrect = AskQuestion(chosenDifficulty.Item2[i]);
                        if (isCorrect) { Console.WriteLine(StringHelper.Fancify("Correct!\n", colorNum: 32)); score += chosenDifficulty.Item2[i].Points; }
                        else Console.Write(StringHelper.Fancify("Incorrect!\n", colorNum: 31));
                        if (!isCorrect || chosenDifficulty.Item2[i].CorrectAnswers.Count > 1)
                        {
                            Console.Write($"{StringHelper.RGBIfy("Correct Answer(s)", (255, 255, 0))}: [");
                            for (int j = 0; j < chosenDifficulty.Item2[i].CorrectAnswers.Count; j++)
                            {
                                Console.Write($"{chosenDifficulty.Item2[i].CorrectAnswers[j]}");
                                if (j != chosenDifficulty.Item2[i].CorrectAnswers.Count - 1) { Console.Write(", "); }
                            }
                            Console.WriteLine("]\n");
                        }
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
                    Console.WriteLine($"Score: {score}\tPercent: {Math.Round((score / GetTotalQuizPoints(chosenDifficulty.Item2)) * 100)}%");


                    //ask if replaying or not
                    do
                    {
                        Console.Write($"Would you like to replay [Y/N]?\n{StringHelper.RGBIfy("Option", (91, 217, 210))}: ");
                        replaychoice = Console.ReadLine().ToUpper();
                        if (replaychoice.Length == 1)
                        {
                            if (replaychoice[0] == 'Y')
                            {
                                replay = true;
                            }
                            else if (replaychoice[0] == 'N')
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
                    } while (!(new List<string> { "Y", "N" }.Contains(replaychoice)));
                }
                else
                {
                    replay = true;
                }
            } while (replay);
        }

        //return a high score, if there is none, return zero
        static float GetHighscoreOrZero(Scoredict scores, char difficulty) => scores.ContainsKey(difficulty) ? scores[difficulty] : 0;

        //returns quiz questions
        static (char, List<Question>) GetQuizQuestions(string diffi)
        {
            var rnd = new Random(); // so that rnd.Next works later when shuffling questions before return
            List<Question> theQuiz = [];

            if (diffi.Length == 1)
            {
                switch (char.ToUpper(diffi[0]))
                {
                    case 'E':
                        theQuiz = [
                            ("What does kia ora mean?\nA. Hello\nB. Good Morning\nC. Good Night\nD. I'm Hungry", ['A'], ['B', 'C', 'D'], 1),
                            ("What is the Maori name for New Zealand?\nA. Kaitiakitanga\nB. Tawhirimatea\nC. Aotearoa\nD. Whitu", ['C'], ['A', 'B', 'D'], 1),
                            ("Who was the prime minister in 2026?\nA. Christopher Luxon\nB. Winston Peters\nC. Martin Luther King Jr.\nD. Joe Biden", ['A'], ['B', 'C', 'D'], 1),
                        ];
                        return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())]);

                    case 'M':
                        theQuiz = [
                            ("What is the capital of New Zealand?\nA. Christchurch\nB. Wellington\nC. Auckland\nD. Hamilton", ['B'], ['A', 'C', 'D'], 1),
                            ("Which of these is a place in New Zealand and has the longest name?\nA. Taumatawhakatangihangakoauauotamateaturipukakapikimaungahoronukupokaiwhenuakitanatahu\nB. Chargoggagoggmanchauggauggagoggchaubunagungamaugg\nC. Captain Cook Hawkes Bay Port\nD. Tane Mahuta Walk", ['A'], ['B', 'C', 'D'], 1),
                            ("What does aroha mean?\nA. Good\nB. Terrible\nC. Effort\nD. Love", ['D'], ['A', 'B', 'C'], 1),
                            ("True or False: The Treaty Of Waitangi was signed in 1845?\nT. True\nF. False", ['F'], ['T'], 1),
                        ];
                        return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())]);

                    case 'H':
                        theQuiz = [
                            ("Which of these bird is native to New Zealand and is extinct?\nA. Kiwi\nB. Moa\nC. Emu\nD. Dodo", ['B'], ['A', 'C', 'D'], 1),
                            ("Which of these is a reptile native to New Zealand?\nA. Charlie\nB. Karakia\nC. Tuatara\nD. Aurora Borealis", ['C'], ['A', 'B', 'D'], 1),
                            ("(Worth double points) What is the Maori word for door?\nA. Doa\nB. Tatau\nC. Cacao\nD. Matao", ['B'], ['A', 'C', 'D'], 2),
                            ("What is the Maori word for stage?\nA. Atamira\nB. Whitu\nC. Stage\nD. Whare", ['A'], ['A', 'C', 'D'], 1),
                            ("What does koura translate to?\nA. Silver\nB. Yellow\nC. Gold\nD. Tattoo", ['C'], ['A', 'B', 'D'], 1),
                            ("What does pepa translate to?\nA. Pig\nB. Cling\nC. Pepper\nD. Paper", ['D'], ['A', 'B', 'C'], 1),
                        ];
                        return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())]);

                    case 'S':
                        //these are the questions for the secret difficulty
                        theQuiz = [
                            ("Which of these people helped translate the Treaty of Waitangi?\nA. Mike Tyson\nB. George Washington\nC. Henry Williams\nD. John McDonald", ['C'], ['A', 'B', 'D'], 1),
                        ];
                        return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())]);

                    case 'Q':
                        return (char.ToUpper(diffi[0]), [
                        ("Really Quit? (Y/N)", ['Y'], ['N'], 1)
                        ]);
                    default: return ('♣', []);
                }
                ;
            }
            return ('♣', []);
        }

        //gets the users answer for a question and checks if its correct
        static bool AskQuestion(Question questions)
        {
            string userInput;
            Console.WriteLine(questions.QuestionString);
            do
            {
                Console.Write($"{StringHelper.RGBIfy("Answer", (91, 217, 210))}: ");
                userInput = Console.ReadLine();
                if (userInput.Length != 1 || !questions.IncorrectAnswers.Contains(char.ToUpper(userInput[0])) && !questions.CorrectAnswers.Contains(char.ToUpper(userInput[0])))
                {
                    Console.WriteLine("Invalid Answer.\n");
                }
            } while (userInput.Length != 1 || !(questions.IncorrectAnswers.Contains(char.ToUpper(userInput[0])) || questions.CorrectAnswers.Contains(char.ToUpper(userInput[0]))));
            return questions.CorrectAnswers.Contains(char.ToUpper(userInput[0]));
        }

        //returns the amount of points you can possibly get from one quiz
        static float GetTotalQuizPoints(List<Question> questions)
        {
            float total = 0;
            foreach (Question questio in questions)
            {
                total += questio.Points;
            }
            return total;
        }
    }

    //class for functions to help with strings
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
            resetstring = (reset) ? $"\e[0m" : "";
            return $"{formatting}{text}{resetstring}";
        }

        //colours text by taking rgb input
        public static string RGBIfy(string text, RGBColour col, bool reset = true)
        {
            string resulttext = "\x1b[38;2;" + col.R + ";" + col.G + ";" + col.B + "m" + text;
            if (reset)
            {
                resulttext += "\e[0m";
            }
            return resulttext;
        }

        //resets formatting, either by outputting reset code or by returning the reset code to something
        public static string ResetFormatting(bool returnInstead)
        {
            if (returnInstead) return "\e[0m";
            else { Console.WriteLine("\e[0m"); return ""; }
        }

        //checks if a name is within 52 chars long, is at least 2 words, doesnt have double spacebars, doesnt have numbers, and any full stops must come after words rather than in or before
        //e.g. "George Harris Sr." is valid, ".Jr Mac" is invalid ("Jr. Mac" is valid), "Sheldon  Cooper" is invalid
        public static bool ValidName(string nameToTest)
        {
            Regex nameRegex = new Regex(@"([a-z]+\.? *)()+", RegexOptions.IgnoreCase);
            if (string.Join("", nameRegex.Matches(nameToTest)).Length == nameToTest.Length && nameToTest != "" && nameToTest.Length <= 52 && nameToTest.Contains(' '))
            {
                int count = nameToTest.Count(c => c == ' ');
                if (count + 1 == nameRegex.Matches(nameToTest).Count) { return true; }
            }
            return false;
        }

        //capitalize first letters of words in string, otherwise decapitalizes them (this is to correct wrong capitalization, "mICheal JoRdAN" becomes "Micheal Jordan")
        public static string Capitalize(string stringToCapitalize) => Regex.Replace(Regex.Replace(stringToCapitalize, @"\b[a-z]", delegate (Match match)
            {
                return match.ToString().ToUpper();
            }, RegexOptions.IgnoreCase), @"(?!\b)[A-Z]", delegate (Match match)
            {
                return match.ToString().ToLower();
            });
    }

    //class which is kind of unnecessary
    public static class ConsoleHelper
    {
        //clears console (do i have to explain)
        public static void ClearFullConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J");
        }
    }
}
