namespace DroneDeliveryServiceCC;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            // Read input from a file
            var input = ReadInputFile("input.txt");
            // Parse drones from the first line of the input
            var drones = ParseDrones(input.First());
            // Parse locations from the rest of the input
            var locations = ParseLocations(input.Skip(1));
            // Assign locations to drones
            AssignLocationsToDrones(drones, locations);
            // Display results
            DisplayTrips(drones);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    /// <summary>
    /// Reading file from source path
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static List<string> ReadInputFile(string filePath)
    {
        return File.ReadAllLines(filePath).ToList();
    }
    /// <summary>
    /// Parse drone line to return the list of drones
    /// </summary>
    /// <param name="droneLine"></param>
    /// <returns></returns>
    private static List<Drone> ParseDrones(string droneLine)
    {
        return droneLine
            .Split(',')
            .Select((s, i) => new { s, i })
            .GroupBy(x => x.i / 2, x => x.s.Trim())
            .Select(g => new Drone(g.First(), int.Parse(g.Last().Trim().Substring(1, g.Last().Length-2))))
            .ToList();
    }
    /// <summary>
    /// Parse locations lines to return list of locations
    /// </summary>
    /// <param name="locationLines"></param>
    /// <returns></returns>
    private static List<Location> ParseLocations(IEnumerable<string> locationLines)
    {
        return locationLines
            .Select(l => l.Split(',').Select(s => s.Trim()))
            .Select(l => new Location(l.First(), int.Parse(l.Last().Replace("[", "").Replace("]", "").Trim())))
            .ToList();
    }
    /// <summary>
    /// Assign locations to drones to get the trips to each drone
    /// </summary>
    /// <param name="drones"></param>
    /// <param name="locations"></param>
    private static void AssignLocationsToDrones(List<Drone> drones, List<Location> locations)
    {
        drones = drones.OrderByDescending(d => d.MaximumWeight).ToList();

        while (locations.Any())
        {
            foreach (var drone in drones)
            {
                var availableLocations = locations.Where(l => l.PackageWeight <= drone.MaximumWeight).ToList();

                if (!availableLocations.Any()) continue;

                var selectedLocations = new List<Location>();
                var remainingCapacity = drone.MaximumWeight;

                foreach (var location in availableLocations.Where(location => location.PackageWeight <= remainingCapacity))
                {
                    selectedLocations.Add(location);
                    remainingCapacity -= location.PackageWeight;
                }

                selectedLocations.ForEach(l => locations.Remove(l));
                drone.Trips.Add(selectedLocations);
            }
        }
    }
    /// <summary>
    /// Display the trips by console
    /// </summary>
    /// <param name="drones"></param>
    private static void DisplayTrips(List<Drone> drones)
    {
        using var writer = new StreamWriter("Output.txt"); // Create the output file
        foreach (var drone in drones)
        {
            // Display the selected trips for this drone
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{drone.Name}");
            writer.WriteLine($"{drone.Name}"); // Write drone name
            for (var i = 0; i < drone.Trips.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Trip #{i + 1}");
                writer.WriteLine($"Trip #{i + 1}"); // write trip number
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{string.Join(",", drone.Trips[i].Select(x => x.Name))}");
                writer.WriteLine($"{string.Join(",", drone.Trips[i].Select(x => x.Name))}"); // write locations 
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
    }
}

public class Drone
{
    public Drone(string name, int maximumWeight)
    {
        Name = name;
        MaximumWeight = maximumWeight;
        Trips = new List<List<Location>>();
    }

    public string Name { get; set; }
    public int MaximumWeight { get; set; }
    public List<List<Location>> Trips { get; set; }
}

public class Location
{
    public Location(string name, int packageWeight)
    {
        Name = name;
        PackageWeight = packageWeight;
    }

    public string Name { get; set; }
    public int PackageWeight { get; set; }
}