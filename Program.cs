namespace MaoriQuiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name;
            do
            {
                Console.Write("Enter Name: ");
                name = Console.ReadLine();
            } while (!ValidName(name));
        }

        static bool ValidName(string nameToTest) {
            return true;
        }
    }
}
