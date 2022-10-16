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
        Console.WriteLine("Getting location...");
        // Get the location data
        var locationData = await httpClient.GetFromJsonAsync<LocationData>($"http://ip-api.com/json/");
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
}
