using FluentResults;
using MinioSample.Dtos;
using MinioSample.Models;

namespace MinioSample.Services;

public interface IMinioAdminService
{
    Task<Result> MakeBucket(string name);


    Task<Result<List<MinioUser>>> GetUsers();
    Task<Result<MinioUser?>> GetUser(string username);
    Task<Result> DisableUser(string username);
    Task<Result> EnableUser(string username);
    Task<Result> AddUser(string username, string password);
    Task<Result> DeleteUser(string username);
    Task<Result> AddUsersToGroup(string groupName, List<string> users);
    
    
    Task<Result<List<string>>> GetGroups();
    Task<Result<MinioGroup>> GetGroup(string groupName);
    Task<Result> DisableGroup(string groupName);
    Task<Result> EnableGroup(string groupName);
    Task<Result> DeleteGroup(string groupName);
    
    Task<Result> AttachPolicyToUser(string policy, string user);
    Task<Result> CreatePolicyAsync(string policyName, string policyJson);
    Task<Result> CreateDefaultPolicyAndAttachUser(string username, string bucketName);
}