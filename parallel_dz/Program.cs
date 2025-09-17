using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;

namespace parallel_dz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose files : \n1 - .txt\n2 - SQLite");
            string choice = Console.ReadLine();

            List<User> users = new();

            if (choice == "1")
            {
                Console.WriteLine("Write path to file");
                string path = Console.ReadLine();

                if (!File.Exists(path))
                {
                    Console.WriteLine("No Such Directory");
                    return;
                }

                foreach (var line in File.ReadAllLines(path))
                {
                    var parts = line.Split('-');
                    if (parts.Length == 2)
                    {
                        users.Add(new User { FullName = parts[0].Trim(), Username = parts[1].Trim() });
                    }
                }
            }

            else if (choice == "2")
            {
                Console.WriteLine("Write path to SQLite DataBase: ");
                string dbpath = Console.ReadLine();

                if (!File.Exists(dbpath))
                {
                    Console.WriteLine("No Such Directory");
                    return;
                }

                using var connection = new SqliteConnection($"Data Source = {dbpath}");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT FullName , Username FROM Users";
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User { FullName = reader.GetString(0), Username = reader.GetString(1) });
                }

            }

            else
            {
                Console.WriteLine("Wrong choose");
                return;
            }

            Console.WriteLine("Write main word  for search: ");
            string word = Console.ReadLine();

            var sw = Stopwatch.StartNew();
            var linqResult = users.Where(u => u.FullName.Contains(word) || u.Username.Contains(word)).ToList();

            sw.Stop();

            Console.WriteLine($"LINQ find {linqResult} for {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            var plinqResults = users
                .AsParallel()
                .Where(u => u.FullName.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                            u.Username.Contains(word, StringComparison.OrdinalIgnoreCase))
                .ToList();
            sw.Stop();
            Console.WriteLine($"Parallel LINQ: find {plinqResults.Count} for {sw.ElapsedMilliseconds} ms\n");

            Console.WriteLine("Results of search (LINQ):");
            foreach (var user in linqResult)
                Console.WriteLine($"{user.FullName} - {user.Username}");

            Console.WriteLine("\nresults of search (Parallel LINQ):");
            foreach (var user in plinqResults)
                Console.WriteLine($"{user.FullName} - {user.Username}");
        }
    }
}
