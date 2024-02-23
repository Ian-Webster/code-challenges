namespace BankOcr.Business.Services;

/// <summary>
/// Handles account number validation
/// </summary>
public interface IAccountNumberService
{
    /// <summary>
    /// Checks if the given account number is valid
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <returns></returns>
    bool AccountNumberIsValid(string accountNumber);

    /// <summary>
    /// Checks each of the given account numbers and returns any that are valid
    /// </summary>
    /// <remarks>
    /// if none of the numbers are valid will return null
    /// </remarks>
    /// <param name="accountNumbers"></param>
    /// <returns></returns>
    List<string>? GetValidAccountNumbers(List<string> accountNumbers);
}