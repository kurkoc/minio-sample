using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using MinioSample.Dtos;
using MinioSample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint("localhost:9000")
    .WithCredentials("vCndFcDrSWPuhazfAZr7", "gW5SJYEbHkHvwOzNoxymfzx7Xv7cXTBmIEmM9jM1")
    .WithSSL(false)
    .Build());

builder.Services.AddScoped<IMinioAdminService, MinioAdminService>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("buckets", async (IMinioClient minioClient) =>
{
    var buckets = await minioClient.ListBucketsAsync();
    return Results.Ok(buckets);
});

app.MapPost("upload", async ([FromServices] IMinioClient minioClient, 
            [FromForm] IFormFile file, 
            [FromForm] string bucket,
            [FromForm] string? path) =>
    {
        string fileName = file.FileName;
        string contentType = file.ContentType;
        long length = file.Length;

        path = string.IsNullOrEmpty(path) ? "" : path.TrimStart('/').TrimEnd('/');
        string filePath = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";

        using var stream = file.OpenReadStream();

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(filePath)
            .WithStreamData(stream)
            .WithObjectSize(length)
            .WithContentType(contentType);

        var response = await minioClient.PutObjectAsync(putObjectArgs);
        return Results.Ok(response);

    })
    .DisableAntiforgery();

app.MapGet("download", async ([FromServices] IMinioClient minioClient) => 
{
    string fileName = "download.pdf";

    MemoryStream memoryStream = new();

    var args = new GetObjectArgs()
        .WithBucket("tikas")
        .WithObject($"documents/{fileName}")
        .WithCallbackStream(async stream => await stream.CopyToAsync(memoryStream));

    var stat = await minioClient.GetObjectAsync(args);

    memoryStream.Seek(0, SeekOrigin.Begin);

    return Results.File(memoryStream,stat.ContentType, fileName);
});

app.MapGet("users", async ([FromServices] IMinioAdminService minioAdminService) =>
{
    var groups = await minioAdminService.GetUsers();
    return groups.IsFailed ? Results.BadRequest(groups.Errors[0].Message) : Results.Ok(groups.Value);
});


app.MapPost("users", async ([FromServices] IMinioClient minioClient, [FromServices] IMinioAdminService minioAdminService, [FromBody] AddUserModel addUserModel) => 
{
    //create user on database

    //create user on minio
    await minioAdminService.AddUser(addUserModel.Username, addUserModel.Password);

    //add user to appusers group
    await minioAdminService.AddUsersToGroup("appusers", [addUserModel.Username]);
    
    //make default bucket for user
    await minioAdminService.MakeBucket(addUserModel.BucketName);
    
    //create default policy for user
    await minioAdminService.CreateDefaultPolicyAndAttachUser(addUserModel.Username, addUserModel.BucketName);;
    
    return Results.Ok($"MinIO user {addUserModel.Username} created successfully.");
});

app.MapGet("groups", async ([FromServices] IMinioAdminService minioAdminService) => 
{
    var groups = await minioAdminService.GetGroups();
    return groups.IsFailed ? Results.BadRequest(groups.Errors[0].Message) : Results.Ok(groups.Value);
});

app.MapGet("groups/{groupName}", async ([FromRoute] string groupName,[FromServices] IMinioAdminService minioAdminService) => 
{
    var group = await minioAdminService.GetGroup(groupName);
    return group.IsFailed ? Results.BadRequest(group.Errors[0].Message) : Results.Ok(group.Value);
});

app.MapPost("groups", async ([FromServices] IMinioAdminService minioAdminService, [FromBody] AddGroupModel addGroupModel) =>
{
    var groups = await minioAdminService.AddUsersToGroup(addGroupModel.GroupName, addGroupModel.Users);
    
    if (groups.IsFailed) return Results.BadRequest(groups.Errors[0].Message);

    return Results.Ok();

});

app.Run();