namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class ConvertOcrNumberToAccountNumber: OcrServiceBase
{
    [TestCaseSource(nameof(GetInvalidNumberTestCaseData))]
    public void Should_Return_InvalidAccountNumber_When_OcrRowIsInValid(string orcRow, string expectedOutPut, bool shouldBeValid)
    {
        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(expectedOutPut, result.Number);
        if (shouldBeValid)
        {
            Assert.That(result.Status, Is.Null);
        }
        else
        {
            Assert.AreEqual("ILL", result.Status);
        }
    }

    [TestCaseSource(nameof(GetOcrNumberTestCaseData))]
    public void Should_Return_ExpectedAccountNumber_When_OrcRowIsValid(string orcRow, string expectedAccountNumber)
    {
        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(expectedAccountNumber, result.Number);
    }
}