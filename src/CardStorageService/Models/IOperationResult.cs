namespace CardStorageService.Models;

public interface IOperationResult
{
    int ErrorCode { get; set; }
    string? ErrorMessage { get; set; }
}
