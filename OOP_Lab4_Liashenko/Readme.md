# Повербанк – Практична робота (Lab-4): Статичні поля, властивості, методи

**Дисципліна:** Об'єктно-орієнтоване програмування  
**Виконала:** Ляшенко К.В., група 621п, освітня програма 121 «Інженерія програмного забезпечення»

## Опис

Предметна область – повербанк (портативний акумулятор). Реалізовано:

- static-властивості: `Counter` (лічильник створених об'єктів), `StandardCurrentMa`, `NominalVoltage`;
- static-методи: `MahToWh`, `MinutesToFullCharge`, `DecreaseCounter`, `Parse`, `TryParse`;
- `ToString()` у форматі, який розуміють `Parse`/`TryParse`;
- додавання об'єкта з рядка.

## Вимоги

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Visual Studio 2022/2026 (необов'язково, можна `dotnet` CLI)

## Запуск

```bash
git clone https://github.com/<ваш-логін>/OOP_Lab4_Liashenko.git
cd OOP_Lab4_Liashenko
dotnet run --project OOP_Lab4_Liashenko
```

Або відкрийте `OOP_Lab4_Liashenko.slnx` у Visual Studio і натисніть **Ctrl+F5**.

## Меню програми

| Пункт | Дія |
|---|---|
| 1 | Додати об'єкт |
| 2 | Переглянути всі об'єкти |
| 3 | Знайти об'єкт |
| 4 | Продемонструвати поведінку |
| 5 | Видалити об'єкт |
| 6 | Продемонструвати static-методи |
| 0 | Вийти з програми |

На початку програма запитує N – максимальну кількість об'єктів (1–100).

## Формат рядка (Parse / TryParse / ToString)

```
Model,Capacity,Charge,dd.MM.yyyy,State
PowerMax 10,10000,50,15.03.2022,Idle
```

Допустимі значення: модель 3–20 символів (латиниця, цифри, пробіл); ємність 1000–50000 мА·год; заряд 0–100 %; дата від 01.01.2015 до сьогодні; стан `Off`, `Idle`, `Charging`, `Discharging`.

## Структура репозиторію

```
OOP_Lab4_Liashenko/
├── OOP_Lab4_Liashenko.slnx
├── OOP_Lab4_Liashenko/
│   ├── Powerbank.cs
│   ├── PowerbankState.cs
│   ├── Program.cs
```

| Файл | Призначення |
|---|---|
| `Powerbank.cs` | клас повербанка |
| `PowerbankState.cs` | перелік станів |
| `Program.cs` | консольне меню |
