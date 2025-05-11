using System.Diagnostics;
using FluentResults;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using MinioSample.Dtos;

namespace MinioSample.Services;

public class MinioAdminService : IMinioAdminService
{
    private readonly string _mcPath;
    private readonly string _alias;
    private readonly IMinioClient _minioClient;

    public MinioAdminService(IMinioClient minioClient, string mcPath = "/opt/homebrew/bin/mc", string alias = "local")
    {
        _minioClient = minioClient;
        _mcPath = mcPath;
        _alias = alias;
    }
    
    public async Task<Result> MakeBucket(string name)
    {
        try
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(name);

            await _minioClient.MakeBucketAsync(makeBucketArgs);
            return Result.Ok();
        }
        catch (InvalidBucketNameException)
        {
            return Result.Fail("Bucket adı, geçerli bir ad değil");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
    public async Task<Result<List<string>>> GetUsers()
    {
        string args = $"admin user ls {_alias}";
        var result = await RunProcessAsync(args);
        
        if (result.IsFailed) return Result.Fail(result.Errors[0].Message);
        
        var users = result.Value.Split("\n").Where(q=>!string.IsNullOrEmpty(q)).ToList();
        return Result.Ok(users);
    }
    public async Task<Result> AddUser(string username, string password)
    {
        string args = $"admin user add {_alias} {username} {password}";
        var result = await RunProcessAsync(args);
        return Helpers.ResultHelper.ConvertToResult(result);
    }
    public async Task<Result> AddUsersToGroup(string groupName, List<string> users)
    {
        string userLine = string.Join(" ", users);
        string args = $"admin group add {_alias} {groupName} {userLine}";
        var result = await RunProcessAsync(args);
        return Helpers.ResultHelper.ConvertToResult(result);
    }
    public async Task<Result> AttachPolicyToUser(string policy, string user)
    {
        string args = $"admin policy attach {_alias} {policy} --user {user}";
        var result = await RunProcessAsync(args);
        return Helpers.ResultHelper.ConvertToResult(result);
    }
    public async Task<Result<List<string>>> GetGroups()
    {
        string args = $"admin group ls {_alias}";
        var result = await RunProcessAsync(args);
        
        if (result.IsFailed) return Result.Fail(result.Errors[0].Message);
        
        var groups = result.Value.Split("\n").Where(q=>!string.IsNullOrEmpty(q)).ToList();
        return Result.Ok(groups);
    }
    public async Task<Result<GroupInfoDto>> GetGroup(string groupName)
    {
        string args = $"admin group info {_alias} {groupName}";
        var result =  await RunProcessAsync(args);
        
        if (result.IsFailed) return Result.Fail(result.Errors[0].Message);
        
        var groupInfo = result.Value.Split("\n").ToList();
        return Result.Ok(new GroupInfoDto()
        {
            Name = groupInfo[0].Trim().Replace("Group: ",""),
            Status = groupInfo[1].Trim().Replace("Status: ",""),
            Policies = groupInfo[2].Trim().Replace("Policy: ",""),
            Members = groupInfo[3].Trim().Replace("Members: ","")
        });
    }

    public async Task<Result> CreateDefaultPolicyAndAttachUser(string username, string bucketName)
    {
        string policyJson = $$"""
                              {
                                "Version": "2012-10-17",
                                "Statement": [
                                  {
                                    "Effect": "Allow",
                                    "Action": [
                                      "s3:GetObject",
                                      "s3:PutObject",
                                      "s3:DeleteObject",
                                      "s3:ListBucket"
                                    ],
                                    "Resource": "arn:aws:s3:::{{bucketName}}/*"
                                  }
                                ]
                              }
                              """;

        string policyName = $"{username}-policy";
        var policyResult = await CreatePolicyAsync(policyName, policyJson);
        
        return await AttachPolicyToUser(policyName, username);
    }

    public async Task<Result> CreatePolicyAsync(string policyName, string policyJson)
    {
        string tempFilePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(tempFilePath, policyJson);
            string args = $"admin policy create {_alias} {policyName} {tempFilePath}";
            var result = await RunProcessAsync(args);
            return Helpers.ResultHelper.ConvertToResult(result);
        }
        finally
        {
            try
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
            catch
            {
                // opsiyonel: loglama
            }
        }
    }
    private async Task<Result<string>> RunProcessAsync(string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _mcPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();
            

        if (process.ExitCode == 0) return Result.Ok(output);

        string errorMessage = !string.IsNullOrEmpty(error) ? error : output;

        return Result.Fail(errorMessage);
    }
}