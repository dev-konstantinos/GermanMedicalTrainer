namespace MainApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Welcome to the German Medical Synonyms Trainer (C1)\n");

            var wordData = new WordData();
            Random random = new Random();

            int score = 0; // Points counter
            int attempts = 0; // Attempts counter
            Queue<string> previousKeys = new Queue<string>(2); // Queue to store the last two keys

            while (true)
            {
                Console.WriteLine("\nType the correct synonym or press Q to quit\n");

                // Generate a random key that was not used in the last two rounds
                string key;
                do
                {
                    key = wordData.WordsDict.Keys.ElementAt(random.Next(0, wordData.WordsDict.Count));
                }
                while (previousKeys.Contains(key)); // If the key was used in the last two rounds, generate a new one

                // Add the key to the queue
                if (previousKeys.Count == 2)
                {
                    previousKeys.Dequeue(); // Remove the oldest element if the queue already contains 2 elements
                }
                previousKeys.Enqueue(key); // Add the current key to the queue

                // Get translations for the key
                var translations = wordData.WordsDict[key];
                List<string> answers = new List<string>();

                // Correct answer
                string correctAnswer = "";
                if (translations is string)
                {
                    correctAnswer = (string)translations;
                    answers.Add(correctAnswer);
                }
                else if (translations is string[])
                {
                    correctAnswer = string.Join(",", (string[])translations);
                    answers.Add(correctAnswer);
                }

                // Generate wrong answers
                while (answers.Count < 4)
                {
                    var wrongKey = wordData.WordsDict.Keys.ElementAt(random.Next(0, wordData.WordsDict.Count));
                    var wrongTranslations = wordData.WordsDict[wrongKey];

                    string wrongAnswer = "";
                    if (wrongTranslations is string)
                    {
                        wrongAnswer = (string)wrongTranslations;
                    }
                    else if (wrongTranslations is string[])
                    {
                        wrongAnswer = string.Join(",", (string[])wrongTranslations);
                    }

                    if (!answers.Contains(wrongAnswer))
                    {
                        answers.Add(wrongAnswer);
                    }
                }

                // Shuffle answers
                answers = answers.OrderBy(x => random.Next()).ToList();

                // Display synonym options
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"'{key}':\n");
                Console.ResetColor();
                for (int i = 0; i < answers.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{i + 1}. {answers[i]}");
                    Console.ResetColor();
                }

                // User input
                Console.WriteLine("\nEnter your answer (number or text) or 'q' to quit:");
                string userInput = Console.ReadLine().Trim();

                if (userInput.ToLower() == "q")
                {
                    break;
                }

                bool isCorrect = false;

                // Check numeric input (answer number)
                if (int.TryParse(userInput, out int answerNumber) && answerNumber >= 1 && answerNumber <= 4)
                {
                    if (answers[answerNumber - 1] == correctAnswer)
                    {
                        isCorrect = true;
                    }
                }
                // Check text input (string)
                else
                {
                    string[] userAnswers = userInput.Split(',')
                        .Select(a => a.Trim().ToLower())
                        .ToArray();

                    string[] correctAnswers = correctAnswer.Split(',')
                        .Select(a => a.Trim().ToLower())
                        .ToArray();

                    isCorrect = userAnswers.SequenceEqual(correctAnswers);
                }

                // Show result
                if (isCorrect)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("CORRECT!\n");
                    Console.ResetColor();
                    score++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WRONG.\nCorrect answer: " + correctAnswer + "\n");
                    Console.ResetColor();
                }

                attempts++; // Increase attempts counter

                // Show current score and attempts
                Console.WriteLine($"Your score: {score}/{attempts}\n");
                Thread.Sleep(2000); // Delay before the next question
            }
        }
    }
}
