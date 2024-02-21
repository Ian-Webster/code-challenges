using System.Collections;
using BankOcr.Business.Services;

namespace BankOcr.Business.Tests.Ocr;

public class OcrServiceBase
{
    protected OcrService GetService()
    {
        return new OcrService();
    }

    public static IEnumerable GetOcrTestData()
    {
        yield return new TestCaseData
        (
            " _ \n" +
            "| |\n" +
            "|_|\n" +
            "",
            0
        );
        yield return new TestCaseData
        (
            "   \n" +
            "  |\n" +
            "  |\n" +
            "",
            1
        );
        yield return new TestCaseData
        (
            " _ \n" +
            " _|\n" +
            "|_ \n" +
            "",
            2
        );
        yield return new TestCaseData
        (
            " _ \n" +
            " _|\n" +
            " _|\n" +
            "",
            3
        );
        yield return new TestCaseData
        (
            "   \n" +
            "|_|\n" +
            "  |\n" +
            "",
            4
        );
        yield return new TestCaseData
        (
            " _ \n" +
            "|_ \n" +
            " _|\n" +
            "",
            5
        );
        yield return new TestCaseData
        (
            " _ \n" +
            "|_ \n" +
            "|_|\n" +
            "",
            6
        );
        yield return new TestCaseData
        (
            " _ \n" +
            "  |\n" +
            "  |\n" +
            "",
            7
        );
        yield return new TestCaseData
        (
            " _ \n" +
            "|_|\n" +
            "|_|\n" +
            "",
            8
        );
        yield return new TestCaseData
        (
            " _ \n" +
            "|_|\n" +
            " _|\n" +
            "",
            9
        );
    }

}