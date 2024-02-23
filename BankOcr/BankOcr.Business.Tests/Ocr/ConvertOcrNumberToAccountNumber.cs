using BankOcr.Business.Models;
using Newtonsoft.Json;
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

    [TestCaseSource(nameof(GetOcrNumberTestCaseData))]
    public void Should_Return_ExpectedAccountNumberRow(string orcRow, AccountNumberRow expectedOutPut, List<string> validNumbers)
    {
        // Arrange
        SetAccountNumberServiceGetValidAccountNumbers(validNumbers);

        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.That(result, Is.Not.Null);
        StringAssert.AreEqualIgnoringCase(JsonConvert.SerializeObject(expectedOutPut), JsonConvert.SerializeObject(result));
    }
}