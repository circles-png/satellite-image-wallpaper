using System.Globalization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

// Define an API key
var key = Environment.GetEnvironmentVariable
    ("SPECTATOR_KEY", EnvironmentVariableTarget.User);
if (key == null) throw new
    ("SPECTATOR_KEY environment variable not set");
// Make a client to send requests to the API
var httpClient = new HttpClient();
// Make a random number generator (seeded with the current time's hash code)
var random = new Random(DateTime.Now.ToString().GetHashCode());

// Get the current location
var location = await Location.GetLocation(httpClient);
// Offset for the bounding box of the image
var offset = 10;

// Request image data from the API
Console.WriteLine($"Making API request... (bounding square width is {offset * 2} degree(s), centered on ({location.latitude}, {location.longitude}))");
var data = await httpClient.GetFromJsonAsync<Results>
    ($"https://api.spectator.earth/imagery/?api_key={key}&bbox={location.longitude - offset},{location.latitude - offset},{location.longitude + offset},{location.latitude + offset}");

// If there is no data, exit
if (data == null) return;
// If there are no results, exit
if (data.results == null) return;

Console.WriteLine($"Picking random result... (from {data.results.Count} result(s))");
// Get a random result
var randomResult = data.results[random.Next(data.results.Count)];
// Define a URL to send requests to
var downloadUrl = randomResult.download_url ?? "";
Console.WriteLine($"Downloading list of images from result...");
Console.WriteLine($"(cloud coverage: {randomResult.cloud_cover_percentage})");
Console.WriteLine($"(date: {randomResult.ingestion_date})");
// Download the images
var images = await httpClient.GetFromJsonAsync<List<Image>>($"{downloadUrl}?api_key={key}");

// If there are no images, exit
if (images == null) return;

images = images.FindAll(
    (Image image) =>
        image.path != null
        // && image.path.EndsWith("jp2")
        // && !(image.path.Contains("DETFOO") || image.path.Contains("QUALIT"))
        && image.path == "preview.jp2");
Console.WriteLine($"Picking random image... (from {images.Count} image(s))");
// Get a random image
var randomImage = images[random.Next(images.Count)];

Console.WriteLine("Computing image size...");
// Convet the image size (in bytes) to a human-readable format
string[] sizes = { "B", "KB", "MB", "GB", "TB" };
int order = 0;
while (randomImage.size >= 1024 && order < sizes.Length - 1)
{
    order++;
    randomImage.size = randomImage.size / 1024;
}
string size = String.Format("{0:0.##} {1}", randomImage.size, sizes[order]);

Console.WriteLine($"Downloading image \"{randomImage.name}\"... (size: {size})");
// Download the image
HttpContent content = (await httpClient.GetAsync
    ($"{downloadUrl}{randomImage.path ?? ""}/?api_key={key}")).Content;

// Create a new directory if it doesn't already exist
Directory.CreateDirectory("images");

Console.WriteLine("Writing image to file...");
// Using an output stream and an input stream,
// write the image to a file
using (Stream output = File.OpenWrite($"images/{randomImage.name}"))
using (Stream input = await content.ReadAsStreamAsync())
{
    await input.CopyToAsync(output);
}

Console.WriteLine("Converting image to PNG...");
// Open the image
using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load($"images/{randomImage.name}"))
{
    // If there is no path, exit
    if (randomImage.name == null) return;
    // Save the image as a PNG
    image.Save($"images/{Regex.Replace(randomImage.name, ".jp2", ".png")}");
}

Console.WriteLine("Cleaning up...");
File.Delete($"images/{randomImage.name}");

Console.WriteLine("Done!");
