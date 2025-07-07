using Bogus;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models.Entities;
using System.Text;
using System.Diagnostics;

var stopwatch = Stopwatch.StartNew();
string basePath = AppContext.BaseDirectory;
string projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
string envPath = Path.Combine(projectRoot, ".env");
Env.Load(envPath);

string host = Environment.GetEnvironmentVariable("POSTGRES_HOST")!;
string port = Environment.GetEnvironmentVariable("POSTGRES_PORT")!;
string user = Environment.GetEnvironmentVariable("POSTGRES_USER")!;
string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!;
string db = Environment.GetEnvironmentVariable("POSTGRES_DB")!;
string connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={db}";

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connectionString)
    .Options;

using var context = new AppDbContext(options);
context.ChangeTracker.AutoDetectChangesEnabled = false;

Random random = new();
Faker faker = new(locale: "ru");

HashSet<string> existingNumbers = context.Companies
    .Select(c => c.RepPhoneNumber)
    .ToHashSet();

int amount = 1_000_002;
int amountSameType = amount / 3;
int limitForSave = 1_000;
List<int> banksId = [];
List<Bank> banks = [];
List<Agent> agents = [];
List<Client> clients = [];

for (int i = 1; i < amountSameType + 1; i++) // Банки
{
    var company = GenerateCompany(i);
    banks.Add(new Bank { Company = company });

    if (i % limitForSave == 0)
    {
        Console.WriteLine($"Saving {i}");
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Hours}:{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");
        context.Banks.AddRange(banks);
        await context.SaveChangesAsync();
        banks.Clear();
    }
}
context.Banks.AddRange(banks);
await context.SaveChangesAsync();
// кэширование банков для оптимизации создание связей с агенатми
var bankMap = await context.Banks.ToDictionaryAsync(b => b.Id);
Console.WriteLine("Banks done");

for (int i = amountSameType + 1; i < 2 * amountSameType + 1; i++) // Агенты
{
    var company = GenerateCompany(i, 2);
    int countBanks = random.Next(1, 4);
    HashSet<int> agentBankIds = [];
    while (agentBankIds.Count < countBanks)
    {
        agentBankIds.Add(random.Next(1, amountSameType + 1));
    }

    agents.Add(new Agent
    {
        Company = company,
        Important = faker.Random.Bool(0.05f),
        Banks = agentBankIds.Select(id => bankMap[id]).ToList()
    });

    if (i % limitForSave == 0)
    {
        Console.WriteLine($"Saving {i}");
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Hours}:{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");
        context.Agents.AddRange(agents);
        await context.SaveChangesAsync();
        agents.Clear();
    }
}

context.Agents.AddRange(agents);
await context.SaveChangesAsync();
Console.WriteLine("Agents done");

for (int i = 2 * amountSameType + 1; i < 3 * amountSameType + 1; i++) // Клиенты
{
    var company = GenerateCompany(i, 3);
    clients.Add(new Client { Company = company });

    if (i % limitForSave == 0)
    {
        Console.WriteLine($"Saving {i}");
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Hours}:{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();
        clients.Clear();
    }
}

