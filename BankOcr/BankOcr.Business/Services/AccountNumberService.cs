namespace BankOcr.Business.Services;

public class AccountNumberService : IAccountNumberService
{
    public bool AccountNumberIsValid(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) return false;

        if (accountNumber.Length != 9) return false;

        if (!accountNumber.All(char.IsDigit)) return false;

        return accountNumber
            .ToArray()
            .Reverse()
            .Select((c, index) => (index+1) * byte.Parse(c.ToString())).Sum() % 11 == 0;
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