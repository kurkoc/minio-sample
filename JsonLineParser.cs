using System.Text.Json;

namespace MinioSample;


public static class JsonLinesParser
{
    public static List<T> ParseMany<T>(string jsonLines) where T : class
    {
        var results = new List<T>();

        if (string.IsNullOrWhiteSpace(jsonLines))
            return results;

        // Her satırı ayrı ayrı parse et
        var lines = jsonLines.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            try
            {
                var obj = JsonSerializer.Deserialize<T>(trimmedLine, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (obj != null)
                    results.Add(obj);
            }
            catch (JsonException ex)
            {
                // Log error but continue parsing other lines
                Console.WriteLine($"Failed to parse line: {trimmedLine}. Error: {ex.Message}");
            }
        }

        return results;
    }

    public static T? Parse<T>(string jsonLines) where T : class
    {
        if (string.IsNullOrWhiteSpace(jsonLines))
            return null;

        var lines = jsonLines.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                            .Where(line => !string.IsNullOrWhiteSpace(line.Trim()))
                            .ToList();



        if (lines.Count > 1) throw new Exception("ParseMany kullan");

        if (lines.Count == 0) return null;

        return JsonSerializer.Deserialize<T>(lines[0].Trim(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
