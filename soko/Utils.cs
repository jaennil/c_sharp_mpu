namespace Game;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public struct Position
{
    public int X;
    public int Y;

    public Position AddDisplacement(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return new Position { X = X, Y = Y - 1 };
            case Direction.Down:
                return new Position { X = X, Y = Y + 1 };
            case Direction.Left:
                return new Position { X = X - 1, Y = Y };
            case Direction.Right:
                return new Position { X = X + 1, Y = Y };
            default:
                throw new ArgumentException("Invalid direction");
        }
    }

}

public class DropOutStack<T>
{
    private T[] items;
    private int topIndex = 0;
    public int Count = 0;

    public DropOutStack(int limit)
    {
        items = new T[limit];
    }

    public void Push(T item)
    {
        items[topIndex] = item;

        if (Count < items.Length) Count++;

        topIndex = (topIndex + 1) % items.Length;
    }

    public T Pop()
    {
        if (Count <= 0) throw new InvalidOperationException("Can't pop from an empty stack.");

        Count--;

        topIndex = (items.Length + topIndex - 1) % items.Length;
        return items[topIndex];
    }

    public bool CanPop() => Count > 0;

    public void Clear()
    {
        Array.Clear(items);

        Count = 0;
        topIndex = 0;
    }
}
