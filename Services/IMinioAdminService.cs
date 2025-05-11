using FluentResults;
using MinioSample.Dtos;

namespace MinioSample.Services;

public interface IMinioAdminService
{
    Task<Result> MakeBucket(string name);
    Task<Result<List<string>>> GetUsers();
    Task<Result> AddUser(string username, string password);
    Task<Result> AddUsersToGroup(string groupName, List<string> users);
    Task<Result<List<string>>> GetGroups();
    Task<Result<GroupInfoDto>> GetGroup(string groupName);
    Task<Result> AttachPolicyToUser(string policy, string user);
    Task<Result> CreatePolicyAsync(string policyName, string policyJson);
    Task<Result> CreateDefaultPolicyAndAttachUser(string username, string bucketName);
}