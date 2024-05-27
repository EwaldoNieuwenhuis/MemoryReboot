// See https://aka.ms/new-console-template for more information
using MemoryDataAccess;
using BusinessMemory;
MemoryRecordRepository repository = new MemoryRecordRepository();

//MemoryRecord record = new MemoryRecord(12,12,12, "Klaas", DateTime.Now);
//repository.CreateReadOrUpdate(record);
//MemoryRecord x = new MemoryRecord(20, 12, 12, "Klaas", DateTime.Now);
//MemoryRecord updatedRecord = repository.CreateReadOrUpdate(x);
//Console.WriteLine(updatedRecord);
//MemoryRecord l = new MemoryRecord(12, 387, 78, "Klaas", DateTime.Now);
//repository.CreateReadOrUpdate(l);
//var dic = new Dictionary<string, object>();
//dic.Add("PlayerName", l.PlayerName);
//var rowRecords = repository.ReadAllWhere(dic);
//foreach (var rowRecord in rowRecords)
//{
//    Console.WriteLine(rowRecord);
//}
//Console.WriteLine(repository.ReadAllRaw());
Dictionary<string, List<object>> table = repository.ReadAllWithColumns("SELECT PlayerName as Name, ((SQRT(AmountOfCards)) / (Seconds * Attempts)) * 1000 AS Score, AmountOfCards as Cards, Seconds, Attempts, DateTime FROM MemoryRecords ORDER by Score DESC");
Dictionary<string, List<int>> data = new Dictionary<string, List<int>>();
data["banana"] = new List<int> {3,1,1};
data["apple"] = new List<int> { 1, 4, 2 };
data["orange"] = new List<int> { 5, 2, 1 };

int Sum(int a, int b, int c)
{
    return a + b + c;
}
// Sorteer de dictionary op basis van de values
//var sortedDictionary = data.OrderByDescending(pair => Sum(pair.Value[0], pair.Value[1], pair.Value[2]));
// Print de gesorteerde dictionary
//foreach (var pair in sortedDictionary)
//{
//    Console.WriteLine($"{pair.Key}: {Sum(pair.Value[0], pair.Value[1], pair.Value[2])}");
//}

var sortedTable = table;


int maxLength = 0;
foreach (var pair in sortedTable)
{
    if (pair.Value.Count > maxLength)
    {
        maxLength = pair.Value.Count;
    }
}
// Print de tabel
//Console.WriteLine("┌──────────┬──────────┐");
foreach (var pair in sortedTable)
{
    Console.Write($"│ {pair.Key,-9}");
}
Console.Write($"│ Score |");
Console.WriteLine();
//Console.WriteLine("├──────────┼──────────┤");
for (int i = 0; i < maxLength; i++)
{
    foreach (var pair in table)
    {
        if (i < pair.Value.Count)
        {
            Console.Write($"│ {pair.Value[i],-9}");
        }
        else
        {
            Console.Write("│          │");
        }
       
    }
    Console.WriteLine();
}


//Console.WriteLine("└──────────┴──────────┘");

