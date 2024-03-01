namespace BankOcr.Business.Tests.Models.OcrDigit;

[TestFixture]
public class GuessOcrDigit : OcrDigitBase
{
    [TestCaseSource(nameof(GetOcrDigitInvalidGuessTestCaseData))]
    public void Should_Return_ExpectedGuesses(string ocrDigit, List<char> expectedGuesses)
    {
        // Act
        var result = Business.Models.OcrDigit.GuessOcrDigit(ocrDigit);

        // Assert
        Assert.That(result, Is.Not.Null);
        CollectionAssert.AreEquivalent(expectedGuesses, result);
    }

    [TestCaseSource(nameof(GetIllegibleOcrDigitTestCaseData))]
    public void Should_Return_Null_WhenCharacterIsIllegible(string ocrDigit)
    {
        // Act
        var result = Business.Models.OcrDigit.GuessOcrDigit(ocrDigit);

        // Assert
        Assert.That(result, Is.Null);
    }
}