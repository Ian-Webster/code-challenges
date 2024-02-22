using System;

namespace BankOcr.Business.Tests.AccountNumber;

[TestFixture]
public class AccountNumberIsValid: AccountNumberServiceBase
{
    [TestCase(true)]
    [TestCase(false)]
    public void Should_ReturnFalse_WhenAccountNumberIsNullOrEmpty(bool isNull)
    {
        // Act
        var result = GetService().AccountNumberIsValid(isNull ? null : string.Empty);

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase("123")]
    [TestCase("1234567890")]
    public void Should_ReturnFalse_WhenAccountNumberIsWrongLength(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False); ;
    }

    [TestCase("abcdefghi")]
    [TestCase("_=+()!£$%")]
    public void Should_ReturnFalse_WhenAccountNumberIsWNumeric(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase("711111111")]
    [TestCase("123456789")]
    [TestCase("490867715")]
    public void Should_ReturnTrue_WhenAccountNumberIsValid(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.True);
    }

    [TestCase("888888888")]
    [TestCase("490067715")]
    [TestCase("012345678")]
    public void Should_ReturnFalse_WhenAccountNumberIsinValid(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False);
    }
}