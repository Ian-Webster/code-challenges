using System.Collections;
using BankOcr.Business.Services;

namespace BankOcr.Business.Tests.AccountNumber;

public class AccountNumberServiceBase
{
    protected AccountNumberService GetService()
    {
        return new AccountNumberService();
    }
}