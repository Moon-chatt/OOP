using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
public static class PowerbankFiles
{
    public const string CsvHeader = "Model,CapacityMah,ChargePercent,ManufactureDate,State";

    public static void SaveToCsv(List<Powerbank> list, string path)
    {
        List<string> lines = new List<string>();
        lines.Add(CsvHeader);

        foreach (Powerbank p in list)
            lines.Add(p.ToString());

        File.WriteAllLines(path, lines);
    }

    public static List<Powerbank> ReadFromCsv(string path, out int skipped)
    {
        List<Powerbank> result = new List<Powerbank>();
        skipped = 0;
        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            Powerbank p;
            if (Powerbank.TryParse(lines[i], out p)) result.Add(p);
            else skipped++;
        }
        return result;
    }

    public static void SaveToJson(List<Powerbank> list, string path)
    {
        string json = JsonConvert.SerializeObject(list, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static List<Powerbank> ReadFromJson(string path)
    {
        string text = File.ReadAllText(path);
        int before = Powerbank.Counter;

        try
        {
            List<Powerbank> list = JsonConvert.DeserializeObject<List<Powerbank>>(text);
            if (list == null) return new List<Powerbank>();
            list.RemoveAll(x => x == null);
            return list;
        }
        catch (Exception ex)
        {
            int created = Powerbank.Counter - before;
            for (int i = 0; i < created; i++)
                Powerbank.DecreaseCounter();

            string reason = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            throw new FormatException("Error: файл JSON містить некоректні дані. " + reason);
        }
    }
}
