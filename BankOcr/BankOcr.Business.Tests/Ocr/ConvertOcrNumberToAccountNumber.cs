namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class ConvertOcrNumberToAccountNumber: OcrServiceBase
{
    [TestCaseSource(nameof(GetOcrNumberTestCaseData))]
    public void Should_Return_ExpectedAccountNumber(string orcRow, string expectedAccountNumber)
    {
        // Act
        var result = GetService().ConvertOcrNumberToAccountNumber(orcRow);

        // Assert
        Assert.AreEqual(expectedAccountNumber, result);
    }
}