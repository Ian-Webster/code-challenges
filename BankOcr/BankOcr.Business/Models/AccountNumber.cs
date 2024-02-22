namespace BankOcr.Business.Models;

/// <summary>
/// Holds data about an individual account number
/// </summary>
public class AccountNumber
{
    /// <summary>
    /// The account number
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// The status for this account number
    /// </summary>
    /// <remarks>
    /// Used to indicate if the account number is valid or not
    /// </remarks>
    public string? Status { get; set; }
}