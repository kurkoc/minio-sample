namespace MinioSample.Models;

public class MinioGroup
{
    public string Status { get; set; }
    public string GroupName { get; set; }
    public string GroupStatus { get; set; }
    public string GroupPolicy { get; set; }
    
    public List<string> GroupPolicies => string.IsNullOrEmpty(GroupPolicy) ? [] : GroupPolicy.Split(',').ToList();
    public List<string> Members { get; set; }

}
