using BusinessMemory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MemoryDataAccess;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design;

namespace MemoryConsole
{
    public class ConsoleMethods
    {
        public static List<(int, int)> GuessedCards = new List<(int, int)>();
        private static (int,int) LastCard = (-1,-1);
        public static int NumberOfCards { get; set; }
        private static int Points { get; set; }
        public static int ElapsedSeconds { get; private set; } = 0;
        public static int Attempts { get; set; } = 0;
        public static MemoryRecordRepository memoryRecordRepository = new MemoryRecordRepository();
        public static char StandardBanner { get; private set; } = '▮';

        public static (int, int) GuessCoordinates(MemoryCard<char>[][] grid)
        {

            Console.WriteLine("Enter coordinates to guess (in this format: 1,1): ");
            string? anwser = Console.ReadLine();

            if (anwser != null)
            {
                if (anwser.Contains(','))
                {
                    // Split the string based on commas
                    string[] numbers = anwser.Split(',');
                    int row = 0;
                    int col = 0;
                    try
                    {
                        row = Int32.Parse(numbers[0]);
                        col = int.Parse(numbers[1]);
                        if (grid[row-1][col-1] != null)
                        {
                            return (row, col);
                        }
                    }
                    catch (Exception e) {
                        return (0, 0);
                    }

                }

            }

            return (0, 0);
        }

      

        public static void TakeGuess((int, int) coordinates, MemoryCard<char>[][] grid)
        {
            int row = coordinates.Item1;
            int col = coordinates.Item2;
            if (row > 0)
            {
                //coordinates start with 1 less because indexes of arrays start with 0
                row--;
                col--;

                if (grid[row][col] != null)
                {
                    
                    if (!GuessedCards.Contains((row, col)))
                    {

                        if (LastCard.Item1 != -1) //tweede kaart
                        {
                            // MemoryCard<char> memoryCard = grid[row][col];
                            GuessedCards.Add((row, col));
                            //show
                            Console.Clear();
                            ShowGrid(grid);
                            // wait couple of milliseconds
                            Thread.Sleep(800);
                            // check if the two cards were identical
                            if (grid[row][col].Id == grid[LastCard.Item1][LastCard.Item2].Id) //correct
                            {
                                Points++;
                                LastCard.Item1 = -1;
                                if (GuessedCards.Count == NumberOfCards) //all cards have been guessed
                                {
                                    return;
                                }
                                else
                                {
                                    var coords = GuessCoordinates(grid);
                                    TakeGuess(coords, grid);
                                }
    
                            }
                            else //incorrect
                            {
                                //remove cards from cardsguessed
                                GuessedCards.Remove((row, col));
                                GuessedCards.Remove((LastCard.Item1, LastCard.Item2));
                                LastCard.Item1 = -1;
                                //reset
                                Console.Clear();
                                ShowGrid(grid);
                                //ask coords
                                var coords = GuessCoordinates(grid);
                                TakeGuess(coords, grid);
                            }

                            
                        }
                        else //eerste kaart
                        {
                            // MemoryCard<char> memoryCard = grid[row][col];
                            GuessedCards.Add((row, col));
                            LastCard = (row, col);
                            Attempts++;
                            Console.Clear();
                            ShowGrid(grid);
                            var coords = GuessCoordinates(grid);
                            TakeGuess(coords, grid);
                        }
                       
                    }
                }
            }
            else
            {
                Console.Clear();
                ShowGrid(grid);
                var coords = GuessCoordinates(grid);
                TakeGuess(coords, grid);
            }

        }

        public static int GetAnwser()
        {
            Console.WriteLine($"Welcome to memory, with how many cards do you want to play? (even number, between {GameRules.MinCards}-{GameRules.MaxCards} cards)");
            string? anwser = Console.ReadLine();
            try
            {
                GameRules.CheckAnwserCardNumber(anwser);
                return int.Parse(anwser);
            }catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(1000);
                Console.Clear();
                return GetAnwser();
            }

        }

