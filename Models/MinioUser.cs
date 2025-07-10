using System.Text.Json.Serialization;

namespace MinioSample.Models;

public class MinioUser
{
    public string Status { get; set; }
    public string AccessKey { get; set; }
    public string UserStatus { get; set; }
    public string PolicyName { get; set; }
    public List<string> Policies => string.IsNullOrEmpty(PolicyName) ? [] : PolicyName.Split(',').ToList();
    public List<MinioGroupMembership> MemberOf { get; set; } = [];
}


public class MinioGroupMembership
{
    public string Name { get; set; }

    public List<string> Policies { get; set; } = [];
}
