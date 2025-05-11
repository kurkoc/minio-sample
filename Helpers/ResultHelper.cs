using FluentResults;

namespace MinioSample.Helpers;

public class ResultHelper
{
    public static Result ConvertToResult(Result<string> result)
    {
        if (result.IsSuccess) return Result.Ok();

        string errorMessage = result.Errors[0].Message;
        return Result.Fail(errorMessage);
    } 
    
    public static Result<T> ConvertToResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Result.Ok(result.Value);

        string errorMessage = result.Errors[0].Message;
        return Result.Fail(errorMessage);
    }
}