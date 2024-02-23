namespace BankOcr.Business.Tests.AccountNumber;

[TestFixture]
public class GetValidAccountNumbers: AccountNumberServiceBase
{
    [TestCaseSource(nameof(GetValidAccountNumberTestCaseData))]
    public void Should_Return_ExpectedResult(List<string> accountNumbers, List<string> expected)
    {
        // Act
        var result = GetService().GetValidAccountNumbers(accountNumbers);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}