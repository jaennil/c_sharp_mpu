using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Transactions;
using Avalonia.Media;

namespace cards.Models;

public class Card : INotifyPropertyChanged
{
    public static int Width => 360;
    public static int Height => 540;
    public Suit Suit;
    public Rank Rank;
    public IImage Image { get; }
    private double _x;
    private double _y;
    
    public double X
    {
        get => _x;
        set
        {
            if (_x != value)
            {
                _x = value;
                OnPropertyChanged();
            }
        }
    }
    
    public double Y
    {
        get => _y;
        set
        {
            if (_y != value)
            {
                _y = value;
                OnPropertyChanged();
            }
        }
    }

    private double _zIndex;
    public double ZIndex
    {
        get => _zIndex;
        set
        {
            if (_zIndex != value)
            {
                _zIndex = value;
                OnPropertyChanged();
            }
        }
    }
    public double Angle { get; set; }
    private Random _random = new Random();
    
    public Card(Suit suit, Rank rank, IImage image)
    {
        Suit = suit;
        Rank = rank;
        Image = image;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}