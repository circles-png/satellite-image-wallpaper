// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Geometry
{
    public string? type { get; set; }
    public List<List<List<double>>>? coordinates { get; set; }
}

public class Result
{
    public int id { get; set; }
    public string? uuid { get; set; }
    public string? identifier { get; set; }
    public DateTime ingestion_date { get; set; }
    public DateTime begin_position_date { get; set; }
    public DateTime end_position_date { get; set; }
    public string? download_url { get; set; }
    public string? size { get; set; }
    public Geometry? geometry { get; set; }
    public string? satellite { get; set; }
    public string? scene_id { get; set; }
    public string? cloud_cover_percentage { get; set; }
    public string? product_type { get; set; }
}

public class Results
{
    public int page { get; set; }
    public int total_pages { get; set; }
    public string? next { get; set; }
    public object? previous { get; set; }
    public List<Result>? results { get; set; }
}
