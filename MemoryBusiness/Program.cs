// See https://aka.ms/new-console-template for more information
using BusinessMemory;

static (int rows, int cols, int extra) CalculateGridSize(int numberOfCards)
{
    // Bepaal een schatting voor het aantal rijen en kolommen
    int sqrt = (int)Math.Sqrt(numberOfCards);

    // Kijk of het aantal kaarten perfect past in het grid
    if (sqrt * sqrt == numberOfCards)
    {
        return (sqrt, sqrt, 0);
    }
    else
    {
        // Zoek het aantal rijen en kolommen waarbij je het aantal kaarten niet overschrijdt
        int rows = sqrt;
        int cols = sqrt;

        // Bereken het aantal extra kaarten
        int extra = (rows * cols) - numberOfCards;

        return (rows, cols, extra);
    }
}


// Voorbeeld: bereken de grootte van het grid voor 10 kaarten
int numberOfCards = 4;
var gridSize = CalculateGridSize(numberOfCards);

Console.WriteLine($"Voor {numberOfCards} kaarten heb je een grid nodig met {gridSize.rows} rij(en), {gridSize.cols} kolom(men), en {gridSize.extra} extra kaart(en).");

