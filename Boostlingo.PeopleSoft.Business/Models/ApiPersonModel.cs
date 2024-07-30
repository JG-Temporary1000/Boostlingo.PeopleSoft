namespace Boostlingo.PeopleSoft.Business.Models;
using System.Text.Json.Serialization;

public class ApiPersonModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("language")]
    public required string Language { get; set; }
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("bio")]
    public required string Bio { get; set; }
    [JsonPropertyName("version")]
    public required double Version { get; set; }
}
