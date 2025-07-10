namespace MinioSample.Models;

public class MinioUser
{
    public string Status { get; set; }
    public string AccessKey { get; set; }
    public string UserStatus { get; set; }
    public string PolicyName { get; set; }
    public List<MinioGroupMembership> MemberOf { get; set; } = new();
}


public class MinioGroupMembership
{
    public string Name { get; set; } = "";
}
