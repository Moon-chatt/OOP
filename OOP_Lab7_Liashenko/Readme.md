# Повербанк – Практична робота (Lab-7): Серіалізація / десеріалізація

**Дисципліна:** Об'єктно-орієнтоване програмування  
**Виконала:** Ляшенко К.В., група 621п, освітня програма 121 «Інженерія програмного забезпечення»

## Опис

Предметна область – повербанк (портативний акумулятор). Реалізовано:

- збереження та зчитування колекції у форматах CSV (із заголовком) та JSON (Newtonsoft.Json);
- при читанні CSV некоректні рядки пропускаються, виводиться кількість доданих і пропущених;
- JSON: перейменовані ключі (`[JsonProperty]`), службові властивості не зберігаються (`[JsonIgnore]`), стан текстом;
- зчитані об'єкти додаються до наявної колекції (з урахуванням ліміту N);
- unit-тести для сховища (`AddRange`) та файлового класу `PowerbankFiles`.

## Вимоги

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Visual Studio 2022/2026 (необов'язково, можна `dotnet` CLI)
- NuGet-пакет `Newtonsoft.Json` (відновлюється автоматично)
- NuGet-пакет `MSTest` (відновлюється автоматично)

## Запуск

```bash
git clone https://github.com/<ваш-логін>/OOP_Lab7_Liashenko.git
cd OOP_Lab7_Liashenko
dotnet run --project OOP_Lab7_Liashenko
```

Або відкрийте `OOP_Lab7_Liashenko.slnx` у Visual Studio і натисніть **Ctrl+F5**.

## Меню програми

| Пункт | Дія |
|---|---|
| 1 | Додати об'єкт |
| 2 | Переглянути всі об'єкти |
| 3 | Знайти об'єкт |
| 4 | Продемонструвати поведінку |
| 5 | Видалити об'єкт |
| 6 | Продемонструвати static-методи |
| 7 | Зберегти колекцію об'єктів у файл (CSV / JSON) |
| 8 | Зчитати колекцію об'єктів з файлу (CSV / JSON) |
| 9 | Очистити колекцію об'єктів |
| 0 | Вийти з програми |

На початку програма запитує N – максимальну кількість об'єктів (1–100).

## Формат рядка (Parse / TryParse / ToString)

```
Model,Capacity,Charge,dd.MM.yyyy,State
PowerMax 10,10000,50,15.03.2022,Idle
```

Допустимі значення: модель 3–20 символів (латиниця, цифри, пробіл); ємність 1000–50000 мА·год; заряд 0–100 %; дата від 01.01.2015 до сьогодні; стан `Off`, `Idle`, `Charging`, `Discharging`.

## Формат файлів

**CSV** (перший рядок – заголовок):

```
Model,CapacityMah,ChargePercent,ManufactureDate,State
PowerMax 10,10000,50.0,15.03.2022,Idle
```

**JSON** (масив об'єктів):

```json
[
  {
    "model": "PowerMax 10",
    "capacity_mah": 10000,
    "charge_percent": 50.0,
    "manufacture_date": "2022-03-15T00:00:00",
    "state": "Idle"
  }
]
```

Файли створюються в поточній теці (за замовчуванням `bin/Debug/net10.0`).

## Тести

```bash
dotnet test
```

У Visual Studio: **Test → Run All Tests**; покриття коду: **Test → Analyze Code Coverage for All Tests**.

## Структура репозиторію

```
OOP_Lab7_Liashenko/
├── OOP_Lab7_Liashenko.slnx
├── OOP_Lab7_Liashenko/
│   ├── Powerbank.cs
│   ├── PowerbankState.cs
│   ├── PowerbankStorage.cs
│   ├── PowerbankFiles.cs
│   ├── Program.cs
├── PowerbankTests/
```

| Файл | Призначення |
|---|---|
| `Powerbank.cs` | клас повербанка (з JSON-атрибутами) |
| `PowerbankState.cs` | перелік станів |
| `PowerbankStorage.cs` | сховище об'єктів |
| `PowerbankFiles.cs` | робота з CSV та JSON |
| `Program.cs` | консольне меню |
