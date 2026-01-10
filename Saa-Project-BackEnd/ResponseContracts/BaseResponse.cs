namespace Saa_Project_BackEnd.ResponseContracts; 
using System.Collections.Generic;


public class BaseResponse <T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static BaseResponse<T> Success(T data) 
        => new() { IsSuccess = true, Data = data};

    public static BaseResponse<T> Failure(List<string> errors) 
        => new() { IsSuccess = false, Errors = errors };
}