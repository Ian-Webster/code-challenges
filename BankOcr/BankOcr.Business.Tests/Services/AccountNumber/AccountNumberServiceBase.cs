using System.Collections;
using BankOcr.Business.Services;

namespace BankOcr.Business.Tests.Services.AccountNumber;

public class AccountNumberServiceBase
{
    protected AccountNumberService GetService()
    {
        return new AccountNumberService();
    }

    protected static IEnumerable GetValidAccountNumberTestCaseData()
    {
        yield return new TestCaseData(
            new List<string> { "888888888", "490067715", "012345678" },
            null
        ).SetName("All invalid");

        yield return new TestCaseData(
            new List<string> { "711111111", "123456789", "490867715" },
            new List<string> { "711111111", "123456789", "490867715" }
        ).SetName("All valid");

        yield return new TestCaseData(
            new List<string> { "888888888", "490067715", "490867715" },
            new List<string> { "490867715" }
        ).SetName("One valid");
    }
}