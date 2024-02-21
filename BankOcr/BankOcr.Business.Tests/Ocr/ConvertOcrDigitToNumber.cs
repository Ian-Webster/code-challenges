namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class ConvertOcrDigitToNumber: OcrServiceBase
{
    [TestCaseSource(nameof(GetOcrTestData))]
    public void Should_Return_ExpectedDigit(string ocrString, int expectedDigit)
    {
        // Act
        var result = GetService().ConvertOcrDigitToNumber(ocrString);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedDigit));
    }
}