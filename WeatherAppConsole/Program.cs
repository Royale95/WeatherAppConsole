using System;
using System.IO;
using System.Linq;
using TempData.Models;

namespace TempData
{
    class Program
    {
        static void Main(string[] args)
        {
            StartUpMenu();
        }

        private static void StartUpMenu()
        {
            Console.Clear();
            Console.WriteLine("***************Choose an alternative*************** \n\n" +
                "Press [1] to insert data to database\n" +
                "Press [2] to print average temperature\n" +
                "Press [3] to sort by hottest to coldest day\n" +
                "Press [4] to sort by driest to most humid day\n" +
                "Press [5] to sort by highest to lowest risk of mold\n" +
                "Press [6] to view meteorological autumn\n" +
                "Press [7] to view meteorological winter\n" +
                "Press [ESC] to exit");
            ConsoleKeyInfo consoleKey = Console.ReadKey(true);
            switch (consoleKey.Key)
            {
                case ConsoleKey.D1:
                    AddDataToDB();
                    break;
                case ConsoleKey.D2:
                    AverageTemperatures(new DateTime());
                    break;
                case ConsoleKey.D3:
                    HighToLowTemperatures();
                    break;
                case ConsoleKey.D4:
                    DriestToMostHumidDays();
                    break;
                case ConsoleKey.D5:
                    MoldRisk();
                    break;
                case ConsoleKey.D6:
                    MeteorologicalAutumn(new DateTime());
                    break;
                case ConsoleKey.D7:
                    MeteorologicalWinter(new DateTime());
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(1);
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Something went wrong. Press any key to try again..");
                    Console.ReadKey();
                    StartUpMenu();
                    break;
            }
        }

        private static void AddDataToDB()
        {
            const string csvFile = @"Data\tempdata4.csv";

            var items = from t in File.ReadLines(csvFile)
                        let columns = t.Split(',')
                        select new Temperature
                        {
                            DateTime = DateTime.Parse(columns[0]),
                            Location = columns[1],                                                 // Connects all properties to it's counterpart in the csv file
                            Temperatures = double.Parse(columns[2].Replace(".", ",")),
                            Humidity = double.Parse(columns[3])
                        };

            using (var db = new EFContext())
            {
                if (db.Temperatures.Count() > 0) return;

                items.Take(1).ToList().ForEach(t => db.Temperatures.AddRange(items));             // Takes all data from items and puts it in the database 

                db.SaveChanges();
            }
        }

        private static void AverageTemperatures(DateTime date)
        {
            using (var db = new EFContext())
            {
                Console.Clear();

                while (true)
                {
                    Console.WriteLine("Enter a date [YYYY-MM-DD]:");
                    DateTime userInput;
                    if (DateTime.TryParse(Console.ReadLine(), out userInput))
                    {
                        date = userInput;
                        break;
                    }
                }

                Console.Clear();
                Console.WriteLine("Choose an alternitive:\n[I] for inside \n[O] for outside");


                ConsoleKeyInfo consoleKey = Console.ReadKey(true);

                try
                {
                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.I:
                            var avgTempIn = db.Temperatures
                                         .Where(x => x.DateTime.Date == date.Date && x.Location == "Inne")
                                         .Average(x => x.Temperatures);
                            Console.Clear();
                            Console.WriteLine($"Average temperature inside for {date:yyyy-MM-dd} is {Math.Round(avgTempIn)} degrees");

                            break;
                        case ConsoleKey.O:
                            var avgTempOut = db.Temperatures
                                            .Where(x => x.DateTime.Date == date.Date && x.Location == "Ute")
                                            .Average(x => x.Temperatures);
                            Console.Clear();
                            Console.WriteLine($"Average temperature outside for {date:yyyy-MM-dd} is {Math.Round(avgTempOut)} degrees");


                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Something went wrong. Press any key to try again..");
                            Console.ReadKey();
                            AverageTemperatures(date);
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.Clear();
                    Console.WriteLine("The date does not exist in database, enter a valid date... \nPress any key to return to main menu");
                    Console.ReadKey();
                    StartUpMenu();
                }

                Console.Write("\nPress any key to return to main menu");
                Console.ReadKey();
                StartUpMenu();
            }
        }

