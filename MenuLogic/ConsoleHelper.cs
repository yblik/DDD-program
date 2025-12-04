using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DDD_program.MenuLogic
{
    public static class ConsoleHelper
    {
        public static int GetIntegerInRange(int pMin, int pMax, string pMessage)
        {

            int result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter a number between {pMin} and {pMax} inclusive.");

                string userInput = Console.ReadLine();

                try
                {
                    result = int.Parse(userInput);
                }
                catch
                {
                    Console.WriteLine($"{userInput} is not a number");
                    continue;
                }

                if (result >= pMin && result <= pMax)
                {
                    return result;
                }
                Console.WriteLine($"{result} is not between {pMin} and {pMax} inclusive.");
            } while (true);
        }
        //gets a time input from the user within a specified range
        public static TimeSpan GetTimeInRange(TimeSpan pMin, TimeSpan pMax, string pMessage)
        {
            // Ensure the minimum is not greater than the maximum (optional safety check)
            if (pMin > pMax)
            {
                throw new ArgumentException($"Minimum time {pMin} cannot be greater than maximum time {pMax}");
            }

            TimeSpan result;

            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine($"Please enter a time between {pMin:hh\\:mm} and {pMax:hh\\:mm} (24-hour format, e.g. 13:45):");

                string userInput = Console.ReadLine();

                // Attempt to parse the input as a TimeSpan
                if (!TimeSpan.TryParse(userInput, out result))
                {
                    Console.WriteLine($"{userInput} is not a valid time format.");
                    continue;
                }

                // Check if the parsed time is within the allowed range
                if (result >= pMin && result <= pMax)
                {
                    return result;
                }

                Console.WriteLine($"{result:hh\\:mm} is not between {pMin:hh\\:mm} and {pMax:hh\\:mm}.");
            } while (true);
        }

        //multiple choice menu selection
        public static int GetSelectionFromMenu(IEnumerable<string> items, string prompt)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(prompt);

            int itemNumber = 0;

            foreach (string item in items)
            {
                itemNumber++;
                sb.AppendLine($"{itemNumber}. {item}");
            }

            return GetIntegerInRange(1, itemNumber, sb.ToString()) - 1;
        }

        //gets non empty input from user
        public static string GetInput(string prompt)
        {
            string userInput;
            do
            {
                Console.WriteLine(prompt);
                userInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userInput)) //checks if there is actually stuff in
                {
                    return userInput;
                }
                Console.WriteLine("Input cannot be empty. Please try again.");
            } while (true);
        }

        //get input letters only
        public static string GetInputLettersOnly(string pMessage) //based on get input (same same but different)
        {
            string userInput;
            do
            {
                Console.WriteLine(pMessage);
                Console.WriteLine("Please enter only upper case and lower case:");
                userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Input cannot be empty. Please try again");
                    continue;
                }
                if (!userInput.All(c => char.IsLetter(c) || c == ' ')) //i mean this worked
                {
                    Console.WriteLine($"{userInput} contains invalid characters. Letters only!");
                    continue;
                }

                return userInput; // valid input
            } while (true);
        }
        public static string GetEmail(string prompt)
        {
            string userInput;
            do
            {
                //uses regex - start = ^, [^@\s] = any words or wierd chars,
                //\.com just .com, $ = end of string
                Console.WriteLine(prompt);
                userInput = Console.ReadLine();
                if (Regex.IsMatch(userInput, @"^[^@\s]+@+[^@\s]+\.com$")) //checks if in format _@_.com
                {
                    return userInput;
                }
                Console.WriteLine("Input is not in __@__.com format! Please try again.");
            } while (true);
        }
    }
}
