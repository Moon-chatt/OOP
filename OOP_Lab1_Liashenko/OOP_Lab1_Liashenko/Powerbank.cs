using System;
using System.Text.RegularExpressions;

public class Powerbank
{
    private const double Efficiency = 0.85;

    private string model;
    private int capacityMah;
    private double chargePercent;
    private DateTime manufactureDate;
    private PowerbankState state;

    public double NominalVoltage { get; } = 3.7;

    public int ChargeCount { get; private set; } = 0;

    public string Model
    {
        get { return model; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(null, "Error: відсутнє значення!");
            if (value.Length < 3 || value.Length > 20)
                throw new ArgumentOutOfRangeException(null, "Error: довжина має бути від 3 до 20 символів!");
            if (!Regex.IsMatch(value, "^[A-Za-z0-9 ]+$"))
                throw new FormatException("Error: лише латинські літери, цифри та пробіл!");
            model = value;
        }
    }

    public int CapacityMah
    {
        get { return capacityMah; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(null, "Error: від'ємне значення!");
            if (value < 1000 || value > 50000)
                throw new ArgumentOutOfRangeException(null, "Error: ємність має бути від 1000 до 50000!");
            capacityMah = value;
        }
    }

    public double ChargePercent
    {
        get { return chargePercent; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(null, "Error: від'ємне значення!");
            if (value > 100)
                throw new ArgumentOutOfRangeException(null, "Error: заряд має бути в діапазоні [0..100]!");
            chargePercent = value;
        }
    }

    public DateTime ManufactureDate
    {
        get { return manufactureDate; }
        set
        {
            if (value < new DateTime(2015, 1, 1))
                throw new ArgumentOutOfRangeException(null, "Error: дата не раніше за 01.01.2015!");
            if (value > DateTime.Today)
                throw new ArgumentOutOfRangeException(null, "Error: дата не може бути в майбутньому!");
            manufactureDate = value;
        }
    }

    public PowerbankState State
    {
        get { return state; }
        set
        {
            if (!Enum.IsDefined(typeof(PowerbankState), value))
                throw new ArgumentException("Error: недопустимий стан!");
            state = value;
        }
    }

    public double StoredEnergyWh => CapacityMah / 1000.0 * NominalVoltage * ChargePercent / 100;

    public string Charge(int minutes)
    {
        if (state == PowerbankState.Off) return "Неможливо: повербанк вимкнений.";
        if (chargePercent >= 100) return "Неможливо: повербанк уже заряджений на 100%.";
        chargePercent = Math.Min(100, chargePercent + PercentFromMah(1500.0 * minutes / 60));
        state = PowerbankState.Charging;
        ChargeCount++;
        return "Заряджено: " + this;
    }

    public string Discharge(int mah)
    {
        double need = PercentFromMah(mah);
        if (state == PowerbankState.Off) return "Неможливо: повербанк вимкнений.";
        if (need > chargePercent) return "Неможливо: недостатньо заряду.";
        chargePercent -= need;
        state = PowerbankState.Discharging;
        return "Віддано: " + this;
    }

    public string TogglePower()
    {
        state = state == PowerbankState.Off ? PowerbankState.Idle : PowerbankState.Off;
        return "Стан: " + state;
    }

    public string EstimateRuntime(double watts)
    {
        if (state == PowerbankState.Off) return "Повербанк вимкнений, час роботи 0 год.";
        double hours = StoredEnergyWh * Efficiency / watts;
        return $"Приблизний час роботи: {hours:F1} год";
    }

    public override string ToString()
    {
        return $"{Model} ({CapacityMah} мА·год), заряд {ChargePercent:F1}%, {State}, заряджань: {ChargeCount}";
    }

    private double PercentFromMah(double mah)
    {
        return mah / CapacityMah * 100;
    }
}