/** Requirements
1. You need to create a Math game containing the 4 basic operations
2. The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
3. Users should be presented with a menu to choose an operation
4. You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
5. You don't need to record results on a database. Once the program is closed the results will be deleted
*/
/** Challenges
1. Try to implement levels of difficulty.
2. Add a timer to track how long the user takes to finish the game.
3. Create a 'Random Game' option where the players will be presented with questions from random operations
*/

List<HighscoreEntry> highscores = new List<HighscoreEntry>();

int menuSelection;

Console.WriteLine("Welcome to the Math Game app.");
do
{
    Console.WriteLine("\nSelect an option:");
    Console.WriteLine("1. Start a new game");
    Console.WriteLine("2. See Highscores (session)");
    Console.WriteLine("0. Exit");

    menuSelection = ReadUserInput();

    Console.Clear();
    switch (menuSelection)
    {
        case 1:
            ShowNewGameMenu();
            break;
        case 2:
            ShowHighscores();
            break;
        case 0:
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
} while (menuSelection != 0);

void ShowHighscores()
{
    if (highscores.Count == 0)
        System.Console.WriteLine("Nothing here yet...");
    else
    {
        // Order highscores
        List<HighscoreEntry> ordered = highscores.OrderByDescending(t => t.Score).ThenBy(t => t.ElapsedTime).ToList();
        for (int i = 0; i < highscores.Count; i++)
        {
            Console.WriteLine($"{(i + 1).ToString().PadLeft(2)}. {ordered[i]}");
        }
    }

    Console.WriteLine("Press enter to continue.");
    Console.ReadLine();
}

void ShowNewGameMenu()
{
    int menuSelection;
    do
    {
        // Game options:
        Console.WriteLine("Select a game:");
        Console.WriteLine("1. Addition");
        Console.WriteLine("2. Subtraction");
        Console.WriteLine("3. Multiplication");
        Console.WriteLine("4. Division");
        Console.WriteLine("5. Random");
        Console.WriteLine("0. Main Menu");

        menuSelection = ReadUserInput();

        Console.Clear();
        switch (menuSelection)
        {
            case 1:
                StartGame(MathOperation.Addition);
                break;
            case 2:
                StartGame(MathOperation.Subtraction);
                break;
            case 3:
                StartGame(MathOperation.Multiplication);
                break;
            case 4:
                StartGame(MathOperation.Division);
                break;
            case 5:
                StartGame(MathOperation.Random);
                break;
            case 0:
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    } while (menuSelection != 0);
}

GameDifficulty GetGameDifficulty()
{
    // Game difficulties:
    Console.WriteLine("Select a difficulty:");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");

    int difficultySelection = ReadUserInput();
    GameDifficulty selectedDifficulty = GameDifficulty.Easy;

    bool validOption = false;
    do
    {
        switch (difficultySelection)
        {

            case 1:
                selectedDifficulty = GameDifficulty.Easy;
                validOption = true;
                break;
            case 2:
                selectedDifficulty = GameDifficulty.Medium;
                validOption = true;
                break;
            case 3:
                selectedDifficulty = GameDifficulty.Hard;
                validOption = true;
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    } while (!validOption);

    return selectedDifficulty;
}

/** This function cannot return MathOperation.Random */
MathOperation GetMathOperation()
{
    int option = new Random().Next(1, 5);
    switch (option)
    {
        case 1: return MathOperation.Addition;
        case 2: return MathOperation.Subtraction;
        case 3: return MathOperation.Multiplication;
        case 4: return MathOperation.Division;
    }
    throw new ArgumentOutOfRangeException("Invalid option");
}

void StartGame(MathOperation gameOption)
{
    GameDifficulty gameDifficulty = GetGameDifficulty();
    Console.Clear();

    int score = 0;
    bool continueGame = true;

    int userResponse, correctResponse;
    MathOperation mathOperation = gameOption;

    var start = DateTime.Now;
    while (continueGame)
    {
        if (gameOption == MathOperation.Random)
            mathOperation = GetMathOperation();

        (int, int) terms = GetOperationTerms(mathOperation, gameDifficulty);

        Console.WriteLine($"How much is {terms.Item1}{GetGameOperator(mathOperation)}{terms.Item2}?");
        userResponse = ReadUserInput();

        correctResponse = ApplyMathOperation(mathOperation, terms.Item1, terms.Item2);
        if (correctResponse == userResponse)
            score += CalcScore(mathOperation, gameDifficulty);
        else
        {
            Console.WriteLine($"The right answer was {correctResponse}");
            continueGame = false;
        }
    }
    var end = DateTime.Now;
    TimeSpan delta = end - start;

    Console.WriteLine($"Game finished after {delta.TotalSeconds} seconds! Score: {score}");

    // TODO Update Highscores
    Console.WriteLine("Insert player name");
    string? userNameInput = Console.ReadLine();
    string userName = userNameInput == null ? "Anonymous" : userNameInput;
    highscores.Add(new HighscoreEntry(score, gameDifficulty, userName, delta));
}

(int, int) GetOperationTerms(MathOperation mathOperation, GameDifficulty gameDifficulty)
{
    Random rnd = new Random();
    int term1, term2;
    int difficulty = (int)gameDifficulty;

    // Term 1
    term1 = rnd.Next(difficulty);

    // Term 2
    if (mathOperation == MathOperation.Division)
        // If operation is Division, then avoid 0 as the denominator.
        // Also, avoid terms that don't produce integer results
        do
        {
            term2 = rnd.Next(1, difficulty);
        } while (term1 % term2 != 0);
    else
        term2 = rnd.Next(difficulty / 5);

    // If Subtracting, then make the result a positive value
    if (mathOperation == MathOperation.Subtraction && term1 < term2)
        return (term2, term1);
    else
        return (term1, term2);
}

int ReadUserInput()
{
    string? readResult;
    bool parseSuccess = false;
    int userInput = -1;

    do
    {
        readResult = Console.ReadLine();
        if (readResult != null)
        {
            parseSuccess = int.TryParse(readResult, out userInput);
        }
    } while (!parseSuccess);

    return userInput;
}

int CalcScore(MathOperation mathOperation, GameDifficulty gameDifficulty)
{
    int operationScore = (int)mathOperation + 1;
    switch (gameDifficulty)
    {
        case GameDifficulty.Easy: return operationScore * 2;
        case GameDifficulty.Medium: return operationScore * 3;
        case GameDifficulty.Hard: return operationScore * 4;
    }
    throw new ArgumentOutOfRangeException("gameDifficulty is not valid");
}

int ApplyMathOperation(MathOperation mathOperation, int leftNumber, int rightNumber)
{
    switch (mathOperation)
    {
        case MathOperation.Addition:
            return leftNumber + rightNumber;
        case MathOperation.Subtraction:
            return leftNumber - rightNumber;
        case MathOperation.Multiplication:
            return leftNumber * rightNumber;
        case MathOperation.Division:
            return leftNumber / rightNumber;
        default:
            throw new ArgumentOutOfRangeException("GameOption is not valid: " + mathOperation);
    }
}

string GetGameOperator(MathOperation mathOperation)
{
    switch (mathOperation)
    {
        case MathOperation.Addition:
            return "+";
        case MathOperation.Subtraction:
            return "-";
        case MathOperation.Multiplication:
            return "*";
        case MathOperation.Division:
            return "/";
        default:
            throw new ArgumentOutOfRangeException("GameOption is not valid: " + mathOperation);
    }
}

enum MathOperation
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Random,
}

public enum GameDifficulty
{
    Easy = 50,
    Medium = 100,
    Hard = 500,
}

public struct HighscoreEntry
{
    public HighscoreEntry(int score, GameDifficulty selectedDifficulty, string playerName, TimeSpan elapsedTime)
    {
        Score = score;
        SelectedDifficulty = selectedDifficulty;
        PlayerName = playerName;
        ElapsedTime = elapsedTime;
    }

    public double Score { get; }
    public GameDifficulty SelectedDifficulty { get; }
    public string PlayerName { get; }
    public TimeSpan ElapsedTime { get; }

    public override string ToString() => $"{PlayerName.PadRight(20)} -- {Score.ToString().PadLeft(5)} ({SelectedDifficulty}) -- {ElapsedTime:mm':'ss}";
}
