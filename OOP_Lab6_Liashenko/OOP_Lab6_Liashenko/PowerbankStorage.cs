using System;
using System.Collections.Generic;

public class PowerbankStorage
{
    private List<Powerbank> items = new List<Powerbank>();
    private int maxCount;

    public PowerbankStorage(int maxCount)
    {
        if (maxCount < 1)
            throw new ArgumentOutOfRangeException(null, "Error: N має бути більше 0!");
        this.maxCount = maxCount;
    }

    public int Count => items.Count;

    public int MaxCount => maxCount;

    public bool IsFull => items.Count >= maxCount;

    public bool Add(Powerbank powerbank)
    {
        if (powerbank == null || IsFull) return false;
        items.Add(powerbank);
        return true;
    }

    public Powerbank GetByNumber(int number)
    {
        if (number < 1 || number > items.Count)
            throw new ArgumentOutOfRangeException(null, "Error: об'єкта з таким номером немає!");
        return items[number - 1];
    }

    public int GetNumber(Powerbank powerbank)
    {
        return items.IndexOf(powerbank) + 1;
    }

    public bool Contains(Powerbank powerbank)
    {
        return items.Contains(powerbank);
    }

    public List<Powerbank> GetAll()
    {
        return new List<Powerbank>(items);
    }

    public List<Powerbank> FindByModel(string text)
    {
        if (string.IsNullOrEmpty(text)) return new List<Powerbank>();
        return items.FindAll(x => x.Model.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public List<Powerbank> FindByState(PowerbankState state)
    {
        return items.FindAll(x => x.State == state);
    }

    public bool RemoveByNumber(int number)
    {
        if (number < 1 || number > items.Count) return false;
        items.RemoveAt(number - 1);
        Powerbank.DecreaseCounter();
        return true;
    }

    public int RemoveByModel(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int removed = items.RemoveAll(x => x.Model.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
        DecreaseCounter(removed);
        return removed;
    }

    public int RemoveByState(PowerbankState state)
    {
        int removed = items.RemoveAll(x => x.State == state);
        DecreaseCounter(removed);
        return removed;
    }

    public void Clear()
    {
        DecreaseCounter(items.Count);
        items.Clear();
    }

    private void DecreaseCounter(int times)
    {
        for (int i = 0; i < times; i++)
            Powerbank.DecreaseCounter();
    }
}
