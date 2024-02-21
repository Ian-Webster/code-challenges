namespace BankOcr.Business.Tests.Ocr;

[TestFixture]
public class GetAccountNumbersFromOcrFileContents: OcrServiceBase
{
    [TestCaseSource(nameof(GetOcrFileContentsTestCaseData))]
    public void Should_Return_ExpectedAccountNumbers(string ocrFileContents, List<string> expectedAccountNumbers)
    {
        // Act
        var result = GetService().GetAccountNumbersFromOcrFileContents(ocrFileContents);

        // Assert
        CollectionAssert.AreEqual(expectedAccountNumbers, result);
    }
}