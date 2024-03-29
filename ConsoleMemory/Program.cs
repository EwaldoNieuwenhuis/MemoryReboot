using BusinessMemory;
using MemoryConsole;
using System.Text;
// Set an initial title
Console.Title = "Memory";
Console.OutputEncoding = Encoding.UTF8;

//Get amount of cards
int number = ConsoleMethods.GetAnwser();
ConsoleMethods.NumberOfCards = number;
Console.Clear();

//create memory grid
MemoryCard<char>[][] grid = MemoryCard<char>.CreateGrid(number / 2);
List<char> banners = ConsoleMethods.GetBanners(number / 2);
MemoryCard<char>.SetBanners(grid, banners);
ConsoleMethods.ShowGrid(grid);

// Create a Timer that updates the seconds played every second
Timer timer = new Timer(ConsoleMethods.UpdateTitle, null, 0, 1000);

//Guess the memory cards
var coordinates = ConsoleMethods.GuessCoordinates(grid);
ConsoleMethods.TakeGuess(coordinates, grid);

//calculate score
double score = GameRules.CalculatePoints(ConsoleMethods.NumberOfCards, ConsoleMethods.ElapsedSeconds, ConsoleMethods.Attempts);
Console.WriteLine($"You won! score: {score}");
timer.Dispose();
Thread.Sleep(1000);


//highscores
MemoryRecord record = ConsoleMethods.UploadScore(score, ConsoleMethods.Attempts, ConsoleMethods.NumberOfCards, ConsoleMethods.ElapsedSeconds);
ConsoleMethods.ShowMemoryRecords(record);
Console.ReadLine();







