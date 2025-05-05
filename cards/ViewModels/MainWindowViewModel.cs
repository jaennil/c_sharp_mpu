using System.ComponentModel;
using Avalonia;
using Avalonia.Platform;
using System.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using cards.Models;

namespace cards.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CardsDeck _deck;
    private readonly Random _random = new Random();
    private ObservableCollection<Card> _displayedCards = [];
    private const int NumCardsToDisplay = 6;

    public ObservableCollection<Card> DisplayedCards
    {
        get => _displayedCards;
        set
        {
            _displayedCards = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        _deck = new CardsDeck("avares://cards/Assets/cards22.png");
        DisplayedCards = new ObservableCollection<Card>();
        GenerateRandomCards();
    }

    private void GenerateRandomCards()
    {
        DisplayedCards.Clear();
        var allCards = _deck.GetAllCards().OrderBy(x => _random.Next()).Take(NumCardsToDisplay).ToList();

        var centerX = 900;
        var centerY = 400;
        var fanAngle = 150;
        double angleStep = fanAngle / (NumCardsToDisplay - 1);
        double startAngle = -fanAngle/2;
        for (int i = 0; i < NumCardsToDisplay; i++)
        {
            var card = allCards[i];
            
            var angle = startAngle + angleStep * i;
            
            card.X = centerX;
            card.Y = centerY;
            card.Angle = angle;
            card.ZIndex = i;
            
            DisplayedCards.Add(card);
        }
    }
}
