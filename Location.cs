using System.Net.Http.Json;
using System.Text.RegularExpressions;

class LocationData
{
    public string? status { get; set; }
    public string? country { get; set; }
    public string? countryCode { get; set; }
    public string? region { get; set; }
    public string? regionName { get; set; }
    public string? city { get; set; }
    public string? zip { get; set; }
    public double lat { get; set; }
    public double lon { get; set; }
    public string? timezone { get; set; }
    public string? isp { get; set; }
    public string? org { get; set; }
    public string? @as { get; set; }
    public string? query { get; set; }
}

struct GeoPosition
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public GeoPosition(double latitude = default, double longitude = default)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }
}

static class Location
{
    public async static Task<GeoPosition> GetLocation(HttpClient httpClient)
    {
        // Get the IP address
        var IP = await GetIP(httpClient);
        Console.WriteLine("Getting location...");
        // Get the location data
        var locationData = await httpClient.GetFromJsonAsync<LocationData>($"http://ip-api.com/json/{IP}");
        // Return the location
        GeoPosition geoPosition = new GeoPosition();
        if (locationData != null)
        {
            geoPosition = new GeoPosition
            {
                latitude = locationData.lat,
                longitude = locationData.lon
            };
        }

        return geoPosition;
    }

    async static Task<string> GetIP(HttpClient httpClient)
    {
        // Pattern to find IPv4 addresses
        string pattern = @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?::(\d|[1-9]\d{1,3}|[1-5]\d{4}|6[0-4]\d{3}|65[0-4]\d{2}|655[0-2]\d|6553[0-5]))?";
        Console.WriteLine("Getting IP address...");
        // Get the IP address
        var IPData = await ((await httpClient.GetAsync("http://checkip.dyndns.org/"))
            .Content.ReadAsStringAsync());
        Console.WriteLine("Matching IP address...");
        // Match the IP address
        var match = Regex.Match(IPData, pattern);
        Console.WriteLine($"IPv4 address is {match.Value}");
        return match.Value;
    }
}
