#nullable disable // helps with not getting warnings for Console.ReadLine(), and im not using nullable types anyways (i think? nullable types are like "Type?" instead of "Type" )

using System.Text.RegularExpressions;

// alias for a very long type
using Question = (string QuestionString, System.Collections.Generic.List<char> CorrectAnswers, System.Collections.Generic.List<char> IncorrectAnswers, float Points);
//                question,              correct answer chars,                                 incorrect options chars/all option chars,               points awarded for correct answer

// alias for rgb colours, the r, g and b obviously stand for red, green and blue
// this is used for colouring text with StringHelper.RGBIfy()
using RGBColour = (int R, int G, int B);
using Scoredict = System.Collections.Generic.Dictionary<char, float>;

namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(/*string[] args*/) // dont really need any args (at least for now)
        {
            // initialize vars
            string name;
            bool isCorrect;
            float score;
            bool replay = false;
            string replaychoice;
            Scoredict highscores = [];
            (char, List<Question>, RGBColour) chosenDifficulty;
            // the char in this type is to tell high scores what key to put the score under
            // RGBColour colour here is so that the difficulty has a colour correlated to it, which is used when outputting the diffculty name to the terminal

            ConsoleHelper.ClearFullConsole();

            // ask for name
            do
            {
                Console.Write($"Please enter your first name: ");
                name = StringHelper.Capitalize(Console.ReadLine().Trim());
                if (!StringHelper.ValidFirstName(name))
                {
                    Console.WriteLine("Not a valid first name.\n");
                }
            } while (!StringHelper.ValidFirstName(name));

            do
            {
                ConsoleHelper.ClearFullConsole();
                score = 0;
                // print difficulties, highscores, and what percent of questions were right from highscore, with rgb colouring
                Console.WriteLine("""
                    {0}
                    Choose a difficulty:
                    {2}
                    {3}
                    {4}
                    {5}{1}
                    """,
                    (IsQuizMaster(highscores)) ? $"Congrats, {name}! You have aced every quiz difficulty!" : $"Welcome to the Maori/NZ Quiz, {name}!", // check if user is master of quizes (has every question right on every difficulty), congratulates them if they are, welcomes them if not
                    StringHelper.RGBIfy("Quit [Q]", GetQuizQuestions("Q").Item3, reset: true),
                    StringHelper.RGBIfy($"Easy [E]\t(High Score: {GetHighscoreOrZero(highscores, 'E')}/{GetTotalQuizPoints(GetQuizQuestions("E").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'E') / GetTotalQuizPoints(GetQuizQuestions("E").Item2)) * 100)}% Correct)", GetQuizQuestions("E").Item3, reset: true),
                    StringHelper.RGBIfy($"Medium [M]\t(High Score: {GetHighscoreOrZero(highscores, 'M')}/{GetTotalQuizPoints(GetQuizQuestions("M").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'M') / GetTotalQuizPoints(GetQuizQuestions("M").Item2)) * 100)}% Correct)", GetQuizQuestions("M").Item3, reset: true),
                    StringHelper.RGBIfy($"Hard [H]\t(High Score: {GetHighscoreOrZero(highscores, 'H')}/{GetTotalQuizPoints(GetQuizQuestions("H").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'H') / GetTotalQuizPoints(GetQuizQuestions("H").Item2)) * 100)}% Correct)", GetQuizQuestions("H").Item3, reset: true),
                    (highscores.ContainsKey('S')) ? StringHelper.RGBIfy($"Secret [S]\t(High Score: {GetHighscoreOrZero(highscores, 'S')}/{GetTotalQuizPoints(GetQuizQuestions("S").Item2)}, {Math.Round((GetHighscoreOrZero(highscores, 'S') / GetTotalQuizPoints(GetQuizQuestions("S").Item2)) * 100)}% Correct)\n", GetQuizQuestions("S").Item3, reset: true) : ""
                );

                // pick a difficulty
                do
                {
                    Console.Write($"{StringHelper.RGBIfy("Choice", (91, 217, 210))}: ");
                    chosenDifficulty = GetQuizQuestions(Console.ReadLine().Trim()); // parsing chosen difficulty to GetQuizQuestions as a string
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
                        Console.Write(StringHelper.RGBIfy($"Question {i + 1}: ", chosenDifficulty.Item3));
                        isCorrect = AskQuestion(chosenDifficulty.Item2[i]);
                        if (isCorrect) { Console.WriteLine(StringHelper.Fancify($"Correct! (+{chosenDifficulty.Item2[i].Points} points)\n", colorNum: 32)); score += chosenDifficulty.Item2[i].Points; }
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


                // quiz score/highscore related things
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


                    // ask if replaying or not
                    do
                    {
                        Console.Write($"Would you like to replay [Y/N]?\n{StringHelper.RGBIfy("Option", (91, 217, 210))}: ");
                        replaychoice = Console.ReadLine().Trim().ToUpper();
                        if (replaychoice == "Y")
                        {
                            replay = true;
                        }
                        else if (replaychoice == "N")
                        {
                            replay = false;
                        }
                        else
                        {
                            Console.WriteLine("Invalid Option.\n");
                        }
                    } while (!(new string[] { "Y", "N" }.Contains(replaychoice)));
                }
                else
                {
                    replay = true;
                }
            } while (replay);
        }

        // check dictionary containing high scores per difficulty, if it doesnt contain the difficulty as a key, say that the high scores is 0
        static float GetHighscoreOrZero(Scoredict scores, char difficulty) => scores.ContainsKey(difficulty) ? scores[difficulty] : 0;


        // returns quiz questions (will make colour correlated to quiz later)
        static (char, List<Question>, RGBColour) GetQuizQuestions(string diffi) // diffi is short for difficulty, which in this case is a string supplied by the user, which is check if it is one letter, if it is, then it checks if the letter correlates to a difficulty
        {
            var rnd = new Random(); // so that rnd.Next works later when shuffling questions before return
            List<Question> theQuiz = [];

            switch (diffi.ToUpper())
            {
                // make sure that every possible option is either in the correct answer char list, incorrect answer char list, or both (will count as a correct answer if in both)
                case "E":
                    theQuiz = [
                        ("What does kia ora mean?\nA. Good Morning\nB. Hello\nC. Good Night\nD. I'm Hungry", ['B'], ['A', 'B', 'C', 'D'], 1),
                            ("What is the Maori name for New Zealand?\nA. Kaitiakitanga\nB. Tawhirimatea\nC. Aotearoa\nD. Whitu", ['C'], ['A', 'B', 'C', 'D'], 1),
                            ("Who was the prime minister in 2026?\nA. Christopher Luxon\nB. Winston Peters\nC. Martin Luther King Jr.\nD. Joe Biden", ['A'], ['A', 'B', 'C', 'D'], 1),
                            ("What does ma translate to?\nA. Black\nB. Father\nC. Mother\nD. White", ['D'], ['A', 'B', 'C', 'D'], 1),
                            ("What does kakariki translate to? (Double Point Question!)\nA. Green\nB. Yellow\nC. Purple\nD. White", ['A'], ['A', 'B', 'C', 'D'], 2),
                        ];
                    return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())], (0, 255, 0)); // rnd.Next returns a random int32, so .OrderBy sorts the list by which items have the highest numbers assigned to them

                case "M":
                    theQuiz = [
                        ("What is the capital of New Zealand?\nA. Christchurch\nB. Wellington\nC. Auckland\nD. Hamilton", ['B'], ['A', 'B', 'C', 'D'], 1),
                            ("What is the steepest street in New Zealand?\nA. Harry Street\nB. Third Steet\nC. Baldwin Street\nD. Tuff Street", ['C'], ['A', 'B', 'C', 'D'], 1),
                            ("What does aroha mean?\nA. Good\nB. Terrible\nC. Effort\nD. Love", ['D'], ['A', 'B', 'C', 'D'], 1),
                            ("True or False: The Treaty Of Waitangi was signed in 1845? (Double Point Question!)\nT. True\nF. False", ['F'], ['T', 'F'], 2),
                        ];
                    return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())], (255, 255, 0)); // ditto

                case "H":
                    theQuiz = [
                        ("Which of these birds is native to New Zealand and is extinct?\nA. Kiwi\nB. Moa\nC. Emu\nD. Dodo", ['B'], ['A', 'B', 'C', 'D'], 1),
                            ("Which of these is a reptile native to New Zealand?\nA. Charlie\nB. Karakia\nC. Tuatara\nD. Aurora Borealis", ['C'], ['A', 'B', 'C', 'D'], 1),
                            ("What is the Maori word for door? (Double Point Question!)\nA. Doa\nB. Tatau\nC. Cacao\nD. Matao", ['B'], ['A', 'B', 'C', 'D'], 2),
                            ("What is the Maori word for stage?\nA. Atamira\nB. Whitu\nC. Stage\nD. Whare", ['A'], ['A', 'B', 'C', 'D'], 1),
                            ("What does koura translate to?\nA. Silver\nB. Yellow\nC. Gold\nD. Tattoo", ['C'], ['A', 'B', 'C', 'D'], 1),
                            ("What does pepa translate to?\nA. Pig\nB. Cling\nC. Pepper\nD. Paper", ['D'], ['A', 'B', 'C', 'D'], 1),
                        ];
                    return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())], (255, 0, 0));

                case "S":
                    // these are the questions for the secret difficulty
                    theQuiz = [
                        ("Which of these people helped translate the Treaty of Waitangi? (Triple Point Question!)\nA. Mike Tyson\nB. George Washington\nC. Henry Williams\nD. John McDonald", ['C'], ['A', 'B', 'C', 'D'], 3),
                            ("What year was Aotearoa discovered? (Worth 10 Points!)\nA. ~1750\nB. ~1580\nC. ~1225\nD. ~1280", ['D'], ['A', 'B', 'C', 'D'], 10),
                        ];
                    return (char.ToUpper(diffi[0]), [.. theQuiz.OrderBy(item => rnd.Next())], (199, 0, 255));

                case "Q":
                    return (char.ToUpper(diffi[0]), [
                    ("Really Quit? (Y/N)", ['Y'], ['N'], 1)
                    ], (123, 0, 217));
                default: return ('♣', [], (0, 0, 0));
            }
        }

        // gets the users answer for a question and checks if its correct
        static bool AskQuestion(Question questions)
        {
            string userInput;
            Console.WriteLine(questions.QuestionString);
            do
            {
                Console.Write($"{StringHelper.RGBIfy("Answer", (91, 217, 210))}: ");
                userInput = Console.ReadLine().Trim();
                if (userInput.Length != 1 || !questions.IncorrectAnswers.Contains(char.ToUpper(userInput[0])) && !questions.CorrectAnswers.Contains(char.ToUpper(userInput[0])))
                {
                    Console.WriteLine("Invalid Answer.\n");
                }
            } while (userInput.Length != 1 || !(questions.IncorrectAnswers.Contains(char.ToUpper(userInput[0])) || questions.CorrectAnswers.Contains(char.ToUpper(userInput[0]))));
            return questions.CorrectAnswers.Contains(char.ToUpper(userInput[0]));
        }

        // returns the amount of points you can possibly get from one quiz
        static float GetTotalQuizPoints(List<Question> questions)
        {
            float total = 0;
            foreach (Question questio in questions)
            {
                if (questio.Points > 0)
                {
                    total += questio.Points;
                }
            }
            return total;
        }

        // checks if user has every possible point for every possible difficulty
        static bool IsQuizMaster(Scoredict scores) =>
            GetHighscoreOrZero(scores, 'E') == GetTotalQuizPoints(GetQuizQuestions("E").Item2) &&
            GetHighscoreOrZero(scores, 'M') == GetTotalQuizPoints(GetQuizQuestions("M").Item2) &&
            GetHighscoreOrZero(scores, 'H') == GetTotalQuizPoints(GetQuizQuestions("H").Item2) &&
            GetHighscoreOrZero(scores, 'S') == GetTotalQuizPoints(GetQuizQuestions("S").Item2);
    }

    // class for functions to help with strings
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
        method that makes ansi formatting easier for other functions
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

        public static void SlowTyper(string inputString, int interval)
        {
            foreach (char letter in inputString)
            {
                Console.Write(letter);
                Thread.Sleep(interval);
            }
        }

        // colours text by taking rgb input
        public static string RGBIfy(string text, RGBColour col, bool reset = true)
        {
            string resulttext = "\x1b[38;2;" + col.R + ";" + col.G + ";" + col.B + "m" + text;
            if (reset)
            {
                resulttext += "\e[0m";
            }
            return resulttext;
        }

        // resets formatting, either by outputting reset code or by returning the reset code to something
        public static string ResetFormatting(bool returnInstead)
        {
            if (returnInstead) return "\e[0m";
            else { Console.Write("\e[0m"); return ""; }
        }

        // checks if name is one word, with no numbers, and is within the length of 1 to 21 characters
        public static bool ValidFirstName(string nameToTest) => Regex.Matches(nameToTest, @"[a-z]{1,21}", RegexOptions.IgnoreCase).Count == 1 && nameToTest.Length == Regex.Matches(nameToTest, @"[a-z]{1,21}", RegexOptions.IgnoreCase)[0].Length;

        // capitalize first letters of words in string, otherwise decapitalizes them (this is to correct wrong capitalization, "mICheal JoRdAN" becomes "Micheal Jordan")
        public static string Capitalize(string stringToCapitalize) => Regex.Replace(Regex.Replace(stringToCapitalize, @"\b[a-z]", delegate (Match match)
            {
                return match.ToString().ToUpper();
            }, RegexOptions.IgnoreCase), @"(?!\b)[A-Z]", delegate (Match match)
            {
                return match.ToString().ToLower();
            });
    }

    // class which is kind of unnecessary, but might be helpful if i make another function to help with consoles
    public static class ConsoleHelper
    {
        // clears console (do i have to explain)
        public static void ClearFullConsole()
        {
            Console.Clear();
            Console.Write("\x1b[3J");
        }
    }
}
