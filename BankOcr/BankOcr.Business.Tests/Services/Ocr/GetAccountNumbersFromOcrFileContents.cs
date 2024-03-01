namespace BankOcr.Business.Tests.Services.Ocr;

[TestFixture]
public class GetAccountNumbersFromOcrFileContents : OcrServiceBase
{
    [TestCase(true)]
    [TestCase(false)]
    public void Should_Return_EmptyList_When_OcrFileContentsIsNullOrEmpty(bool isNull)
    {
        // Arrange
        var ocrFileContents = isNull ? null : string.Empty;

        // Act
#pragma warning disable CS8604 // Possible null reference argument. - deliberately testing null/empty
        var result = GetService().GetAccountNumbersFromOcrFileContents(ocrFileContents);
#pragma warning restore CS8604 // Possible null reference argument.

        // Assert
        Assert.That(result, Is.Empty);
    }

    [TestCaseSource(nameof(GetOcrFileContentsTestCaseData))]
    public void Should_Return_ExpectedAccountNumbers(string ocrFileContents, List<string> expectedAccountNumbers)
    {
        // Act
        var result = GetService().GetAccountNumbersFromOcrFileContents(ocrFileContents);

        // Assert
        CollectionAssert.AreEqual(expectedAccountNumbers, result.Select(r => r.Data.Number));
    }
}