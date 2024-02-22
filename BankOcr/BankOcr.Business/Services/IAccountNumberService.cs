namespace BankOcr.Business.Services;

public interface IAccountNumberService
{
    bool AccountNumberIsValid(string accountNumber);
}