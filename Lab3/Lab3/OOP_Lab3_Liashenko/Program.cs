using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

class Program
{
    static List<Powerbank> items = new List<Powerbank>();
    static Random rnd = new Random();
    static DateTime minDate = new DateTime(2015, 1, 1);
    static PowerbankState[] states = (PowerbankState[])Enum.GetValues(typeof(PowerbankState));

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        int max = (int)ReadNumber("Введіть N (1-100): ", true, 1, 100);

        while (true)
        {
            Console.WriteLine("\n1 – Додати об'єкт\n2 – Переглянути всі об'єкти\n3 – Знайти об'єкт\n4 – Продемонструвати поведінку\n5 – Видалити об'єкт\n0 – Вийти з програми");
            int c = (int)ReadNumber("Вибір: ", true, 0, 5);

            if (c == 0) { Console.WriteLine("Роботу завершено."); return; }
            if (c == 1) Console.WriteLine(Add(max));
            else if (items.Count == 0) Console.WriteLine("Жодного об'єкта немає.");
            else if (c == 2) Console.WriteLine(Table(Enumerable.Range(0, items.Count)));
            else if (c == 3) Console.WriteLine(Search());
            else if (c == 4) Demo();
            else Console.WriteLine(Delete());
        }
    }

    static string Add(int max)
    {
        if (items.Count >= max) return $"Ліміт N = {max} досягнуто.";

        Powerbank p;
        string how;

        try
        {
            if (ReadNumber("1 – вручну, 2 – автоматично (випадковий конструктор): ", true, 1, 2) == 1)
            {
                p = new Powerbank();
                SetUntilValid(() => p.Model = Input("Модель (3-20 символів, латиниця/цифри/пробіл): "));
                SetUntilValid(() => p.CapacityMah = (int)ReadNumber("Ємність, мА·год (1000-50000): ", true));
                SetUntilValid(() => p.ChargePercent = ReadNumber("Заряд, % (0-100): ", false));
                SetUntilValid(() => p.ManufactureDate = ReadDate());
                SetUntilValid(() => p.State = ReadState());
                how = p.ConstructorInfo + ", значення задано через властивості";
            }
            else
            {
                string model = "PowerBank " + rnd.Next(10, 100);
                int capacity = rnd.Next(2, 31) * 1000;
                double charge = Math.Round(rnd.NextDouble() * 100, 1);
                DateTime date = minDate.AddDays(rnd.Next((DateTime.Today - minDate).Days + 1));
                PowerbankState st = states[rnd.Next(states.Length)];
                bool withInitializer = false;

                switch (rnd.Next(5))
                {
                    case 0:
                        p = new Powerbank
                        {
                            Model = model,
                            CapacityMah = capacity,
                            ChargePercent = charge,
                            ManufactureDate = date,
                            State = st
                        };
                        withInitializer = true;
                        break;
                    case 1: p = new Powerbank(); break;
                    case 2: p = new Powerbank(model, capacity); break;
                    case 3: p = new Powerbank(model, capacity, charge); break;
                    default: p = new Powerbank(model, capacity, charge, date, st); break;
                }
                how = p.ConstructorInfo + (withInitializer ? " + ініціалізатор об'єкта" : "");
            }
        }
        catch (Exception ex)
        {
            return "Об'єкт не створено: " + ex.Message;
        }

        items.Add(p);
        return $"Спрацював конструктор: {how}\nДодано: {p}";
    }

    static string Table(IEnumerable<int> indexes)
    {
        string result = string.Format("{0,-4}{1,-20}{2,-12}{3,-10}{4,-12}{5,-14}", "№", "Модель", "Ємність", "Заряд,%", "Дата", "Стан");
        foreach (int i in indexes)
        {
            Powerbank p = items[i];
            result += "\n" + string.Format("{0,-4}{1,-20}{2,-12}{3,-10:F1}{4,-12:dd.MM.yyyy}{5,-14}",
                                           i + 1, p.Model, p.CapacityMah, p.ChargePercent, p.ManufactureDate, p.State);
        }
        return result;
    }

    static List<int> Find()
    {
        var result = new List<int>();
        if (ReadNumber("Пошук за: 1 – моделлю, 2 – станом: ", true, 1, 2) == 1)
        {
            string q = ReadText("Модель (або частина): ");
            for (int i = 0; i < items.Count; i++)
                if (items[i].Model.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) result.Add(i);
        }
        else
        {
            PowerbankState st = ReadState();
            for (int i = 0; i < items.Count; i++)
                if (items[i].State == st) result.Add(i);
        }
        return result;
    }

    static string Search()
    {
        List<int> found = Find();
        return found.Count == 0 ? "Нічого не знайдено." : Table(found);
    }

    static void Demo()
    {
        Console.WriteLine(Table(Enumerable.Range(0, items.Count)));
        Powerbank p = items[(int)ReadNumber("Номер об'єкта: ", true, 1, items.Count) - 1];

        while (true)
        {
            Console.WriteLine("\n1 – Charge(хвилини)\n2 – Charge(хвилини, струм)\n3 – Discharge\n4 – TogglePower\n5 – EstimateRuntime(Вт)\n6 – EstimateRuntime(В, А)\n7 – ToString\n8 – Властивості (get)\n0 – Назад");
            int c = (int)ReadNumber("Вибір: ", true, 0, 8);

            if (c == 0) return;
            if (c == 1) Console.WriteLine(p.Charge((int)ReadNumber("Хвилин (1-600): ", true, 1, 600)));
            else if (c == 2)
            {
                int min = (int)ReadNumber("Хвилин (1-600): ", true, 1, 600);
                int ma = (int)ReadNumber("Струм зарядки, мА (100-5000): ", true, 100, 5000);
                Console.WriteLine(p.Charge(min, ma));
            }
            else if (c == 3) Console.WriteLine(p.Discharge((int)ReadNumber("мА·год (1-50000): ", true, 1, 50000)));
            else if (c == 4) Console.WriteLine(p.TogglePower());
            else if (c == 5) Console.WriteLine(p.EstimateRuntime(ReadNumber("Потужність, Вт (0.5-200): ", false, 0.5, 200)));
            else if (c == 6)
            {
                double v = ReadNumber("Напруга, В (1-20): ", false, 1, 20);
                double a = ReadNumber("Струм, А (0.1-5): ", false, 0.1, 5);
                Console.WriteLine(p.EstimateRuntime(v, a));
            }
            else if (c == 7) Console.WriteLine(p);
            else Console.WriteLine($"Енергія: {p.StoredEnergyWh:F1} Вт·год, напруга: {p.NominalVoltage} В, заряджань: {p.ChargeCount}, конструктор: {p.ConstructorInfo}");
        }
    }

    static string Delete()
    {
        if (ReadNumber("Видалити: 1 – за номером, 2 – за характеристикою: ", true, 1, 2) == 1)
        {
            Console.WriteLine(Table(Enumerable.Range(0, items.Count)));
            items.RemoveAt((int)ReadNumber("Номер: ", true, 1, items.Count) - 1);
            return "Видалено.";
        }

        List<int> found = Find();
        found.Reverse();
        foreach (int i in found) items.RemoveAt(i);
        return found.Count == 0 ? "Об'єктів не знайдено." : $"Видалено: {found.Count}";
    }

    static void SetUntilValid(Action setValue)
    {
        while (true)
        {
            try
            {
                setValue();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    static string Input(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();
    }

    static string ReadText(string prompt)
    {
        while (true)
        {
            string s = Input(prompt);
            if (s != "") return s;
            Console.WriteLine("Error: відсутнє значення!");
        }
    }

    static double ReadNumber(string prompt, bool integer)
    {
        while (true)
        {
            string s = Input(prompt).Replace(',', '.');

            if (s == "") Console.WriteLine("Error: відсутнє значення!");
            else if (!double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                                      CultureInfo.InvariantCulture, out double v)) Console.WriteLine("Error: не цифри!");
            else if (integer && v != Math.Floor(v)) Console.WriteLine("Error: має бути ціле число!");
            else if (Math.Abs(v) > 1e9) Console.WriteLine("Error: число завелике!");
            else return v;
        }
    }

    static double ReadNumber(string prompt, bool integer, double min, double max)
    {
        while (true)
        {
            double v = ReadNumber(prompt, integer);

            if (v < 0) Console.WriteLine("Error: від'ємне значення!");
            else if (v < min || v > max) Console.WriteLine($"Error: значення має бути від {min} до {max}!");
            else return v;
        }
    }

    static DateTime ReadDate()
    {
        while (true)
        {
            string s = Input("Дата виробництва (dd.MM.yyyy, від 01.01.2015 до сьогодні): ");

            if (s == "") Console.WriteLine("Error: відсутнє значення!");
            else if (!DateTime.TryParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                             DateTimeStyles.None, out DateTime d)) Console.WriteLine("Error: формат має бути dd.MM.yyyy!");
            else return d;
        }
    }

    static PowerbankState ReadState()
    {
        for (int i = 0; i < states.Length; i++)
            Console.WriteLine($"{i + 1} – {states[i]}");
        return states[(int)ReadNumber("Стан: ", true, 1, states.Length) - 1];
    }
}