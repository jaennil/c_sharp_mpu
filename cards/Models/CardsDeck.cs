using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace cards.Models;

public class CardsDeck
{
    private const int Margin = 30;
    private readonly Dictionary<(Suit, Rank), IImage> _cards = new Dictionary<(Suit, Rank), IImage>();

    public CardsDeck(string spritesheetPath)
    {
        LoadCards(spritesheetPath);
    }

    private void LoadCards(string spritesheetPath)
    {
        using var stream = AssetLoader.Open(new Uri(spritesheetPath));
        var bitmap = new Bitmap(stream);

        for (var suitIndex = 0; suitIndex < 4; suitIndex++)
        {
            var suit = (Suit)suitIndex;

            for (var rankIndex = 0; rankIndex < 13; rankIndex++)
            {
                var rank = (Rank)rankIndex;

                var x = rankIndex * Card.Width + Margin * (rankIndex+1);
                var y = suitIndex * Card.Height + Margin * (suitIndex+1);

                var cardImage = new CroppedBitmap(bitmap, new PixelRect(x, y, Card.Width, Card.Height));

                _cards[(suit, rank)] = cardImage;
            }
        }
    }

    public List<Card> GetAllCards()
    {
        return _cards.Select(kvp => new Card(kvp.Key.Item1, kvp.Key.Item2, kvp.Value)).ToList();
    }
}