        public static List<char> GetBanners(int amount)
        {
            int minAscii = 65;
            int maxAscii = 122;
            if (amount > GameRules.MaxCards)
            {
                throw new TooManyCardsException(amount);
            }

            List<char> banners = new List<char>();
            Random random = new Random();

            while (banners.Count < amount)
            {
                int randomAscii = random.Next(minAscii, maxAscii + 1);
                char randomChar = (char)randomAscii;

                // Check for uniqueness before adding to the list
                if (!banners.Contains(randomChar))
                {
                    banners.Add(randomChar);
                }
            }

            return banners;
        }

        public static void ShowGrid(MemoryCard<char>[][] grid)
        {
            MemoryCard<char>.StandardBanner = StandardBanner;
            int columns = grid[0].Length;
            int rows = grid.Length;
            // Draw coordinates
            Console.Write("+ "); //first coordinate middle thing
            //    first line has all the number of cols
            for (int i = 0; i < columns; i++)
            {
                Console.Write($"{i + 1} ");
            }
            Console.WriteLine();

            for (int j = 0; j < grid.Length; j++)
            {
                Console.Write($"{j + 1} ");
                for (int k = 0; k < grid[j].Length; k++)
                {
                    if (grid[j][k] != null)
                    {
                        if (GuessedCards.Contains((j, k)))
                        {
                            Console.Write(grid[j][k].Banner+ " ");
                        }
                        else
                        {
                            Console.Write(MemoryCard<char>.StandardBanner+ " ");
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        public static void UpdateTitle(object state)
        {
            ElapsedSeconds++;
            // Update the title based on some logic (e.g., current time)
            Console.Title = $"Memory, Time: {ElapsedSeconds} seconds, Cards: {NumberOfCards}, Attempts: {Attempts}";
        }

        public static MemoryRecord UploadScore(double score, int attempts, int cards, int time)
        {
            //memoryRecordRepository.DeleteAll(out int rowsaffected);
            Console.Clear();
            Console.WriteLine("What is your name?");
            string? name = Console.ReadLine();
            if (name == null)
            {
                UploadScore(score, attempts, cards, time);
            }

            MemoryRecord record = new MemoryRecord(cards, time, attempts, name);
            memoryRecordRepository.CreateReadOrUpdate(record);
            return record;

        }

        public static void ShowMemoryRecords(MemoryRecord playerRecord)
        {
            Console.Clear();
            Dictionary<string, List<object>> rawTableOld = memoryRecordRepository.ReadAllWithColumns("SELECT PlayerName as Name, ROUND(((SQRT(AmountOfCards)) / (Seconds * Attempts)) * 1000,2) AS Score, AmountOfCards as Cards, Seconds, Attempts, DateTime AS Date FROM MemoryRecords ORDER by Score DESC");
            rawTableOld = addPlace(rawTableOld);
            rawTableOld = SortTablePosition(new List<string> { "Place" }, rawTableOld);
            //rawTable = add
            Dictionary<string, List<object>> rawTable = InsertPadding(rawTableOld, out Dictionary<string, int> mostValuesPerKey);
            //string header = "| Place | Name | Score | Date | Cards | Time | Attempts |";
            int total = CalculateTotalLengthTable(rawTableOld, mostValuesPerKey);
            int place = 0;
            for (int i = 0; i < total; i++)
            {
                Console.Write("_");
            }
            Console.WriteLine();
            Console.WriteLine(GetHeadersString(rawTable));
            for (int i = 0; i < total; i++)
            {
                Console.Write("_");
            }
            Console.WriteLine();
            for (int i = 0; i < rawTableOld["Score"].Count; i++)
            {
                if (rawTableOld["Name"][i].ToString() == playerRecord.PlayerName)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ResetColor();
                }
               
                string record = GetRecordString(i, rawTable);
                Console.WriteLine(record);
            }
            Console.ResetColor();
            for (int i = 0; i < total; i++)
            {
                Console.Write("-");
            }
        }

        private static Dictionary<string, List<object>> SortTablePosition(List<string> list, Dictionary<string, List<object>> rawTableOld)
        {
            Dictionary <string, List<object>> newTable = new Dictionary<string, List<object>>();
            foreach (string key in list)
            {
                newTable.Add(key, rawTableOld[key]);
            }

            foreach(string key in rawTableOld.Keys)
            {
                if (!newTable.ContainsKey(key))
                {
                   newTable.Add(key, rawTableOld[key]);
                }
            }
            return newTable;
        }

        private static Dictionary<string, List<object>> addPlace(Dictionary<string, List<object>> rawTable)
        {
            List<object> placings = new List<object>();
            for (int i = 1; i <= rawTable["Score"].Count; i++)
            {
                placings.Add('#' + i.ToString());
            }
            rawTable["Place"] = placings;
            return rawTable;
        }

        private static Dictionary<string, List<object>> InsertPadding(Dictionary<string, List<object>> rawTable, out Dictionary<string, int> mostValuesPerKey)
        {
            //make a dcitionary to track how much the most characters there are
            mostValuesPerKey = new Dictionary<string, int>();
            foreach(string key  in rawTable.Keys)
            {
                mostValuesPerKey[key] = 0;
            }

            //loop over all keys and values
            foreach (string key in rawTable.Keys)
            {
                //check per key and the belonging values which string has the most amount chars
                if (key.Length> mostValuesPerKey[key])
                {
                    mostValuesPerKey[key] = key.Length;
                }
                List<object> list = rawTable[key];
                foreach(object value in list)
                {
                    string stringValue = value.ToString();
                    if(stringValue.Length > mostValuesPerKey[key])
                    {
                        //add the one with the most into dictionary
                        mostValuesPerKey[key] = stringValue.Length;
                    }
                }
                
            }
            
            Dictionary<string, List<object>> paddingTable = new Dictionary<string, List<object>>();
            //calculate per key the value how much more they need and and the " "'s at the beginning and the end
            foreach(string key in rawTable.Keys)
            {
                int amountOfSpaces = mostValuesPerKey[key]-key.Length;
                string newName = "";
                if(amountOfSpaces > 0)
                {
                    int eachSide = amountOfSpaces / 2;
                    for (int i = 0; i < eachSide; i++)
                    {
                        newName += " ";
                    }
                    newName += key;
                    while (newName.Length < mostValuesPerKey[key]) {
                        newName += " ";
                    }

                }
                else
                {
                    newName = key;
                }

                List<object> newList = new List<object>();
                List<object> list = rawTable[key];
                foreach (var item in list)
                {
                    string value = item.ToString();
                    int amountOfSpacesItems = mostValuesPerKey[key] - value.Length;
                    string newNameItems = "";
                    if (amountOfSpacesItems > 0)
                    {
                        int eachSide = amountOfSpacesItems / 2;
                        for (int i = 0; i < eachSide; i++)
                        {
                            newNameItems += " ";
                        }
                        newNameItems += value;
                        while (newNameItems.Length < mostValuesPerKey[key])
                        {
                            newNameItems += " ";
                        }
                    }
                    else
                    {
                        newNameItems = value;
                    }
                    newList.Add(newNameItems);
                }

                paddingTable[newName] = newList;


            }

            return paddingTable;
        }


        private static string GetRecordString(int row, Dictionary<string, List<object>> table)
        {
            string recordstring = "";

            foreach (var key in table.Keys)
            {
                object value = table[key][row];
                string s = value.ToString();
                recordstring += "| ";
                recordstring += s;
                recordstring += " ";
            }

            recordstring += "|";
            return recordstring;

        }

        private static string GetHeadersString(Dictionary<string, List<object>> table)
        {
            //string header = "| Place | Name | Score | Date | Cards | Time | Attempts |";
            string headersString = "";
            foreach(string key in table.Keys)
            {
                headersString += "| ";
                headersString += key;
                headersString += " ";
            }
            return headersString+="|";
        }
        

        private static int CalculateTotalLengthTable(Dictionary<string, List<object>> rawTable, Dictionary<string, int> mostPerKey)
        {
            int total = 0;
            foreach(string key in rawTable.Keys)
            {
                total += mostPerKey[key];
                total += 3; //3 because of the "| " and the " |"
            }

            return total+1; //1 because of the last "|"
        }

        // Definieer een generieke methode om het maximale aantal tekens van een veld te vinden
        public static int GetMaxStringLength<T>(IEnumerable<T> items, Func<T, string> selector)
        {
            // Selecteer de strings van de velden met behulp van de selector functie
            var strings = items.Select(selector);

            // Bepaal de maximale lengte van de strings en geef deze terug
            return strings.Max(s => s.Length);
        }
    }




}
