namespace BankOcr.Business.Services;

public class AccountNumberService : IAccountNumberService
{
    public bool AccountNumberIsValid(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) return false;

        if (accountNumber.Length != 9) return false;

        if (!accountNumber.All(char.IsDigit)) return false;

        // reverse the account number and convert to byte array
        var accountNumberDigits = accountNumber
            .ToArray()
            .Reverse()
            .Select(c => byte.Parse(c.ToString()))
            .ToArray();

        // hash the result
        return ((1 * accountNumberDigits[0]) + (2 * accountNumberDigits[1]) + (3 * accountNumberDigits[2]) + (4 * accountNumberDigits[3]) + (5 * accountNumberDigits[4]) + (6 * accountNumberDigits[5]) + (7 * accountNumberDigits[6]) + (8 * accountNumberDigits[7]) + (9 * accountNumberDigits[8])) %
            11 == 0;
    }

    public List<string>? GetValidAccountNumbers(List<string> accountNumbers)
    {
        var result = new List<string>();
        accountNumbers.ForEach(accountNumber =>
        {
            if (AccountNumberIsValid(accountNumber))
            {
                result.Add(accountNumber);
            }
        });

        return result.Count > 0 ? result : null;
    }
}