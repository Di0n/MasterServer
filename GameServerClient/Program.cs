using Microsoft.AspNetCore.SignalR.Client;

namespace GameServerClient
{
    internal class Program
    {
        static readonly Random random = new Random();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            ServerClient client = new ServerClient("https://localhost:7284");
            Console.Write("Server name: ");
            string name = Console.ReadLine();
            Console.Write("Host: ");
            string host = Console.ReadLine();
            Console.Write("Port: ");
            uint port = uint.Parse(Console.ReadLine());

            await client.RegisterServerAsync(name, host, port);


            string[] maps = new string[] { "de_dust2", "de_inferno", "de_nuke", "de_mirage" };
            // Periodically update status (e.g., every 30 seconds)
            int maxSlots = 32;
            while (true)
            {
                string mp = maps[random.Next(0, maps.Length)];
                int playerCount = random.Next(0, 33);


                    await client.UpdateStatusAsync(
                        map: mp,
                        currentPlayers: playerCount,
                        maxPlayers: maxSlots
                    );


                Console.WriteLine($"{mp}\n{playerCount}//{maxSlots}");
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }
    }
}
