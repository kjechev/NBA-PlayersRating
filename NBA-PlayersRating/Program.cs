using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NBA_PlayersRating
{
    class Program
    {
        static void Main(string[] args)
        {
            Input();

            Console.ReadKey(true);
        }

        static void Input()
        {
            Console.WriteLine("Enter the name of the JSON file:");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath + ".json"))
            {
                Console.Clear();
                Console.WriteLine("Some of the parameters are wrong. Please enter a valid information.");
                Input();
            }
            Console.WriteLine("Enter the maximum years of player to qualify:");
            string inputMaxYears = Console.ReadLine();
            int maxYears;

            if (!int.TryParse(inputMaxYears, out maxYears))
            {
                Console.Clear();
                Console.WriteLine("Some of the parameters are wrong. Please enter a valid information.");
                Input();
            }
        
            Console.WriteLine("Enter the minimum rating of a player to qualify:");
            string inputMinRating = Console.ReadLine();
            int minRating = 0;

            if (!int.TryParse(inputMinRating, out minRating))
            {
                Console.Clear();
                Console.WriteLine("Some of the parameters are wrong. Please enter a valid information.");
                Input();
            }
         
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            Console.WriteLine("Enter name of the export file:");
            string exportFilePath = Console.ReadLine();

            if (!regexItem.IsMatch(exportFilePath))
            {
                Console.Clear();
                Console.WriteLine("Some of the parameters are wrong. Please enter a valid information.");
                Input();
            }

            Deserializing(filePath, minRating, maxYears, exportFilePath);
        }

        static void FilterData(List<PlayerInfo> allPlayers, double minRating, int maxYears, string exportFilePath)
        {
            StringBuilder sb = new StringBuilder();
            int currentYear = DateTime.UtcNow.Year;

                List<PlayerInfo> selectedPlayers = allPlayers
                .Where(player => player.Rating >= minRating && player.PlayerSince >= currentYear - maxYears)
                .OrderByDescending(player => player.Rating)
                .ThenBy(player => player.Name)
                .ToList();

                sb.AppendLine("Name, Rating:");
                Console.WriteLine();

                selectedPlayers.ForEach(player =>
                    {
                        sb.AppendLine($"{player.Name}, {player.Rating}");
                    }
                );
            
            File.WriteAllText(exportFilePath + ".csv", sb.ToString());
        }

        static void Deserializing(string filePath, double minRating, int maxYears, string exportFilePath)
        {
            StreamReader reader = new StreamReader(filePath + ".json");
            string dataJSON = reader.ReadToEnd();

            List<PlayerInfo> allPlayers = JsonConvert.DeserializeObject<List<PlayerInfo>>(dataJSON);

            FilterData(allPlayers, minRating, maxYears, exportFilePath);
        }
    }
}
