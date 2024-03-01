using System.Collections;

namespace BankOcr.Business.Tests.Models.OcrDigit;

public class OcrDigitBase
{
    public static IEnumerable GetOcrDigitTestCaseData()
    {
        yield return new TestCaseData
        (
            " _ \n" +
            "| |\n" +
            "|_|\n" +
            "",
            '0'
        ).SetName("0");
        yield return new TestCaseData
        (
            "   \n" +
            "  |\n" +
            "  |\n" +
            "",
            '1'
        ).SetName("1");
        yield return new TestCaseData
        (
            " _ \n" +
            " _|\n" +
            "|_ \n" +
            "",
            '2'
        ).SetName("2");
        yield return new TestCaseData
        (
            " _ \n" +
            " _|\n" +
            " _|\n" +
            "",
            '3'
        ).SetName("3");
        yield return new TestCaseData
        (
            "   \n" +
            "|_|\n" +
            "  |\n" +
            "",
            '4'
        ).SetName("4");
        yield return new TestCaseData
        (
            " _ \n" +
            "|_ \n" +
            " _|\n" +
            "",
            '5'
        ).SetName("5");
        yield return new TestCaseData
        (
            " _ \n" +
            "|_ \n" +
            "|_|\n" +
            "",
            '6'
        ).SetName("6");
        yield return new TestCaseData
        (
            " _ \n" +
            "  |\n" +
            "  |\n" +
            "",
            '7'
        ).SetName("7");
        yield return new TestCaseData
        (
            " _ \n" +
            "|_|\n" +
            "|_|\n" +
            "",
            '8'
        ).SetName("8");
        yield return new TestCaseData
        (
            " _ \n" +
            "|_|\n" +
            " _|\n" +
            "",
            '9'
        ).SetName("9");
    }

    public static IEnumerable GetIllegibleOcrDigitTestCaseData()
    {
        yield return new TestCaseData
        (
            "   \n" +
            "| |\n" +
            "| |\n" +
            ""
        ).SetName("Test case 1");

        yield return new TestCaseData
        (
            "   \n" +
            "  |\n" +
            "   \n" +
            ""
        ).SetName("Test case 2");

        yield return new TestCaseData
        (
            " _ \n" +
            " _ \n" +
            " _ \n" +
            ""
        ).SetName("Test case 3");

        yield return new TestCaseData
        (
            "   \n" +
            " _ \n" +
            " _|\n" +
            ""
        ).SetName("Test case 4");

        yield return new TestCaseData
        (
            "   \n" +
            " _ \n" +
            "   \n" +
            ""
        ).SetName("Test case 5");

        yield return new TestCaseData
        (
            "   \n" +
            "   \n" +
            "   \n" +
            ""
        ).SetName("Test case 6");

        yield return new TestCaseData
        (
            "   \n" +
            "|  \n" +
            "| \n" +
            ""
        ).SetName("Test case 7");
    }

    public static IEnumerable GetOcrDigitInvalidGuessTestCaseData()
    {
        // digit that are valid but could be other numbers 
        yield return new TestCaseData(
            " _ \n" +
            "| |\n" +
            "|_|\n" +
            "",
            new List<char> { '8' }
        ).SetName("0 could be 8");

        yield return new TestCaseData(
            "   \n" +
            "  |\n" +
            "  |\n" +
            "",
            new List<char> { '7' }
        ).SetName("1 could be 7");

        yield return new TestCaseData(
            " _ \n" +
            " _|\n" +
            " _|\n" +
            "",
            new List<char> { '9' }
        ).SetName("3 could be 9");

        yield return new TestCaseData(
            " _ \n" +
            "|_ \n" +
            "|_|\n" +
            "",
            new List<char> { '5', '8' }
        ).SetName("6 could be 5 or 8");

        yield return new TestCaseData(
            " _ \n" +
            "  |\n" +
            "  |\n" +
            "",
            new List<char> { '1' }
        ).SetName("7 could be 1");

        yield return new TestCaseData(
            " _ \n" +
            "|_|\n" +
            "|_|\n" +
            "",
            new List<char> { '0', '6', '9' }
        ).SetName("8 could be 0, 6 or 9");

        yield return new TestCaseData(
            " _ \n" +
            "|_|\n" +
            " _|\n" +
            "",
            new List<char> { '3', '5', '8' }
        ).SetName("9 could be 3, 5 or 8");

        // digits that are invalid
        yield return new TestCaseData(
            "   \n" +
            " _|\n" +
            "  |\n" +
            "",
            new List<char> { '1', '4' }
        ).SetName("Invalid #1 could be 1 or 4");

        yield return new TestCaseData(
            "   \n" +
            "| |\n" +
            "|_|\n" +
            "",
            new List<char> { '0' }
        ).SetName("Invalid #2 could be 0");

        yield return new TestCaseData(
            " _ \n" +
            " _ \n" +
            " _|\n" +
            "",
            new List<char> { '3', '5' }
        ).SetName("Invalid #3 could be 3 or 5");
    }
}