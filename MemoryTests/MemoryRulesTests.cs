using BusinessMemory;
using Microsoft.VisualBasic;
using NUnit.Framework.Internal;
using NUnit.Framework;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace MemoryTests
{
    public class MemoryRulesTests
    {

        [SetUp]
        public void Setup()
        {

        }

        //Van iedere unieke kaart zijn er maximaal twee in een spel.
        [Test]
        public void EachUniqueCardAppearsTwice()
        {
            List<char> banners = new List<char> { 'a', 'b', 'c', 'd' };
            MemoryCard<char>[][] grid = MemoryCard<char>.CreateGrid(4);
            MemoryCard<char>.SetBanners(grid, banners);
            Dictionary<char, int> counter = new Dictionary<char, int>();

            // Count occurrences of each card
            foreach (var row in grid)
            {
                foreach (var card in row)
                {
                    if (counter.ContainsKey(card.Banner))
                    {
                        counter[card.Banner]++;
                    }
                    else
                    {
                        counter[card.Banner] = 1;
                    }
                }
            }

            // Check if any card doesn't appear twice
            bool failed = false;
            foreach (var pair in counter)
            {
                if (pair.Value != 2)
                {
                    failed = true;
                }
            }

            // Assert if any card doesn't appear twice
            Assert.That(failed, Is.False);
        }

        //Kaarten worden willekeurig gepositioneerd bij ieder spel dat gespeeld wordt.
        [Test]
        public void CardsAreRandomlyPositionedInEachGame()
        {
            List<char> banners = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i' };
            MemoryCard<char>[][] grid = MemoryCard<char>.CreateGrid(4);
            MemoryCard<char>.SetBanners(grid, banners);

            List<char> bannersShuffled = grid.SelectMany(row => row.Select(card => card.Banner)).ToList();

            string shuffledString = string.Join("", bannersShuffled);
            string originalString = string.Join("", banners);

            Assert.That(shuffledString, Is.Not.EqualTo(originalString));
        }

        //Score Berekening:
        //Controleer of de score correct wordt berekend op basis van het aantal kaarten, de tijd en het aantal pogingen.
        [TestCase(4, 10, 2, 800)]
        [TestCase(10, 20, 5, 1000)]
        [TestCase(4, 20, 2, 400)]
        [TestCase(4, 10, 3, 533)]
        public void ScoreCalculationReturnsCorrectValue(int numberOfCards, int timeInSeconds, int numberOfAttempts, int expectedScore)
        {
            // Voer de scoreberekening uit
            double actualScore = GameRules.CalculatePoints(numberOfCards, timeInSeconds, numberOfAttempts);
            int actualCoreInt = Convert.ToInt32(actualScore);
            // Vergelijk de werkelijke score met de verwachte score
            Assert.That(actualCoreInt, Is.EqualTo(expectedScore));
        }

        [TestCase(9,3,3)]
        [TestCase(10, 3, 4)]
        [TestCase(24, 4, 6)]
        [TestCase(36, 6, 6)]
        [TestCase(101, 10, 11)]
        [TestCase(50, 7, 8)]
        [TestCase(52, 7, 8)]
        [TestCase(3,1,3)]
        [TestCase(2, 1, 2)]
        [TestCase(1, 1, 1)]
        [TestCase(4, 2, 2)]
        [TestCase(35, 5, 7)]
        public void GridShouldBeEvenlyDistributed(int amountofCards, int expectedRows, int expectedColumns)
        {
            var result = MemoryCard<char>.GetRowsColsExtra(amountofCards);

            // Access the returned variables
            int rows = result.Item1;
            int cols = result.Item2;
            int extra = result.Item3;

            int extra2 = extra;

            int colsCoordinates = cols;

            while (extra2 > 0)
            {
                colsCoordinates++;
                extra2 -= rows;
            }

            Assert.Multiple(() =>
            {
                Assert.That(rows, Is.EqualTo(expectedRows));
                Assert.That(colsCoordinates, Is.EqualTo(expectedColumns));
            });
        }

        //Kaarten Genereren:
        //Test of het systeem correct een set kaarten genereert met het juiste aantal unieke paren.
        [TestCase(36, 18)]
        [TestCase(2, 1)]
        [TestCase(4, 2)]
        public void CardsGeneratedUniquePairs(int amountofCards, int expectedPairs)
        {
            MemoryCard<char>[][] grid = MemoryCard<char>.CreateGrid(amountofCards/2);
            List<int> ids = new List<int>();
            foreach (var row in grid)
            {
                foreach (var card in row)
                {
                    ids.Add(card.Id);
                }
            }
            int pairs = ids.Distinct().Count();
            Assert.That(pairs, Is.EqualTo(expectedPairs));

        }

        
       




    }
}