context.Clients.AddRange(clients);
await context.SaveChangesAsync();
Console.WriteLine("Clients done");
stopwatch.Stop();
Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.Hours}:{stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");

Company GenerateCompany(int i, int m = 1)
{
    var company = GenerateRepCompanyAndDate();
    company.Id = i;
    company.Inn = (Math.Pow(10, 9) * m + i).ToString();
    company.Ogrn = (Math.Pow(10, 12) * m + i).ToString();
    company.Kpp = GenerateDigitString(9);
    var companyNameAndEmail = GenerateCompanyNameAndEmail(company.RepName, i);
    company.ShortName = companyNameAndEmail[0];
    company.FullName = companyNameAndEmail[1];
    company.RepEmail = companyNameAndEmail[2];
    company.RepPhoneNumber = GenerateUniquePhone();
    return company;
}

Company GenerateRepCompanyAndDate()
{
    var gender = faker.Random.Bool() ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
    var companyData = new Faker<Company>(locale: "ru")
        .RuleFor(c => c.RepName, f => f.Name.FirstName(gender))
        .RuleFor(c => c.RepSurName, f => f.Name.LastName(gender))
        .RuleFor(c => c.RepPatronymic, f => faker.Random.Bool(0.9f) ? GeneratePatronymic(f, gender) : null)
        .RuleFor(c => c.RepEmail, f => f.Internet.Email())
        .RuleFor(c => c.OgrnDateOfAssignment, f => DateOnly.FromDateTime(f.Date.Between(new DateTime(1992, 1, 1), new DateTime(2024, 12, 31))));
    return companyData.Generate();
}

static string GeneratePatronymic(Faker f, Bogus.DataSets.Name.Gender gender)
{
    string[] malePatronymics = {
        "Александрович",
        "Алексеевич",
        "Анатольевич",
        "Андреевич",
        "Антонович",
        "Аркадьевич",
        "Артёмович",
        "Богданович",
        "Борисович",
        "Вадимович",
        "Валентинович",
        "Валерьевич",
        "Васильевич",
        "Викторович",
        "Витальевич",
        "Владимирович",
        "Владиславович",
        "Вячеславович",
        "Геннадьевич",
        "Георгиевич",
        "Глебович",
        "Григорьевич",
        "Данилович",
        "Денисович",
        "Дмитриевич",
        "Евгеньевич",
        "Егорович",
        "Иванович",
        "Игоревич",
        "Ильич",
        "Кириллович",
        "Константинович",
        "Леонидович",
        "Львович",
        "Максимович",
        "Маркович",
        "Матвеевич",
        "Михайлович",
        "Никитич",
        "Николаевич",
        "Олегович",
        "Павлович",
        "Петрович",
        "Романович",
        "Сергеевич",
        "Станиславович",
        "Степанович",
        "Тарасович",
        "Тимофеевич",
        "Фёдорович",
        "Юрьевич",
        "Ярославович"
    };
    string[] femalePatronymics = {
        "Александровна",
        "Алексеевна",
        "Анатольевна",
        "Андреевна",
        "Антоновна",
        "Аркадьевна",
        "Артёмовна",
        "Богдановна",
        "Борисовна",
        "Вадимовна",
        "Валентиновна",
        "Валерьевна",
        "Васильевна",
        "Викторовна",
        "Витальевна",
        "Владимировна",
        "Владиславовна",
        "Вячеславовна",
        "Геннадьевна",
        "Георгиевна",
        "Глебовна",
        "Григорьевна",
        "Даниловна",
        "Денисовна",
        "Дмитриевна",
        "Евгеньевна",
        "Егоровна",
        "Ивановна",
        "Игоревна",
        "Ильинична",
        "Кирилловна",
        "Константиновна",
        "Леонидовна",
        "Львовна",
        "Максимовна",
        "Марковна",
        "Матвеевна",
        "Михайловна",
        "Никитична",
        "Николаевна",
        "Олеговна",
        "Павловна",
        "Петровна",
        "Романовна",
        "Сергеевна",
        "Станиславовна",
        "Степановна",
        "Тарасовна",
        "Тимофеевна",
        "Фёдоровна",
        "Юрьевна",
        "Ярославовна"
    };
    return gender == Bogus.DataSets.Name.Gender.Male
        ? f.PickRandom(malePatronymics)
        : f.PickRandom(femalePatronymics);
}

string GenerateDigitString(int length)
{
    var random = new Random();
    var sb = new StringBuilder(length);
    for (int i = 0; i < length; i++)
    {
        sb.Append(random.Next(0, 10));
    }
    return sb.ToString();
}

string[] GenerateCompanyNameAndEmail(string RepName, int i)
{
    Dictionary<string, string> legalForms = new Dictionary<string, string>
    {
        {"ООО", "Общество с ограниченной ответственностью"},
        {"АО", "Акционерное общество"},
        {"ПАО", "Публичное акционерное общество"}
    };
    string[] legalFormsShort = { "ООО", "АО", "ПАО" };
    string[] companyNames = {
        "Аспект",
        "Вектор",
        "Контур",
        "Квант",
        "Орион",
        "Фактор",
        "Эон",
        "Парадигма",
        "Феникс",
        "Северный Берег",
        "СТР",
        "БК Групп",
        "РТС",
        "Восход",
        "Прилив",
        "Гроза",
        "Исток",
        "Равновесие",
        "Гармония",
        "Перспектива",
        "Фокус",
        "Динамика",
        "Атлант",
        "Пегас",
        "Альтаир",
        "Вега",
        "Арго",
        "Гранит",
        "Стальмар",
        "Терминал",
        "Интервал",
        "Новус",
        "Ультима",
        "Вертекс",
        "Оптима",
        "Квадрат"
    };
    int counterNames = (i / companyNames.Length) + 1;
    string companyName = $"{companyNames[i % companyNames.Length]} {counterNames}";
    string legalFormShort = faker.PickRandom(legalFormsShort);
    string companyShortName = $"{legalFormShort} \"{companyName}\"";
    string companyFullName = $"{legalForms[legalFormShort]} \"{companyName}\"";
    string email = $"{Transliterate(RepName)}@{Transliterate(companyName)}.ru";
    return [companyShortName, companyFullName, email];
}

static string Transliterate(string russianText)
{
    var translitMap = new Dictionary<string, string>
    {
        {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
        {"е", "e"}, {"ё", "yo"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"},
        {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
        {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
        {"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "c"}, {"ч", "ch"},
        {"ш", "sh"}, {"щ", "sсh"}, {"ъ", ""}, {"ы", "y"}, {"ь", ""},
        {"э", "e"}, {"ю", "yu"}, {"я", "ya"}, {" ", "-"}
    };

    var result = new StringBuilder();
    foreach (var c in russianText.ToLower())
    {
        result.Append(translitMap.TryGetValue(c.ToString(), out var value) ? value : c.ToString());
    }
    return result.ToString();
}

string GenerateUniquePhone()
{
    string phone;
    do
    {
        int regionCode = random.Next(900, 999);
        int part1 = random.Next(0, 999);
        int part2 = random.Next(0, 99);
        int part3 = random.Next(0, 99);

        phone = $"({regionCode}){part1:D3}-{part2:D2}-{part3:D2}";
    }
    while (existingNumbers.Contains(phone));
    existingNumbers.Add(phone);
    return phone;
}