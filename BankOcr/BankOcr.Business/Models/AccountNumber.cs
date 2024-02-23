using BankOcr.Business.Enums;

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
    public AccountNumberStatus Status { get; set; }

    public string StatusFriendlyMessage => Status switch
    {
        AccountNumberStatus.Error => "ERR",
        AccountNumberStatus.Illegible => "ILL",
        AccountNumberStatus.Ambiguous => "AMB",
        _ => string.Empty
    };
}