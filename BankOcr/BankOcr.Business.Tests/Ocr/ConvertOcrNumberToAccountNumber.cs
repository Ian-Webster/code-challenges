using NSubstitute;

namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class ConvertOcrNumberToAccountNumber: OcrServiceBase
{
    [TestCaseSource(nameof(GetAccountValidationTestCaseData))]
    public void Should_Call_AccountNumberService_AccountNumberIsValid_WithExpectedData(string ocrNumber,
        string accountNumber, bool isValid)
    {
        // Act
        GetService().ConvertOcrNumberToAccountNumber(ocrNumber);

        // Assert
        MockAccountNumberService.Received(1).AccountNumberIsValid(accountNumber);
    }

    [TestCaseSource(nameof(GetInvalidNumberTestCaseData))]
    public void Should_Return_InvalidAccountNumberStatus_When_OcrRowIsInValid(string orcRow, string expectedOutPut, string expectedStatus)
    {
        // Arrange
        SetAccountNumberServiceValidation(false);

        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(expectedOutPut, result.Number);
        Assert.AreEqual(expectedStatus, result.Status);
    }

    [TestCaseSource(nameof(GetAccountValidationTestCaseData))]
    public void Should_Return_ExpectedAccountNumberStatus(string orcRow, string expectedOutPut, bool isValid)
    {
        // Arrange
        SetAccountNumberServiceValidation(isValid);

        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Number, Is.EqualTo(expectedOutPut));
        if (!isValid)
        {
            Assert.That(result.Status, Is.EqualTo("ERR"));
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