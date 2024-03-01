namespace BankOcr.Business.Tests.Services.AccountNumber;

[TestFixture]
public class AccountNumberIsValid : AccountNumberServiceBase
{
    [TestCase(true)]
    [TestCase(false)]
    public void Should_Return_False_When_AccountNumberIsNullOrEmpty(bool isNull)
    {
        // Act
#pragma warning disable CS8604 // Possible null reference argument - deliberately testing null/empty
        var result = GetService().AccountNumberIsValid(isNull ? null : string.Empty);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase("123")]
    [TestCase("1234567890")]
    public void Should_Return_False_When_AccountNumberIsWrongLength(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False); ;
    }

    [TestCase("abcdefghi")]
    [TestCase("_=+()!£$%")]
    public void Should_Return_False_When_AccountNumberIsNotNumeric(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False);
    }

    [TestCase("711111111")]
    [TestCase("123456789")]
    [TestCase("490867715")]
    public void Should_Return_True_When_AccountNumberIsValid(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.True);
    }

    [TestCase("888888888")]
    [TestCase("490067715")]
    [TestCase("012345678")]
    public void Should_Return_False_When_AccountNumberIsInValid(string accountNumber)
    {
        // Act
        var result = GetService().AccountNumberIsValid(accountNumber);

        // Assert
        Assert.That(result, Is.False);
    }
}