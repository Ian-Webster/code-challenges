namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class GuessOcrDigit: OcrServiceBase
{
    [TestCaseSource(nameof(GetOcrDigitInvalidGuessTestCaseData))]
    public void Should_Return_ExpectedGuesses(string ocrDigit, List<char> expectedGuesses)
    {
        // Act
        var result = GetService().GuessOcrDigit(ocrDigit);

        // Assert
        Assert.That(result, Is.Not.Null);
        CollectionAssert.AreEquivalent(expectedGuesses, result);
    }
}