        private static void HighToLowTemperatures()
        {
            using (var db = new EFContext())
            {
                Console.Clear();
                Console.WriteLine("Choose an alternitive:\n[I] for inside \n[O] for outside");
                ConsoleKeyInfo consoleKey = Console.ReadKey(true);
                switch (consoleKey.Key)
                {
                    case ConsoleKey.I:

                        var highestToLowestTempIn = db.Temperatures.Where(l => l.Location == "Inne")
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageTemp = da.Average(c => c.Temperatures) }).ToList()
                        .OrderByDescending(x => x.AverageTemp);

                        Console.Clear();
                        foreach (var item in highestToLowestTempIn)
                        {
                            Console.WriteLine($"{item.Date:yyyy-MM-dd} -> {Math.Round(item.AverageTemp)} degrees");
                        }

                        break;
                    case ConsoleKey.O:

                        var highestToLowestTempOut = db.Temperatures.Where(l => l.Location == "Ute")
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageTemp = da.Average(c => c.Temperatures) }).ToList()
                        .OrderByDescending(x => x.AverageTemp);

                        Console.Clear();
                        foreach (var item in highestToLowestTempOut)
                        {
                            Console.WriteLine($"{item.Date:yyyy - MM - dd} -> { Math.Round(item.AverageTemp)} degrees");
                        }

                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Something went wrong. Press any key to try again..");
                        Console.ReadKey();
                        HighToLowTemperatures();
                        break;
                }
                Console.Write("\nPress any key to return to main menu");
                Console.ReadKey();
                StartUpMenu();
            }
        }
        private static void MoldRisk()
        {
            using (var db = new EFContext())
            {
                Console.Clear();
                Console.WriteLine("Choose an alternative: \n\n" +
                    "Press [I] to sort by moldrisk indoors\n" +
                    "Press [O] to sort by moldrisk outdoors");
                ConsoleKeyInfo consoleKey = Console.ReadKey(true);
                Console.Clear();
                switch (consoleKey.Key)
                {
                    case ConsoleKey.I:
                        var moldRiskInside = db.Temperatures.Where(l => l.Location == "Inne")
                    .GroupBy(d => d.DateTime.Date)
                    .Select(n => new
                    {
                        Date = n.Key,
                        AverageTemp = n.Average(c => c.Temperatures),
                        AverageHum = n.Average(c => c.Humidity)
                    }).ToList().OrderByDescending(x => ((x.AverageHum - 78) * (x.AverageTemp / 15)) / 0.22);

                        foreach (var item in moldRiskInside)
                        {
                            double temp = item.AverageTemp;
                            double hum = item.AverageHum;
                            double moldRisk = ((hum - 78) * (temp / 15)) / 0.22;

                            if (moldRisk < 0)
                            {
                                moldRisk = 0;
                            }
                            else if (moldRisk > 100)
                            {
                                moldRisk = 100;
                            }
                            Console.WriteLine($"{item.Date:yyyy-MM-dd} = {Math.Round((decimal)moldRisk)}%");
                        }
                        
                        break;
                    case ConsoleKey.O:
                        var moldRiskOut = db.Temperatures.Where(l => l.Location == "Ute")
                    .GroupBy(d => d.DateTime.Date)
                    .Select(n => new
                    {
                        Date = n.Key,
                        AverageTemp = n.Average(c => c.Temperatures),
                        AverageHum = n.Average(c => c.Humidity),
                    }).ToList().OrderByDescending(x => ((x.AverageHum - 78) * (x.AverageTemp / 15)) / 0.22);

                        foreach (var item in moldRiskOut)
                        {
                            double temp = item.AverageTemp;
                            double hum = item.AverageHum;
                            double moldrisk = ((hum - 78) * (temp / 15)) / 0.22;

                            if (moldrisk < 0)
                            {
                                moldrisk = 0;
                            }
                            else if (moldrisk > 100)
                            {
                                moldrisk = 100;
                            }
                            Console.WriteLine($"{item.Date:yyyy-MM-dd} = {Math.Round((decimal)moldrisk)}%");
                        }

                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid key. Press any key to try again..");
                        Console.ReadKey();
                        MoldRisk();
                        break;
                }
                Console.Write("\nPress any key to return to main menu");
                Console.ReadKey();
                StartUpMenu();
            }
        }
        private static void DriestToMostHumidDays()
        {
            using (var db = new EFContext())
            {
                Console.Clear();
                Console.WriteLine("For inside press [I] \nFor outside press [O]");
                ConsoleKeyInfo consoleKey = Console.ReadKey(true);
                switch (consoleKey.Key)
                {
                    case ConsoleKey.I:

                        var highestToLowestHumIn = db.Temperatures.Where(l => l.Location == "Inne")
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageHum = da.Average(h => h.Humidity) }).ToList()
                        .OrderBy(x => x.AverageHum);

                        Console.Clear();
                        foreach (var item in highestToLowestHumIn)
                        {
                            Console.WriteLine($"{item.Date:yyyy-MM-dd} -> {Math.Round(item.AverageHum)}%");
                        }

                        break;
                    case ConsoleKey.O:

                        var highestToLowestHumOut = db.Temperatures.Where(l => l.Location == "Ute")
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageHum = da.Average(h => h.Humidity) }).ToList()
                        .OrderBy(x => x.AverageHum);

                        Console.Clear();
                        foreach (var item in highestToLowestHumOut)
                        {
                            Console.WriteLine($"{item.Date:yyyy-MM-dd} -> {Math.Round(item.AverageHum)}%");
                        }

                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid key. Press any key to try again..");
                        Console.ReadKey();
                        DriestToMostHumidDays();
                        break;
                }
                Console.Write("\nPress any key to return to main menu..");
                Console.ReadKey();
                StartUpMenu();
            }
        }
        private static void MeteorologicalAutumn(DateTime date)
        {
            using (var db = new EFContext())
            {
                var autumn = db.Temperatures.Where(l => l.Location == "Ute" && l.DateTime > DateTime.Parse("2016-06-11"))
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageTemp = da.Average(c => c.Temperatures) }).ToList()
                        .OrderBy(x => x.Date);

                Console.Clear();

                int dayCount = 0;
                foreach (var dates in autumn)
                {
                    if (dates.AverageTemp <= 10)
                    {
                        if (dayCount == 0)
                        {
                            date = dates.Date;
                        }
                        else if (dayCount == 5)
                        {
                            Console.WriteLine($"Meteorological autumn starts at {date:yyyy-MM-dd}");
                            break;
                        }
                        dayCount++;
                    }
                }
                Console.Write("\nPress any key to return to main menu");
                Console.ReadKey();
                StartUpMenu();
            }
        }

        private static void MeteorologicalWinter(DateTime date)
        {
            using (var db = new EFContext())
            {
                var winter = db.Temperatures.Where(l => l.Location == "Ute")
                        .GroupBy(d => d.DateTime.Date)
                        .Select(da => new { Date = da.Key, AverageTemp = da.Average(c => c.Temperatures) }).ToList()
                        .OrderBy(x => x.Date);

                Console.Clear();

                int dayCount = 0;
                foreach (var dates in winter)
                {
                    if (dates.AverageTemp <= 0)
                    {
                        if (dayCount == 0)
                        {
                            date = dates.Date;
                        }
                        else if (dayCount == 5)
                        {
                            Console.WriteLine($"Meteorological winter starts at {date:yyyy-MM-dd}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("No meteorological winter found");
                            break;
                        }
                        dayCount++;
                    }
                }
                Console.Write("\nPress any key to return to main menu");
                Console.ReadKey();
                StartUpMenu();
            }
        }


    }
}


