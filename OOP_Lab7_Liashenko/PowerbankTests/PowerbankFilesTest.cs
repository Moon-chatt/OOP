using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

[DoNotParallelize]
[TestClass]
public class PowerbankFilesTest
{
    private List<Powerbank> list = new List<Powerbank>();
    private List<string> tempFiles = new List<string>();
    private DateTime date = new DateTime(2022, 3, 15);

    [TestInitialize]
    public void Setup()
    {
        list = new List<Powerbank>();
        list.Add(new Powerbank("PowerMax 10", 10000, 50, date, PowerbankState.Idle));
        list.Add(new Powerbank("VoltBank Pro", 20000, 80.5, date, PowerbankState.Charging));
        list.Add(new Powerbank("EcoCell", 5000, 10, date, PowerbankState.Off));
    }

    [TestCleanup]
    public void Cleanup()
    {
        foreach (string file in tempFiles)
        {
            if (File.Exists(file)) File.Delete(file);
        }
        tempFiles.Clear();
    }

    private string TempFile(string extension)
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
        tempFiles.Add(path);
        return path;
    }

    private string ValidJson(string model)
    {
        return "{\"model\":\"" + model + "\",\"capacity_mah\":10000,\"charge_percent\":50,\"manufacture_date\":\"2022-03-15T00:00:00\",\"state\":\"Idle\"}";
    }

    [TestMethod]
    public void SaveToCsvTest_first_line_is_header()
    {
        string path = TempFile(".csv");

        PowerbankFiles.SaveToCsv(list, path);

        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(PowerbankFiles.CsvHeader, lines[0]);
    }

    [TestMethod]
    public void SaveToCsvTest_file_has_header_and_row_for_each_object()
    {
        string path = TempFile(".csv");

        PowerbankFiles.SaveToCsv(list, path);

        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(4, lines.Length);
        Assert.AreEqual(list[0].ToString(), lines[1]);
        Assert.AreEqual(list[2].ToString(), lines[3]);
    }

    [TestMethod]
    public void SaveToCsvTest_empty_list_saves_only_header()
    {
        string path = TempFile(".csv");

        PowerbankFiles.SaveToCsv(new List<Powerbank>(), path);

        string[] lines = File.ReadAllLines(path);
        Assert.AreEqual(1, lines.Length);
    }

    [TestMethod]
    public void ReadFromCsvTest_reads_saved_objects()
    {
        string path = TempFile(".csv");
        PowerbankFiles.SaveToCsv(list, path);

        List<Powerbank> result = PowerbankFiles.ReadFromCsv(path, out int skipped);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(0, skipped);
        Assert.AreEqual("VoltBank Pro", result[1].Model);
        Assert.AreEqual(20000, result[1].CapacityMah);
        Assert.AreEqual(80.5, result[1].ChargePercent, 0.001);
        Assert.AreEqual(date, result[1].ManufactureDate);
        Assert.AreEqual(PowerbankState.Charging, result[1].State);
    }

    [TestMethod]
    public void ReadFromCsvTest_incorrect_rows_are_skipped()
    {
        string path = TempFile(".csv");
        string[] lines =
        {
            PowerbankFiles.CsvHeader,
            "PowerMax 10,10000,50.0,15.03.2022,Idle",
            "Ab,10000,50.0,15.03.2022,Idle",
            "VoltBank Pro,20000,80.5,15.03.2022,Charging",
            "abc"
        };
        File.WriteAllLines(path, lines);

        List<Powerbank> result = PowerbankFiles.ReadFromCsv(path, out int skipped);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, skipped);
    }

    [TestMethod]
    public void ReadFromCsvTest_empty_lines_are_ignored()
    {
        string path = TempFile(".csv");
        string[] lines =
        {
            PowerbankFiles.CsvHeader,
            "PowerMax 10,10000,50.0,15.03.2022,Idle",
            "",
            "VoltBank Pro,20000,80.5,15.03.2022,Charging"
        };
        File.WriteAllLines(path, lines);

        List<Powerbank> result = PowerbankFiles.ReadFromCsv(path, out int skipped);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(0, skipped);
    }

    [TestMethod]
    public void ReadFromCsvTest_header_is_not_read_as_object()
    {
        string path = TempFile(".csv");
        File.WriteAllLines(path, new string[] { PowerbankFiles.CsvHeader });

        List<Powerbank> result = PowerbankFiles.ReadFromCsv(path, out int skipped);

        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(0, skipped);
    }

    [TestMethod]
    public void ReadFromCsvTest_counter_increases_only_for_correct_rows()
    {
        string path = TempFile(".csv");
        string[] lines =
        {
            PowerbankFiles.CsvHeader,
            "PowerMax 10,10000,50.0,15.03.2022,Idle",
            "Ab,10000,50.0,15.03.2022,Idle",
            "VoltBank Pro,20000,80.5,15.03.2022,Charging",
            "abc"
        };
        File.WriteAllLines(path, lines);
        int before = Powerbank.Counter;

        PowerbankFiles.ReadFromCsv(path, out int skipped);

        Assert.AreEqual(before + 2, Powerbank.Counter);
    }

    [TestMethod]
    public void ReadFromCsvTest_file_not_found_throws_FileNotFoundException()
    {
        string path = TempFile(".csv");

        Assert.Throws<FileNotFoundException>(() => PowerbankFiles.ReadFromCsv(path, out int skipped));
    }

    [TestMethod]
    public void SaveToJsonTest_property_names_are_renamed()
    {
        string path = TempFile(".json");

        PowerbankFiles.SaveToJson(list, path);

        string json = File.ReadAllText(path);
        Assert.IsTrue(json.Contains("\"model\""));
        Assert.IsTrue(json.Contains("\"capacity_mah\""));
        Assert.IsTrue(json.Contains("\"charge_percent\""));
        Assert.IsTrue(json.Contains("\"manufacture_date\""));
        Assert.IsTrue(json.Contains("\"state\""));
        Assert.IsFalse(json.Contains("CapacityMah"));
    }

    [TestMethod]
    public void SaveToJsonTest_not_main_properties_are_not_saved()
    {
        string path = TempFile(".json");

        PowerbankFiles.SaveToJson(list, path);

        string json = File.ReadAllText(path);
        Assert.IsFalse(json.Contains("ChargeCount"));
        Assert.IsFalse(json.Contains("ConstructorInfo"));
        Assert.IsFalse(json.Contains("StoredEnergyWh"));
    }

    [TestMethod]
    public void SaveToJsonTest_state_is_saved_as_text()
    {
        string path = TempFile(".json");

        PowerbankFiles.SaveToJson(list, path);

        string json = File.ReadAllText(path);
        Assert.IsTrue(json.Contains("\"Charging\""));
    }

    [TestMethod]
    public void SaveToJsonTest_file_contains_array_of_all_objects()
    {
        string path = TempFile(".json");

        PowerbankFiles.SaveToJson(list, path);

        string json = File.ReadAllText(path);
        Assert.IsTrue(json.TrimStart().StartsWith("["));
        Assert.AreEqual(3, PowerbankFiles.ReadFromJson(path).Count);
    }

    [TestMethod]
    public void ReadFromJsonTest_reads_saved_objects()
    {
        string path = TempFile(".json");
        PowerbankFiles.SaveToJson(list, path);

        List<Powerbank> result = PowerbankFiles.ReadFromJson(path);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("VoltBank Pro", result[1].Model);
        Assert.AreEqual(20000, result[1].CapacityMah);
        Assert.AreEqual(80.5, result[1].ChargePercent, 0.001);
        Assert.AreEqual(date, result[1].ManufactureDate);
        Assert.AreEqual(PowerbankState.Charging, result[1].State);
    }

    [TestMethod]
    public void ReadFromJsonTest_counter_increases_by_number_of_objects()
    {
        string path = TempFile(".json");
        PowerbankFiles.SaveToJson(list, path);
        int before = Powerbank.Counter;

        PowerbankFiles.ReadFromJson(path);

        Assert.AreEqual(before + 3, Powerbank.Counter);
    }

    [TestMethod]
    public void ReadFromJsonTest_incorrect_value_throws_FormatException_and_counter_is_not_changed()
    {
        string path = TempFile(".json");
        string json = "[" + ValidJson("PowerMax 10") + "," + ValidJson("Ab") + "," + ValidJson("VoltBank") + "]";
        File.WriteAllText(path, json);
        int before = Powerbank.Counter;

        Assert.Throws<FormatException>(() => PowerbankFiles.ReadFromJson(path));

        Assert.AreEqual(before, Powerbank.Counter);
    }

    [TestMethod]
    public void ReadFromJsonTest_broken_json_throws_FormatException()
    {
        string path = TempFile(".json");
        File.WriteAllText(path, "this is not json");

        Assert.Throws<FormatException>(() => PowerbankFiles.ReadFromJson(path));
    }

    [TestMethod]
    public void ReadFromJsonTest_empty_file_returns_empty_list()
    {
        string path = TempFile(".json");
        File.WriteAllText(path, "");

        List<Powerbank> result = PowerbankFiles.ReadFromJson(path);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ReadFromJsonTest_null_elements_are_skipped()
    {
        string path = TempFile(".json");
        File.WriteAllText(path, "[null," + ValidJson("PowerMax 10") + "]");

        List<Powerbank> result = PowerbankFiles.ReadFromJson(path);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void ReadFromJsonTest_file_not_found_throws_FileNotFoundException()
    {
        string path = TempFile(".json");

        Assert.Throws<FileNotFoundException>(() => PowerbankFiles.ReadFromJson(path));
    }
}
