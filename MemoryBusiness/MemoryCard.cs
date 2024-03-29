using BusinessMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessMemory
{
    public class MemoryCard<T>
    {
        public int Id { get; private set; }
        public T Banner { get; private set; }
        private static T? _standardBanner = default(T);
        public static Random random = new Random();
        public static T? StandardBanner
        {
            set { if (EqualityComparer<T>.Default.Equals(_standardBanner, default(T))) _standardBanner = value; }
            get { return _standardBanner; }
        }
        private static int LastId = 0;
        private Random Random = new Random();

        public MemoryCard()
        {
            Id = LastId + 1;
            LastId = Id;
        }

        public MemoryCard(T banner)
        {
            Id = LastId + 1;
            LastId = Id;
            Banner = banner;
        }

        /// <summary>
        /// Creates a memory grid with the amount of unique cards
        /// </summary>
        public static MemoryCard<T>[][] CreateGrid(int numberOfUniqueCards)
        {
            //unique memorycards
            MemoryCard<T>[] memoryCards = new MemoryCard<T>[numberOfUniqueCards];
            SetIds(memoryCards);
            int cardstouse = numberOfUniqueCards * 2;
            var result = GetRowsColsExtra(cardstouse);

            // Access the returned variables
            int rows = result.Item1;
            int cols = result.Item2;
            int extra = result.Item3;


            int extra2 = extra;
           
            int colsCoordinates = cols;

            while (extra2 > 0)
            {
                colsCoordinates++;
                extra2 = extra2 - rows;
            }

            MemoryCard<T>[][] cards = GetMemoryCards(rows, cols, extra, memoryCards, colsCoordinates);
            return cards;

        }

        private static void SetIds(MemoryCard<T>[] memoryCards)
        {
            for (int i = 0; i < memoryCards.Length; i++)
            {
                memoryCards[i] = new MemoryCard<T>();
            }
        }

        public static (int, int, int) GetRowsColsExtra(int numberOfCards)
        {
            // Bepaal het aantal rijen en kolommen
            double sqrtFl = Math.Sqrt(numberOfCards);
            int sqrt = (int)sqrtFl;
            // Bereken het aantal extra kaarten
            double extraD = numberOfCards - Math.Ceiling(sqrtFl)*sqrt;
            int extra = (int)extraD;
            return (sqrt, (int)Math.Ceiling(sqrtFl), extra);
            

        }

        private static MemoryCard<T>[][] GetMemoryCards(int rows, int cols, int extra, MemoryCard<T>[] memoryCards, int colsCoordinates)
        {
            MemoryCard<T>[][] cards = new MemoryCard<T>[rows][];
            //initalize cards
            for (int i = 0; i < rows; i++)
            {
                cards[i] = new MemoryCard<T>[colsCoordinates];
            }

            Dictionary<int, int> pickedCardsAmount = new Dictionary<int, int>();

            // Create and display the grid
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    MemoryCard<T> card = GetRandomMemoryCard(memoryCards, pickedCardsAmount);
                    cards[i][j] = card;

                }

                //how many columns i need to add extra?
                int amount = cards[i].Length;
                int colstoadd = amount - cols;
                // Add extra columns
                for (int k = 1; k<=colstoadd; k++)
                {
                    if (extra > 0)
                    {
                        cards[i][(cols-1)+k] = GetRandomMemoryCard(memoryCards, pickedCardsAmount);
                        extra--;
                    }
                    
                }
            }
            return cards;
        }

        private static MemoryCard<T> GetRandomMemoryCard(MemoryCard<T>[] initalCards, Dictionary<int, int> pickedCardsAmount)
        {
            //get random card from memorycards
            int rand = random.Next(initalCards.Length);
            if (!pickedCardsAmount.ContainsKey(initalCards[rand].Id))
            {
                pickedCardsAmount[initalCards[rand].Id] = 1;
                return initalCards[rand];
            }
            else
            {
                if (pickedCardsAmount[initalCards[rand].Id] < 2)
                {
                    pickedCardsAmount[initalCards[rand].Id]++;
                    return initalCards[rand];
                }
                else { return GetRandomMemoryCard(initalCards, pickedCardsAmount); }
            }
        }

            

        public static void SetBanners(MemoryCard<T>[][] grid, List<T> banners)
        {
            int amountOfOriginalImages = GetAmountOfOriginalCards(grid);
            List<T> uniqueBanners = banners.Distinct().ToList();
            if (uniqueBanners.Count < amountOfOriginalImages)
            {
                throw new Exception("Not enough unique banners");
            }
            uniqueBanners = Shuffle(uniqueBanners);
            Dictionary<int, T> genericDictionary = new Dictionary<int, T>();
            int count = 0;
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] != null)
                    {
                        int id = grid[i][j].Id;
                    
                        if (!genericDictionary.ContainsKey(id))
                        {
                            genericDictionary.Add(id, uniqueBanners[count]);
                            count++;
                        }
                    }
                }
            }

            SetBannerWithDictionary(grid, genericDictionary);
            CheckGrid(grid);
        }

        private static void CheckGrid(MemoryCard<T>[][] grid)
        {
            // Count occurrences of each Id
            Dictionary<T, int> idCount = new Dictionary<T, int>();

            // Loop over the grid
            foreach (var row in grid)
            {
                foreach (var card in row)
                {
                    if(card != null)
                    {
                        T banner = card.Banner;

                        // Increment count for the current Id
                        if (idCount.ContainsKey(banner))
                        {
                            idCount[banner]++;
                        }
                        else
                        {
                            idCount[banner] = 1;
                        }
                    }
                    
                }
            }
            // Check if each Id occurs exactly 2 times
            bool allIdsOccurTwice = idCount.All(pair => pair.Value == 2);
            if (!allIdsOccurTwice)
            {
                throw new Exception("Some banners occur more than twice!");
            }
        }

        private static void SetBannerWithDictionary(MemoryCard<T>[][] grid, Dictionary<int, T> genericDictionary)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] != null)
                    {
                        int id = grid[i][j].Id;
                        T banner = genericDictionary[id];
                        grid[i][j].Banner = genericDictionary[id];
                    }
                }
            }
        }

        private static List<T> Shuffle(List<T> list)
        {
            Random random = new Random();

            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                // Swap list[i] and list[j]
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }

        public static int GetAmountOfOriginalCards(MemoryCard<T>[][] grid)
        {
            int x = 0;
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if ((grid[i][j] != null))
                    {
                        x++;
                    }
                }
            }
            return x/2;
        }


    }
}
