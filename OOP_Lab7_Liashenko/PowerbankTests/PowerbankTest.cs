using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[DoNotParallelize]
[TestClass]
public class PowerbankTest
{
    private Powerbank powerbank = new Powerbank();
    private DateTime date = new DateTime(2022, 3, 15);

    [TestInitialize]
    public void Setup()
    {
        Powerbank.StandardCurrentMa = 1500;
        powerbank = new Powerbank("PowerMax 10", 10000, 50, date, PowerbankState.Idle);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Powerbank.StandardCurrentMa = 1500;
    }

    [TestMethod]
    public void ConstructorTest_without_parameters_default_values()
    {
        Powerbank p = new Powerbank();

        Assert.AreEqual("PowerBank", p.Model);
        Assert.AreEqual(1000, p.CapacityMah);
        Assert.AreEqual(0.0, p.ChargePercent, 0.001);
        Assert.AreEqual(PowerbankState.Idle, p.State);
        Assert.IsTrue(p.ConstructorInfo.Contains("без параметрів"));
    }

    [TestMethod]
    public void ConstructorTest_model_and_capacity_charge_is_zero()
    {
        Powerbank p = new Powerbank("VoltBank", 20000);

        Assert.AreEqual("VoltBank", p.Model);
        Assert.AreEqual(20000, p.CapacityMah);
        Assert.AreEqual(0.0, p.ChargePercent, 0.001);
        Assert.AreEqual(DateTime.Today, p.ManufactureDate);
    }

    [TestMethod]
    public void ConstructorTest_model_capacity_charge_state_is_idle()
    {
        Powerbank p = new Powerbank("VoltBank", 20000, 75.5);

        Assert.AreEqual(75.5, p.ChargePercent, 0.001);
        Assert.AreEqual(PowerbankState.Idle, p.State);
    }

    [TestMethod]
    public void ConstructorTest_full_all_values_are_set()
    {
        Assert.AreEqual("PowerMax 10", powerbank.Model);
        Assert.AreEqual(10000, powerbank.CapacityMah);
        Assert.AreEqual(50.0, powerbank.ChargePercent, 0.001);
        Assert.AreEqual(date, powerbank.ManufactureDate);
        Assert.AreEqual(PowerbankState.Idle, powerbank.State);
    }

