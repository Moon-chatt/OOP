using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

[TestClass]
public class PowerbankStorageTest
{
    private PowerbankStorage storage = new PowerbankStorage(5);
    private Powerbank a = new Powerbank();
    private Powerbank b = new Powerbank();
    private Powerbank c = new Powerbank();
    private DateTime date = new DateTime(2022, 3, 15);

    [TestInitialize]
    public void Setup()
    {
        storage = new PowerbankStorage(5);
        a = new Powerbank("PowerMax 10", 10000, 50, date, PowerbankState.Idle);
        b = new Powerbank("VoltBank Pro", 20000, 80, date, PowerbankState.Charging);
        c = new Powerbank("PowerMax 20", 5000, 10, date, PowerbankState.Off);
        storage.Add(a);
        storage.Add(b);
        storage.Add(c);
    }

    [TestCleanup]
    public void Cleanup()
    {
        storage.Clear();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    [DataRow(100)]
    public void ConstructorTest_correct_max_count_is_saved(int max)
    {
        PowerbankStorage s = new PowerbankStorage(max);

        Assert.AreEqual(max, s.MaxCount);
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ConstructorTest_wrong_max_count_throws_ArgumentOutOfRangeException(int max)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PowerbankStorage(max));
    }

    [TestMethod]
    public void AddTest_new_object_is_added()
    {
        Powerbank d = new Powerbank("NewBank", 8000, 20, date, PowerbankState.Idle);

        bool result = storage.Add(d);

        Assert.IsTrue(result);
        Assert.AreEqual(4, storage.Count);
        Assert.IsTrue(storage.Contains(d));
    }

    [TestMethod]
    public void AddTest_null_is_not_added()
    {
        bool result = storage.Add(null);

        Assert.IsFalse(result);
        Assert.AreEqual(3, storage.Count);
    }

    [TestMethod]
    public void AddTest_full_storage_does_not_add_object()
    {
        PowerbankStorage small = new PowerbankStorage(2);
        small.Add(a);
        small.Add(b);

        bool result = small.Add(c);

        Assert.IsFalse(result);
        Assert.AreEqual(2, small.Count);
        Assert.IsFalse(small.Contains(c));
    }

    [TestMethod]
    public void IsFullTest_true_when_count_equals_max()
    {
        PowerbankStorage small = new PowerbankStorage(2);
        small.Add(a);
        small.Add(b);

        Assert.IsTrue(small.IsFull);
        Assert.IsFalse(storage.IsFull);
    }

    [TestMethod]
    public void GetByNumberTest_returns_right_object()
    {
        Assert.AreSame(a, storage.GetByNumber(1));
        Assert.AreSame(b, storage.GetByNumber(2));
        Assert.AreSame(c, storage.GetByNumber(3));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(4)]
    public void GetByNumberTest_wrong_number_throws_ArgumentOutOfRangeException(int number)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.GetByNumber(number));
    }

    [TestMethod]
    public void GetNumberTest_returns_position_starting_from_one()
    {
        Assert.AreEqual(1, storage.GetNumber(a));
        Assert.AreEqual(3, storage.GetNumber(c));
    }

    [TestMethod]
    public void GetNumberTest_object_is_not_in_storage_returns_zero()
    {
        Powerbank other = new Powerbank();

        Assert.AreEqual(0, storage.GetNumber(other));
    }

    [TestMethod]
    public void GetAllTest_returns_copy_of_list()
    {
        List<Powerbank> list = storage.GetAll();

        list.Clear();

        Assert.AreEqual(3, storage.Count);
    }

    [TestMethod]
    [DataRow("power", 2)]
    [DataRow("POWERMAX 10", 1)]
    [DataRow("voltbank", 1)]
    [DataRow("zzz", 0)]
    [DataRow("", 0)]
    public void FindByModelTest_finds_objects_ignoring_case(string text, int expectedCount)
    {
        List<Powerbank> result = storage.FindByModel(text);

        Assert.AreEqual(expectedCount, result.Count);
    }

    [TestMethod]
    public void FindByModelTest_returns_right_objects()
    {
        List<Powerbank> result = storage.FindByModel("PowerMax");

        Assert.AreSame(a, result[0]);
        Assert.AreSame(c, result[1]);
    }

    [TestMethod]
    [DataRow(PowerbankState.Idle, 1)]
    [DataRow(PowerbankState.Charging, 1)]
    [DataRow(PowerbankState.Off, 1)]
    [DataRow(PowerbankState.Discharging, 0)]
    public void FindByStateTest_finds_objects_with_state(PowerbankState state, int expectedCount)
    {
        List<Powerbank> result = storage.FindByState(state);

        Assert.AreEqual(expectedCount, result.Count);
    }

    [TestMethod]
    public void FindByStateTest_finds_all_objects_with_same_state()
    {
        b.State = PowerbankState.Idle;

        List<Powerbank> result = storage.FindByState(PowerbankState.Idle);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void RemoveByNumberTest_correct_number_removes_object()
    {
        int before = Powerbank.Counter;

        bool result = storage.RemoveByNumber(1);

        Assert.IsTrue(result);
        Assert.AreEqual(2, storage.Count);
        Assert.IsFalse(storage.Contains(a));
        Assert.AreEqual(before - 1, Powerbank.Counter);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(4)]
    public void RemoveByNumberTest_wrong_number_removes_nothing(int number)
    {
        int before = Powerbank.Counter;

        bool result = storage.RemoveByNumber(number);

        Assert.IsFalse(result);
        Assert.AreEqual(3, storage.Count);
        Assert.AreEqual(before, Powerbank.Counter);
    }

    [TestMethod]
    public void RemoveByModelTest_removes_all_found_objects()
    {
        int before = Powerbank.Counter;

        int removed = storage.RemoveByModel("PowerMax");

        Assert.AreEqual(2, removed);
        Assert.AreEqual(1, storage.Count);
        Assert.IsTrue(storage.Contains(b));
        Assert.AreEqual(before - 2, Powerbank.Counter);
    }

    [TestMethod]
    [DataRow("zzz")]
    [DataRow("")]
    public void RemoveByModelTest_nothing_found_removes_nothing(string text)
    {
        int removed = storage.RemoveByModel(text);

        Assert.AreEqual(0, removed);
        Assert.AreEqual(3, storage.Count);
    }

    [TestMethod]
    public void RemoveByStateTest_removes_objects_with_state()
    {
        int removed = storage.RemoveByState(PowerbankState.Off);

        Assert.AreEqual(1, removed);
        Assert.AreEqual(2, storage.Count);
        Assert.IsFalse(storage.Contains(c));
    }

    [TestMethod]
    public void RemoveByStateTest_no_objects_with_state_removes_nothing()
    {
        int removed = storage.RemoveByState(PowerbankState.Discharging);

        Assert.AreEqual(0, removed);
        Assert.AreEqual(3, storage.Count);
    }

    [TestMethod]
    public void ClearTest_removes_all_objects_and_decreases_counter()
    {
        int before = Powerbank.Counter;

        storage.Clear();

        Assert.AreEqual(0, storage.Count);
        Assert.AreEqual(before - 3, Powerbank.Counter);
    }

    [TestMethod]
    public void ContainsTest_true_for_added_and_false_for_other_object()
    {
        Powerbank other = new Powerbank();

        Assert.IsTrue(storage.Contains(a));
        Assert.IsFalse(storage.Contains(other));
    }
}
