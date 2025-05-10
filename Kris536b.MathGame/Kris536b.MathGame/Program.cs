// Top level statements
using System.IO.Pipes;

Random dice = new Random();
bool playAgain = true;
int nextAction = 0;
int gameMode = 0;
int minValue = 0;
int maxValue = 0;
string operation = "";
string selectedDifficulty = "";
string selectedGameMode = "";
List<GameRecord> gameHistory = new List<GameRecord>();

// Game play loop
Console.Clear();

while (playAgain)
{
    Console.Clear();

    if (nextAction == 0 || nextAction == 2)
    {
        int mainChoice = MainMenu();

        if (mainChoice == 1) // Play game
        {
            int mode = GameModeSelection();
            if (mode == 6)
            {
                continue;
            }
            gameMode = mode;

            bool difficultyChosen = DifficultySelection();
            if (!difficultyChosen)
            {
                continue;
            }
        }
        else if (mainChoice == 2) // View history
        {
            ViewHistory();
            continue;
        }
        else if (mainChoice == 3) // Exit game
        {
            Console.WriteLine("Thank you for playing!");
            break;
        }
        else
        {
            continue;
        }
    }

    PlayGame(gameMode);

    nextAction = PlayAgainPrompt();

    if (nextAction == 3)
    {
        Console.WriteLine("Thank you for playing!");
        break;
    }
}

int MainMenu()
{
    while (true)
    {
        Console.WriteLine("Welcome to the Math Game!");
        Console.WriteLine("1. Play game");
        Console.WriteLine("2. View history");
        Console.WriteLine("3. Exit");

        string? choice = Console.ReadLine();
        int.TryParse(choice, out int menuChoice);

        if (menuChoice >= 1 && menuChoice <= 3)
            return menuChoice;

        Console.WriteLine("Invalid choice. Please try again.");
    }
}

int GameModeSelection()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Select Game Mode:");
        Console.WriteLine("1. Addition");
        Console.WriteLine("2. Subtraction");
        Console.WriteLine("3. Multiplication");
        Console.WriteLine("4. Division");
        Console.WriteLine("5. Mixed Operations");
        Console.WriteLine("6. Back to Main Menu");

        string? input = Console.ReadLine();
        int.TryParse(input, out int selectedMode);

        if (selectedMode >= 1 && selectedMode <= 6)
        {
            switch (selectedMode)
            {
                case 1:
                    operation = "+";
                    selectedGameMode = "Addition";
                    break;
                case 2:
                    operation = "-";
                    selectedGameMode = "Subtraction";
                    break;
                case 3:
                    operation = "*";
                    selectedGameMode = "Multiplication";
                    break;
                case 4:
                    operation = "/";
                    selectedGameMode = "Division";
                    break;
                case 5:
                    selectedGameMode = "Mixed Operations";
                    break;
            }

            return selectedMode;
        }

        Console.WriteLine("Invalid choice. Please try again.");
    }
}

bool DifficultySelection()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"You selected {selectedGameMode} mode.");
        Console.WriteLine("Select Difficulty Level:");
        Console.WriteLine("1. Easy (1-10)");
        Console.WriteLine("2. Medium (1-20)");
        Console.WriteLine("3. Hard (1-100)");
        Console.WriteLine("4. Back to Game Mode Selection");
        Console.WriteLine("5. Back to Main Menu");

        string? input = Console.ReadLine();
        int.TryParse(input, out int choice);

        switch (choice)
        {
            case 1:
                minValue = 1; maxValue = 10;
                selectedDifficulty = "Easy";
                return true;
            case 2:
                minValue = 1; maxValue = 20;
                selectedDifficulty = "Medium";
                return true;
            case 3:
                minValue = 1; maxValue = 100;
                selectedDifficulty = "Hard";
                return true;
            case 4:
                gameMode = GameModeSelection(); // Re-select game mode
                break;
            case 5:
                return false;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
}

void PlayGame(int gameMode)
{
    int score = 0;
    string[] operations = { "+", "-", "*", "/" };

    GameRecord record = new GameRecord
    {
        Date = DateTime.Now,
        Operation = gameMode == 5 ? "Mixed" : selectedGameMode,
        Difficulty = selectedDifficulty,
        Problems = new List<string>()
    };

    Console.Clear();
    Console.WriteLine("Press any key to start the game...");
    Console.ReadKey();
    Console.Clear();

    for (int i = 0; i < 5; i++)
    {
        int numOne = dice.Next(minValue, maxValue + 1);
        int numTwo = dice.Next(minValue, maxValue + 1);
        string currentOperation;
        int answer = 0;

        if (gameMode == 5)
        {
            currentOperation = operations[dice.Next(operations.Length)];
        }
        else
        {
            currentOperation = operation;
        }

        // Ensure no negative results for subtraction
        if (currentOperation == "-" && numOne < numTwo)
        {
            int temp = numOne;
            numOne = numTwo;
            numTwo = temp;
        }
        // Ensure no division by zero
        else if (currentOperation == "/")
        {
            numTwo = dice.Next(1, maxValue + 1); // Avoid zero
            answer = dice.Next(minValue, maxValue + 1);
            numOne = numTwo * answer;
        }

        Console.WriteLine($"{numOne} {currentOperation} {numTwo} = ?");
        string? input = Console.ReadLine();

        bool correct = false;

        if (int.TryParse(input, out answer))
        {
            if (CheckAnswer(answer, numOne, numTwo, currentOperation))
            {
                score++;
                correct = true;
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            i--; // Decrement i to repeat the question
        }

        string result = correct ? "Correct" : "Wrong";
        string problem = $"{numOne} {currentOperation} {numTwo} = {answer} ({result})";
        record.Problems.Add(problem);
    }

    Console.WriteLine($"Your score is {score} out of 5.");
    Console.WriteLine();

    record.Score = score;
    gameHistory.Add(record);

}

static int PlayAgainPrompt()
{
    while (true)
    {
        Console.WriteLine("Do you want to play again?");
        Console.WriteLine("1. Play again");
        Console.WriteLine("2. Back to Main Menu");
        Console.WriteLine("3. Exit game");

        string? input = Console.ReadLine();

        if (int.TryParse(input, out int choice))
        {
            if (choice >= 1 && choice <= 3)
            {
                return choice;
            }
        }

        Console.WriteLine("Invalid input. Please try again.");
    }
}

void ViewHistory()
{
    Console.Clear();

    if (gameHistory.Count == 0)
    {
        Console.WriteLine("No game history found.");
    }

    Console.WriteLine("Game History:");

    foreach (var record in gameHistory)
    {
        Console.WriteLine($"{record.Date}: Gamemode: {record.Operation} ({record.Difficulty}) - Score: {record.Score}/5");

        foreach (var problem in record.Problems)
        {
            Console.WriteLine($"   {problem}");
        }

        Console.WriteLine();
    }

    Console.WriteLine("Press any key to return to the main menu...");
    Console.ReadKey();
}

static bool CheckAnswer(int answer, int numOne, int numTwo, string currentOperation)
{
    switch (currentOperation)
    {
        case "+":
            return answer == numOne + numTwo;
        case "-":
            return answer == numOne - numTwo;
        case "*":
            return answer == numOne * numTwo;
        case "/":
            return numTwo != 0 && answer == numOne / numTwo;
        default:
            Console.WriteLine("Invalid operation.");
            return false;
    }
}

class GameRecord
{
    public string Operation { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public int Score { get; set; }
    public DateTime Date { get; set; }
    public List<string> Problems { get; set; } = new List<string>();
}