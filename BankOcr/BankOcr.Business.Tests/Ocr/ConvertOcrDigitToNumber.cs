namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class ConvertOcrDigitToNumber: OcrServiceBase
{
    [TestCaseSource(nameof(GetOcrDigitTestCaseData))]
    public void Should_Return_ExpectedDigit(string ocrString, char expectedDigit)
    {
        // Act
        var result = GetService().ConvertOcrDigitToNumber(ocrString);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedDigit));
    }
}