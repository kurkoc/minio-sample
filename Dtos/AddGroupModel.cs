namespace MinioSample.Dtos;

public record AddGroupModel
{
    public string GroupName { get; set; }

    public List<string> Users { get; set; }
}