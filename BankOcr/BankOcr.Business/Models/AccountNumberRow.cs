namespace BankOcr.Business.Models;

/// <summary>
/// Holds data for an account number row
/// </summary>
public class AccountNumberRow
{
    /// <summary>
    /// The account number data for this row
    /// </summary>
    /// <remarks>
    /// If a good match for an account number is found this will be set to it
    /// In the event that there are several possible matching numbers this will be the original number retrieved from the OCR
    /// </remarks>
    public AccountNumber Data { get; set; }

    /// <summary>
    /// In the event that an OCR number is ambiguous this will be set to the possible matches
    /// </summary>
    /// <remarks>
    /// If there are multiple possible matches for an account number this will be set to the possible matches
    /// If there is only a single match this will be null
    /// </remarks>
    public List<String>? PossibleMatches { get; set; }
}