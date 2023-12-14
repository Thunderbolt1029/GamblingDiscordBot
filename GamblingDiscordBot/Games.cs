using System;

namespace GamblingDiscordBot
{
    class Games
    {
        void HigherLower()
        {
            Random random = new Random();

            int computerSelection = random.Next(100) + 1;
            int userSelection;
            int tries = 0;

            do
            {
                Console.Write("Guess a number between 1 and 100: ");
                try
                {
                    userSelection = int.Parse(Console.ReadLine());

                    if (userSelection < 1 || userSelection > 100)
                    {
                        Console.WriteLine("Not between 1 and 100");
                    }
                    else if (userSelection < computerSelection)
                    {
                        Console.WriteLine("Higher");
                        tries++;
                    }
                    else if (userSelection > computerSelection)
                    {
                        Console.WriteLine("Lower");
                        tries++;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Not an integer");
                    userSelection = -1;
                }                
            } while (userSelection != computerSelection);

            Console.WriteLine($"You got it in {tries} tries");
            Console.ReadLine();
        }
    }
}
