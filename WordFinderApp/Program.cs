using WordFinderLib;

namespace WordFinderApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool continueRunning = true;

            while (continueRunning)
            {
                Console.Clear();
                Console.WriteLine("Choose data source:");
                Console.WriteLine("1. Use default mock data");
                Console.WriteLine("2. Load data from files (files must be in the current directory)");
                Console.WriteLine("3. Enter data interactively");
                Console.WriteLine("4. Quit");
                Console.Write("Enter your choice (1, 2, 3, or 4): ");

                var choice = Console.ReadLine();
                List<string>? matrix;
                IEnumerable<string>? wordStream;
                switch (choice)
                {
                    case "1":
                        matrix = GetDefaultMatrix();
                        wordStream = GetDefaultWordStream();
                        break;

                    case "2":
                        ShowFileInstructions();
                        Console.WriteLine("Please ensure that the files are in the specified location.");
                        Console.WriteLine("Press any key to continue after placing the files...");
                        Console.ReadKey();

                        matrix = LoadMatrixFromFile("matrix.txt");
                        wordStream = LoadWordStreamFromFile("wordstream.txt");

                        if (matrix == null || wordStream == null)
                        {
                            Console.WriteLine("Error: Required files are missing or empty. Please check and try again.");
                            Console.WriteLine("Press any key to return to the main menu...");
                            Console.ReadKey();
                            continue;
                        }
                        break;

                    case "3":
                        matrix = GetMatrixFromUserInput();
                        wordStream = GetWordStreamFromUserInput();
                        break;

                    case "4":
                        continueRunning = false;
                        Console.WriteLine("Exiting the program...");
                        continue;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        continue;
                }

                if (matrix == null || wordStream == null)
                {
                    Console.WriteLine("Matrix or Word Stream could not be loaded. Please try again.");
                    continue;
                }

                try
                {
                    var wordFinder = new WordFinder(matrix);
                    var cts = new CancellationTokenSource();
                    var progressTask = ShowProgressAsync(cts.Token);

                    var foundWords = await wordFinder.FindAsync(wordStream, cts.Token);
                    cts.Cancel();
                    await progressTask;

                    PrintMatrixWithHighlightedWords(matrix, foundWords);

                    Console.WriteLine("\nWords found in the matrix:");
                    foreach (var word in foundWords)
                        Console.WriteLine(word);
                }
                catch (Exception ex)
                {
                    var message = ex switch
                    {
                        ArgumentException => $"\nError: {ex.Message}",
                        OperationCanceledException => "\nOperation was canceled.",
                        _ => $"\nAn unexpected error occurred: {ex.Message}. Please try again."
                    };

                    Console.WriteLine(message);
                }

                Console.WriteLine("\nPress any key to restart, or 'q' to quit...");
                var restartChoice = Console.ReadKey().KeyChar;
                if (char.ToLower(restartChoice) == 'q')
                    continueRunning = false;
            }
        }

        static void ShowFileInstructions()
        {
            var directory = Directory.GetCurrentDirectory();
            Console.WriteLine("\nPlease place the following files in the directory:");
            Console.WriteLine($"{directory}");
            Console.WriteLine("- matrix.txt (for the matrix)");
            Console.WriteLine("- wordstream.txt (for the word stream)");
            Console.WriteLine();
        }

        static List<string> GetDefaultMatrix()
        {
            return
            [
                "hellothereapples",
                "xbtgzoilluminatt",
                "randomhcomputers",
                "sunshineqrsolver",
                "catdogswimagames",
                "freedomfcodejava",
                "qwersolveproblem",
                "clouddevpprogram",
                "extratermnilogyv",
                "obstaclecomplexx",
                "engineercoffeesa",
                "knowledgewordsdd",
                "pythonmysticodff",
                "developerxkeyzyt",
                "datastructuretyt"
            ];
        }

        static List<string> GetDefaultWordStream()
        {
            return
            [
                "hello", "apple", "computer", "java", "python", "knowledge", "freedom", "developer"
            ];
        }

        static List<string>? LoadMatrixFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Error: File '{fileName}' not found in the current directory.");
                return null;
            }

            var lines = File.ReadAllLines(fileName).ToList();
            if (lines.Count == 0)
            {
                Console.WriteLine($"Error: File '{fileName}' is empty.");
                return null;
            }

            return lines;
        }

        static IEnumerable<string>? LoadWordStreamFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Error: File '{fileName}' not found in the current directory.");
                yield break;
            }

            using var reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
        }

        static List<string> GetMatrixFromUserInput()
        {
            Console.WriteLine("Enter your matrix rows one by one. Press 'Enter' on an empty line to finish:");

            var matrix = new List<string>();
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                matrix.Add(line);
            }

            return matrix;
        }

        static List<string> GetWordStreamFromUserInput()
        {
            Console.WriteLine("Enter words for word stream one by one. Press 'Enter' on an empty line to finish:");

            var wordStream = new List<string>();
            while (true)
            {
                var word = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(word)) break;
                wordStream.Add(word);
            }

            return wordStream;
        }

        static void PrintMatrixWithHighlightedWords(List<string> matrix, List<string> foundWords)
        {
            Console.WriteLine("\nMatrix with highlighted words:");

            var spacedMatrix = matrix.Select(row => string.Join(" ", row.ToCharArray())).ToList();

            foreach (var row in spacedMatrix)
            {
                string highlightedRow = row;

                foreach (var word in foundWords.OrderByDescending(w => w.Length))
                {
                    string spacedWord = string.Join(" ", word.ToCharArray());
                    if (highlightedRow.Contains(spacedWord))
                    {
                        highlightedRow = highlightedRow.Replace(spacedWord, spacedWord.ToUpper());
                    }
                }

                Console.WriteLine(highlightedRow);
            }
        }

        static async Task ShowProgressAsync(CancellationToken token)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int counter = 0;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    Console.Write($"\rProcessing words... {spinner[counter]}");
                    counter = (counter + 1) % spinner.Length;
                    await Task.Delay(100, token);
                }
            }
            catch (TaskCanceledException)
            {
                // This catch block is used to handle the cancellation of the task
            }
            finally
            {
                Console.Write("\rProcessing words... done.    \n");
            }
        }
    }
}
