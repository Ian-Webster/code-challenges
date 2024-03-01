using BankOcr.Business.Models;
using Newtonsoft.Json;

namespace BankOcr.Business.Tests.Services.Ocr;

[TestFixture]
public class ValidateOcrFile : OcrServiceBase
{
    [TestCaseSource(nameof(GetValidateOcrFileFailureTestCaseData))]
    public void Should_Return_ExpectedFailedValidationResponse_WhenOcrFileIsInvalid(string ocrFileContents,
        OcrFileValidationResult expectedResult)
    {
        // Act
        var result = GetService().ValidateOcrFile(ocrFileContents);

        // Assert
        StringAssert.AreEqualIgnoringCase(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(result));
    }

    [TestCaseSource(nameof(GetOcrFileContentsTestCaseData))]
    public void Should_Return_ValidResponse_WhenOcrFileIsValid(string ocrFileContents, List<string> validNumbers)
    {
        // Act
        var result = GetService().ValidateOcrFile(ocrFileContents);

        // Assert
        Assert.IsTrue(result.IsValid);
    }
}