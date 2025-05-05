using System.ComponentModel;
using Avalonia;
using SkiaSharp;

namespace cards.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private SKBitmap _randomCard;
    
    public SKBitmap RandomCard
    {
        get => _randomCard;
        set
        {
            _randomCard = value;
        }
    }
    
    public MainWindowViewModel()
    {
        int cardWidth = 50;
        int cardHeight = 100;
        RandomCard = new SKBitmap(cardWidth, cardHeight);
        using var spriteSheet = SKBitmap.Decode("cards2.png");
        var sourceRect = new SKRectI(
            0,
            0,
            cardWidth,
            cardHeight
        );
        using var canvas = new SKCanvas(RandomCard);
        canvas.DrawBitmap(spriteSheet, sourceRect, new SKRect(0, 0, cardWidth, cardHeight));
    }

    
    private void GetRandomCard()
    {
        using var spriteSheet = SKBitmap.Decode("/Assets/cards2.png");
        int cardsPerRow = 13;
        int cardsPerColumn = 4;

        for (int row = 0; row < cardsPerColumn; row++)
        {
            for (int column = 0; column < cardsPerRow; column++)
            {
                using var cardBitmap = new SKBitmap(50, 150);

                var sourceRect = new SKRectI(
                    
                );
            }
        }
    }
}
