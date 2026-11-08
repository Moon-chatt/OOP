using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Powerbank
{
    private const double Efficiency = 0.85;

    private static int counter;
    private static int standardCurrentMa = 1500;

    private string model;
    private int capacityMah;
    private double chargePercent;
    private DateTime manufactureDate;
    private PowerbankState state;

    public static int Counter
    {
        get { return counter; }
    }

    public static int StandardCurrentMa
    {
        get { return standardCurrentMa; }
        set
        {
            if (value < 100 || value > 5000)
                throw new ArgumentOutOfRangeException(null, "Error: струм має бути від 100 до 5000 мА!");
            standardCurrentMa = value;
        }
    }

    public static double NominalVoltage { get; } = 3.7;

    [JsonIgnore]
    public int ChargeCount { get; private set; } = 0;

    [JsonIgnore]
    public string ConstructorInfo { get; private set; }

    [JsonProperty("model")]
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

    [JsonProperty("capacity_mah")]
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

    [JsonProperty("charge_percent")]
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

    [JsonProperty("manufacture_date")]
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

    [JsonProperty("state")]
    [JsonConverter(typeof(StringEnumConverter))]
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

    [JsonIgnore]
    public double StoredEnergyWh => MahToWh(CapacityMah) * ChargePercent / 100;

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
        counter++;
    }

    public string Charge(int minutes)
    {
        return "Виклик Charge(int minutes). " + DoCharge(minutes, StandardCurrentMa);
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
        return "Віддано: " + Describe();
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

    public static double MahToWh(int mah)
    {
        if (mah <= 0)
            throw new ArgumentOutOfRangeException(null, "Error: ємність має бути додатною!");
        return mah / 1000.0 * NominalVoltage;
    }

    public static int MinutesToFullCharge(int capacity, int currentMa)
    {
        if (capacity <= 0 || currentMa <= 0)
            throw new ArgumentOutOfRangeException(null, "Error: ємність і струм мають бути додатними!");
        return (int)Math.Ceiling(capacity * 60.0 / currentMa);
    }

    public static void DecreaseCounter()
    {
        if (counter > 0) counter--;
    }

    public static Powerbank Parse(string s)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentNullException(null, "Error: рядок не може бути порожнім!");

        string[] parts = s.Split(',');
        if (parts.Length != 5)
            throw new FormatException("Error: рядок має містити 5 значень, розділених комами!");

        int capacity;
        if (!int.TryParse(parts[1].Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out capacity))
            throw new FormatException("Error: ємність має бути цілим числом!");

        double charge;
        if (!double.TryParse(parts[2].Trim(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                             CultureInfo.InvariantCulture, out charge))
            throw new FormatException("Error: заряд має бути числом!");

        DateTime date;
        if (!DateTime.TryParseExact(parts[3].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out date))
            throw new FormatException("Error: дата має бути у форматі dd.MM.yyyy!");

        PowerbankState st;
        if (!Enum.TryParse(parts[4].Trim(), true, out st))
            throw new FormatException("Error: невідомий стан повербанка!");

        return new Powerbank(parts[0].Trim(), capacity, charge, date, st);
    }

    public static bool TryParse(string s, out Powerbank powerbank)
    {
        string error;
        return TryParse(s, out powerbank, out error);
    }

    public static bool TryParse(string s, out Powerbank powerbank, out string error)
    {
        powerbank = null;
        error = "";
        try
        {
            powerbank = Parse(s);
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public override string ToString()
    {
        return $"{Model},{CapacityMah},{ChargePercent.ToString("F1", CultureInfo.InvariantCulture)},{ManufactureDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)},{State}";
    }

    private string Describe()
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
        return "Заряджено: " + Describe();
    }

    private string Runtime(double watts)
    {
        if (state == PowerbankState.Off) return "Повербанк вимкнений, час роботи 0 год.";
        double hours = StoredEnergyWh * Efficiency / watts;
        return $"Приблизний час роботи: {hours:F1} год";
    }
}