    [TestMethod]
    public void ConstructorTest_incorrect_model_throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Powerbank("Ab", 10000, 50, date, PowerbankState.Idle));
    }

    [TestMethod]
    public void CounterTest_increases_after_object_creation()
    {
        int before = Powerbank.Counter;

        Powerbank p = new Powerbank("VoltBank", 20000, 10, date, PowerbankState.Idle);

        Assert.IsNotNull(p);
        Assert.AreEqual(before + 1, Powerbank.Counter);
    }

    [TestMethod]
    public void CounterTest_not_changed_after_incorrect_creation()
    {
        int before = Powerbank.Counter;

        Assert.Throws<ArgumentOutOfRangeException>(() => new Powerbank("Ab", 10000, 50, date, PowerbankState.Idle));

        Assert.AreEqual(before, Powerbank.Counter);
    }

    [TestMethod]
    public void DecreaseCounterTest_decreases_by_one()
    {
        int before = Powerbank.Counter;

        Powerbank.DecreaseCounter();

        Assert.AreEqual(before - 1, Powerbank.Counter);
    }

    [TestMethod]
    [DataRow("Abc")]
    [DataRow("PowerMax 10")]
    [DataRow("12345678901234567890")]
    public void ModelSetTest_correct_value_is_saved(string value)
    {
        powerbank.Model = value;

        Assert.AreEqual(value, powerbank.Model);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void ModelSetTest_empty_value_throws_ArgumentNullException(string value)
    {
        Assert.Throws<ArgumentNullException>(() => powerbank.Model = value);
    }

    [TestMethod]
    [DataRow("Ab")]
    [DataRow("123456789012345678901")]
    public void ModelSetTest_wrong_length_throws_ArgumentOutOfRangeException(string value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => powerbank.Model = value);
    }

    [TestMethod]
    [DataRow("Power@Bank")]
    [DataRow("Павербанк")]
    [DataRow("Power-Bank")]
    public void ModelSetTest_wrong_symbols_throws_FormatException(string value)
    {
        Assert.Throws<FormatException>(() => powerbank.Model = value);
    }

    [TestMethod]
    [DataRow(1000)]
    [DataRow(25000)]
    [DataRow(50000)]
    public void CapacityMahSetTest_correct_value_is_saved(int value)
    {
        powerbank.CapacityMah = value;

        Assert.AreEqual(value, powerbank.CapacityMah);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(999)]
    [DataRow(50001)]
    public void CapacityMahSetTest_wrong_value_throws_ArgumentOutOfRangeException(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => powerbank.CapacityMah = value);
    }

    [TestMethod]
    [DataRow(0.0)]
    [DataRow(55.5)]
    [DataRow(100.0)]
    public void ChargePercentSetTest_correct_value_is_saved(double value)
    {
        powerbank.ChargePercent = value;

        Assert.AreEqual(value, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    [DataRow(-0.1)]
    [DataRow(100.1)]
    [DataRow(500.0)]
    public void ChargePercentSetTest_wrong_value_throws_ArgumentOutOfRangeException(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => powerbank.ChargePercent = value);
    }

    [TestMethod]
    public void ManufactureDateSetTest_minimal_date_is_saved()
    {
        DateTime value = new DateTime(2015, 1, 1);

        powerbank.ManufactureDate = value;

        Assert.AreEqual(value, powerbank.ManufactureDate);
    }

    [TestMethod]
    public void ManufactureDateSetTest_today_is_saved()
    {
        powerbank.ManufactureDate = DateTime.Today;

        Assert.AreEqual(DateTime.Today, powerbank.ManufactureDate);
    }

    [TestMethod]
    public void ManufactureDateSetTest_date_before_2015_throws_ArgumentOutOfRangeException()
    {
        DateTime value = new DateTime(2014, 12, 31);

        Assert.Throws<ArgumentOutOfRangeException>(() => powerbank.ManufactureDate = value);
    }

    [TestMethod]
    public void ManufactureDateSetTest_future_date_throws_ArgumentOutOfRangeException()
    {
        DateTime value = DateTime.Today.AddDays(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => powerbank.ManufactureDate = value);
    }

    [TestMethod]
    [DataRow(PowerbankState.Off)]
    [DataRow(PowerbankState.Charging)]
    [DataRow(PowerbankState.Discharging)]
    public void StateSetTest_correct_value_is_saved(PowerbankState value)
    {
        powerbank.State = value;

        Assert.AreEqual(value, powerbank.State);
    }

    [TestMethod]
    public void StateSetTest_undefined_value_throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => powerbank.State = (PowerbankState)99);
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1500)]
    [DataRow(5000)]
    public void StandardCurrentMaSetTest_correct_value_is_saved(int value)
    {
        Powerbank.StandardCurrentMa = value;

        Assert.AreEqual(value, Powerbank.StandardCurrentMa);
    }

    [TestMethod]
    [DataRow(-5)]
    [DataRow(0)]
    [DataRow(99)]
    [DataRow(5001)]
    public void StandardCurrentMaSetTest_wrong_value_throws_ArgumentOutOfRangeException(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Powerbank.StandardCurrentMa = value);
    }

    [TestMethod]
    public void NominalVoltageTest_default_value_is_3_7()
    {
        Assert.AreEqual(3.7, Powerbank.NominalVoltage, 0.001);
    }

    [TestMethod]
    public void StoredEnergyWhTest_half_charged_battery()
    {
        Assert.AreEqual(18.5, powerbank.StoredEnergyWh, 0.001);
    }

    [TestMethod]
    public void StoredEnergyWhTest_empty_battery_is_zero()
    {
        powerbank.ChargePercent = 0;

        Assert.AreEqual(0.0, powerbank.StoredEnergyWh, 0.001);
    }

    [TestMethod]
    [DataRow(0, 50.0)]
    [DataRow(60, 65.0)]
    [DataRow(120, 80.0)]
    [DataRow(600, 100.0)]
    public void ChargeTest_charge_depends_on_minutes(int minutes, double expected)
    {
        powerbank.Charge(minutes);

        Assert.AreEqual(expected, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void ChargeTest_state_changes_and_counter_increases()
    {
        powerbank.Charge(30);

        Assert.AreEqual(PowerbankState.Charging, powerbank.State);
        Assert.AreEqual(1, powerbank.ChargeCount);
    }

    [TestMethod]
    public void ChargeTest_powerbank_is_off_returns_error()
    {
        powerbank.TogglePower();

        string result = powerbank.Charge(60);

        Assert.IsTrue(result.Contains("вимкнений"));
        Assert.AreEqual(50.0, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void ChargeTest_battery_is_full_returns_error()
    {
        powerbank.ChargePercent = 100;

        string result = powerbank.Charge(60);

        Assert.IsTrue(result.Contains("100%"));
        Assert.AreEqual(0, powerbank.ChargeCount);
    }

    [TestMethod]
    public void ChargeTest_uses_standard_current()
    {
        Powerbank.StandardCurrentMa = 3000;

        powerbank.Charge(60);

        Assert.AreEqual(80.0, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void ChargeTest_message_shows_called_version()
    {
        string result = powerbank.Charge(10);

        Assert.IsTrue(result.Contains("Charge(int minutes)."));
    }

    [TestMethod]
    [DataRow(60, 1500, 65.0)]
    [DataRow(60, 3000, 80.0)]
    [DataRow(30, 1000, 55.0)]
    public void ChargeWithCurrentTest_charge_depends_on_current(int minutes, int current, double expected)
    {
        powerbank.Charge(minutes, current);

        Assert.AreEqual(expected, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void ChargeWithCurrentTest_message_shows_called_version()
    {
        string result = powerbank.Charge(10, 1000);

        Assert.IsTrue(result.Contains("Charge(int minutes, int currentMa)"));
    }

    [TestMethod]
    [DataRow(0, 50.0, true)]
    [DataRow(2000, 30.0, true)]
    [DataRow(5000, 0.0, true)]
    [DataRow(6000, 50.0, false)]
    public void DischargeTest_charge_decreases_if_enough(int mah, double expectedCharge, bool expectedResult)
    {
        string result = powerbank.Discharge(mah);

        Assert.AreEqual(expectedResult, result.StartsWith("Віддано"));
        Assert.AreEqual(expectedCharge, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void DischargeTest_state_changes_to_discharging()
    {
        powerbank.Discharge(1000);

        Assert.AreEqual(PowerbankState.Discharging, powerbank.State);
    }

    [TestMethod]
    public void DischargeTest_powerbank_is_off_returns_error()
    {
        powerbank.TogglePower();

        string result = powerbank.Discharge(1000);

        Assert.IsTrue(result.Contains("вимкнений"));
        Assert.AreEqual(50.0, powerbank.ChargePercent, 0.001);
    }

    [TestMethod]
    public void TogglePowerTest_idle_becomes_off()
    {
        string result = powerbank.TogglePower();

        Assert.AreEqual(PowerbankState.Off, powerbank.State);
        Assert.AreEqual("Стан: Off", result);
    }

    [TestMethod]
    public void TogglePowerTest_second_call_turns_on_again()
    {
        powerbank.TogglePower();

        powerbank.TogglePower();

        Assert.AreEqual(PowerbankState.Idle, powerbank.State);
    }

    [TestMethod]
    public void EstimateRuntimeTest_message_shows_called_version()
    {
        string result = powerbank.EstimateRuntime(5);

        Assert.IsTrue(result.Contains("EstimateRuntime(double watts)"));
        Assert.IsTrue(result.Contains("Приблизний час роботи"));
    }

    [TestMethod]
    public void EstimateRuntimeTest_powerbank_is_off_returns_zero()
    {
        powerbank.TogglePower();

        string result = powerbank.EstimateRuntime(5);

        Assert.IsTrue(result.Contains("0 год"));
    }

    [TestMethod]
    public void EstimateRuntimeWithVoltsAmpsTest_gives_same_time_as_watts()
    {
        string byWatts = powerbank.EstimateRuntime(5);
        string byVoltsAmps = powerbank.EstimateRuntime(5, 1);

        string time1 = byWatts.Substring(byWatts.IndexOf("Приблизний"));
        string time2 = byVoltsAmps.Substring(byVoltsAmps.IndexOf("Приблизний"));

        Assert.AreEqual(time1, time2);
        Assert.IsTrue(byVoltsAmps.Contains("EstimateRuntime(double volts, double amps)"));
    }

    [TestMethod]
    public void ToStringTest_returns_string_in_parse_format()
    {
        string result = powerbank.ToString();

        Assert.AreEqual("PowerMax 10,10000,50.0,15.03.2022,Idle", result);
    }

    [TestMethod]
    public void ParseTest_correct_string_creates_object()
    {
        Powerbank p = Powerbank.Parse("VoltBank Pro,20000,75.5,01.06.2020,Charging");

        Assert.AreEqual("VoltBank Pro", p.Model);
        Assert.AreEqual(20000, p.CapacityMah);
        Assert.AreEqual(75.5, p.ChargePercent, 0.001);
        Assert.AreEqual(new DateTime(2020, 6, 1), p.ManufactureDate);
        Assert.AreEqual(PowerbankState.Charging, p.State);
    }

    [TestMethod]
    public void ParseTest_spaces_around_values_and_lower_case_state()
    {
        Powerbank p = Powerbank.Parse("PowerMax 10 , 10000 , 50 , 15.03.2022 , idle");

        Assert.AreEqual("PowerMax 10", p.Model);
        Assert.AreEqual(PowerbankState.Idle, p.State);
    }

    [TestMethod]
    public void ParseTest_result_of_ToString_gives_same_object()
    {
        Powerbank p = Powerbank.Parse(powerbank.ToString());

        Assert.AreEqual(powerbank.Model, p.Model);
        Assert.AreEqual(powerbank.CapacityMah, p.CapacityMah);
        Assert.AreEqual(powerbank.ChargePercent, p.ChargePercent, 0.001);
        Assert.AreEqual(powerbank.ManufactureDate, p.ManufactureDate);
        Assert.AreEqual(powerbank.State, p.State);
    }

    [TestMethod]
    public void ParseTest_null_string_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Powerbank.Parse(null));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(",10000,50,15.03.2022,Idle")]
    public void ParseTest_empty_value_throws_ArgumentNullException(string text)
    {
        Assert.Throws<ArgumentNullException>(() => Powerbank.Parse(text));
    }

    [TestMethod]
    [DataRow("PowerMax 10,10000,50")]
    [DataRow("PowerMax 10,10000,50,15.03.2022,Idle,extra")]
    [DataRow("PowerMax 10,abc,50,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000.5,50,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000,abc,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000,50,2022-03-15,Idle")]
    [DataRow("PowerMax 10,10000,50,15.03.2022,Broken")]
    [DataRow("Power@Bank,10000,50,15.03.2022,Idle")]
    public void ParseTest_wrong_format_throws_FormatException(string text)
    {
        Assert.Throws<FormatException>(() => Powerbank.Parse(text));
    }

    [TestMethod]
    [DataRow("Ab,10000,50,15.03.2022,Idle")]
    [DataRow("PowerMax 10,500,50,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000,150,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000,-5,15.03.2022,Idle")]
    [DataRow("PowerMax 10,10000,50,01.01.2010,Idle")]
    public void ParseTest_value_out_of_range_throws_ArgumentOutOfRangeException(string text)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Powerbank.Parse(text));
    }

    [TestMethod]
    [DataRow("PowerMax 10,10000,50.5,15.03.2022,Idle", true)]
    [DataRow("Ab,10000,50,15.03.2022,Idle", false)]
    [DataRow("PowerMax 10,10000,50", false)]
    [DataRow("abc", false)]
    [DataRow("", false)]
    public void TryParseTest_returns_true_only_for_correct_string(string text, bool expected)
    {
        bool actual = Powerbank.TryParse(text, out Powerbank result);

        Assert.AreEqual(expected, actual);
        Assert.AreEqual(expected, result != null);
    }

    [TestMethod]
    public void TryParseTest_correct_string_has_empty_error()
    {
        bool actual = Powerbank.TryParse("PowerMax 10,10000,50.5,15.03.2022,Idle", out Powerbank result, out string error);

        Assert.IsTrue(actual);
        Assert.AreEqual("", error);
        Assert.AreEqual("PowerMax 10", result.Model);
    }

    [TestMethod]
    public void TryParseTest_incorrect_string_has_error_message()
    {
        bool actual = Powerbank.TryParse("Ab,10000,50,15.03.2022,Idle", out Powerbank result, out string error);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
        Assert.IsTrue(error.Contains("Error"));
    }

    [TestMethod]
    public void TryParseTest_counter_increases_only_for_correct_string()
    {
        int before = Powerbank.Counter;

        Powerbank.TryParse("Ab,10000,50,15.03.2022,Idle", out Powerbank bad);
        Assert.AreEqual(before, Powerbank.Counter);

        Powerbank.TryParse("PowerMax 10,10000,50,15.03.2022,Idle", out Powerbank good);
        Assert.AreEqual(before + 1, Powerbank.Counter);
    }

    [TestMethod]
    [DataRow(1000, 3.7)]
    [DataRow(10000, 37.0)]
    [DataRow(20000, 74.0)]
    [DataRow(50000, 185.0)]
    public void MahToWhTest_converts_correctly(int mah, double expected)
    {
        double actual = Powerbank.MahToWh(mah);

        Assert.AreEqual(expected, actual, 0.001);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-100)]
    public void MahToWhTest_not_positive_value_throws_ArgumentOutOfRangeException(int mah)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Powerbank.MahToWh(mah));
    }

    [TestMethod]
    [DataRow(10000, 1500, 400)]
    [DataRow(10000, 3000, 200)]
    [DataRow(1000, 1500, 40)]
    [DataRow(1001, 1500, 41)]
    [DataRow(5000, 500, 600)]
    public void MinutesToFullChargeTest_calculates_minutes(int capacity, int current, int expected)
    {
        int actual = Powerbank.MinutesToFullCharge(capacity, current);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 1500)]
    [DataRow(10000, 0)]
    [DataRow(-1, 1500)]
    public void MinutesToFullChargeTest_not_positive_value_throws_ArgumentOutOfRangeException(int capacity, int current)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Powerbank.MinutesToFullCharge(capacity, current));
    }
}
