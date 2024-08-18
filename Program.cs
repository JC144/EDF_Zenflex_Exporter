using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EDF_Zenflex_Exporter;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var startDate = new DateTime(2024, 01, 01);
            var endDate = new DateTime(2024, 04, 15);

            await DisplayZenFlexDaysForPeriodAsync(startDate, endDate);
            //await CreateJsonForPeriodAsync(startDate, endDate);
        }

        static async Task DisplayZenFlexDaysForPeriodAsync(DateTime startDate, DateTime endDate)
        {
            int zenFlexCount = 0;

            using (ApiCaller api = new ApiCaller())
            {
                while (startDate <= endDate)
                {
                    var dayType = await api.GetDayTypeAsync(startDate.ToString("yyyy-MM-dd"));

                    if (dayType == "ZENF_PM")
                    {
                        Console.WriteLine($"\"{startDate.ToString("yyyy/MM/dd")}\",");
                        zenFlexCount++;
                    }
                    startDate = startDate.AddDays(1);
                }
            }

            if (zenFlexCount == 20)
            {
                Console.WriteLine("Success");
            }
            else if (zenFlexCount < 20)
            {
                Console.WriteLine($"Missing {20 - zenFlexCount} days.");
            }
            else if (zenFlexCount > 20)
            {
                Console.WriteLine($"{zenFlexCount} days found.");
            }
        }

        static async Task CreateJsonForPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var jsonArray = new JArray();
            string beginYear = startDate.ToString("yyyy");

            using (ApiCaller api = new ApiCaller())
            {
                while (startDate <= endDate)
                {
                    Console.WriteLine("Processing date: " + startDate.ToString("yyyy-MM-dd"));

                    var dayType = await api.GetDayTypeAsync(startDate.ToString("yyyy-MM-dd"));
                    if (!String.IsNullOrEmpty(dayType))
                    {
                        JObject jsonEntry = new JObject();
                        jsonEntry["Date"] = startDate.ToString("yyyy-MM-dd");
                        jsonEntry["CouleurJourJ"] = dayType;
                        jsonArray.Add(jsonEntry);
                    }
                    startDate = startDate.AddDays(1);
                }
            }

            string jsonString = jsonArray.ToString(Formatting.Indented);
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{beginYear}_ZenflexCalendar.json");
            File.WriteAllText(outputPath, jsonString);
            Console.WriteLine("JSON file created at: " + outputPath);
        }
    }
}