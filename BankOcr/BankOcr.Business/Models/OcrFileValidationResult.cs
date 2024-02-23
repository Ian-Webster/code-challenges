namespace BankOcr.Business.Models;

public class OcrFileValidationResult
{
    public bool IsValid { get; set; }

    public string ValidationFailure { get; set; }
}