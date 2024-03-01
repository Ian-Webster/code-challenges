namespace BankOcr.Business.Tests.Models.OcrDigit;

[TestFixture]
public class ConvertOcrDigitToNumber : OcrDigitBase
{
    [TestCaseSource(nameof(GetOcrDigitTestCaseData))]
    public void Should_Return_ExpectedDigit(string ocrString, char expectedDigit)
    {
        // Act
        var result = Business.Models.OcrDigit.ConvertOcrDigitToNumber(ocrString);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDigit));
    }

    [TestCaseSource(nameof(GetIllegibleOcrDigitTestCaseData))]
    public void Should_Return_IllegibleChar_WhenDigitIsIllegible(string ocrString)
    {
        // Act
        var result = Business.Models.OcrDigit.ConvertOcrDigitToNumber(ocrString);

        // Assert
        Assert.That(result, Is.EqualTo('?'));
    }
}