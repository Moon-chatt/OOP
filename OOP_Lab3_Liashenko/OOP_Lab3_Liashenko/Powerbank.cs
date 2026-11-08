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

    public string ConstructorInfo { get; private set; }

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

    public Powerbank() : this("PowerBank", 1000)
    {
        ConstructorInfo = "Powerbank() – без параметрів";
    }

    public Powerbank(string model, int capacityMah) : this(model, capacityMah, 0)
    {
        ConstructorInfo = "Powerbank(string, int) – модель і ємність";
    }

    public Powerbank(string model, int capacityMah, double chargePercent)
        : this(model, capacityMah, chargePercent, DateTime.Today, PowerbankState.Idle)
    {
        ConstructorInfo = "Powerbank(string, int, double) – модель, ємність і заряд";
    }

    public Powerbank(string model, int capacityMah, double chargePercent, DateTime manufactureDate, PowerbankState state)
    {
        Model = model;
        CapacityMah = capacityMah;
        ChargePercent = chargePercent;
        ManufactureDate = manufactureDate;
        State = state;
        ConstructorInfo = "Powerbank(string, int, double, DateTime, PowerbankState) – повний";
    }

    public string Charge(int minutes)
    {
        return "Виклик Charge(int minutes). " + DoCharge(minutes, 1500);
    }

    public string Charge(int minutes, int currentMa)
    {
        return "Виклик Charge(int minutes, int currentMa). " + DoCharge(minutes, currentMa);
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
        return "Виклик EstimateRuntime(double watts). " + Runtime(watts);
    }

    public string EstimateRuntime(double volts, double amps)
    {
        return "Виклик EstimateRuntime(double volts, double amps). " + Runtime(volts * amps);
    }

    public override string ToString()
    {
        return $"{Model} ({CapacityMah} мА·год), заряд {ChargePercent:F1}%, {State}, заряджань: {ChargeCount}";
    }

    private double PercentFromMah(double mah)
    {
        return mah / CapacityMah * 100;
    }

    private string DoCharge(int minutes, int currentMa)
    {
        if (state == PowerbankState.Off) return "Неможливо: повербанк вимкнений.";
        if (chargePercent >= 100) return "Неможливо: повербанк уже заряджений на 100%.";
        chargePercent = Math.Min(100, chargePercent + PercentFromMah((double)currentMa * minutes / 60));
        state = PowerbankState.Charging;
        ChargeCount++;
        return "Заряджено: " + this;
    }

    private string Runtime(double watts)
    {
        if (state == PowerbankState.Off) return "Повербанк вимкнений, час роботи 0 год.";
        double hours = StoredEnergyWh * Efficiency / watts;
        return $"Приблизний час роботи: {hours:F1} год";
    }
}