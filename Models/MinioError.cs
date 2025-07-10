namespace MinioSample.Models;


using System.Text.Json.Serialization;

public class MinioError
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("error")]
    public ErrorDetails Error { get; set; }
}

public class ErrorDetails
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("cause")]
    public ErrorCause Cause { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class ErrorCause
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("error")]
    public RawError Error { get; set; }
}

public class RawError
{
    [JsonPropertyName("Code")]
    public string Code { get; set; }

    [JsonPropertyName("Message")]
    public string Message { get; set; }

    [JsonPropertyName("BucketName")]
    public string BucketName { get; set; }

    [JsonPropertyName("Key")]
    public string Key { get; set; }

    [JsonPropertyName("RequestID")]
    public string RequestID { get; set; }

    [JsonPropertyName("HostID")]
    public string HostID { get; set; }

    [JsonPropertyName("Region")]
    public string Region { get; set; }
}