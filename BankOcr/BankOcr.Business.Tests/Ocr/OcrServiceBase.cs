using System.Collections;
using BankOcr.Business.Services;

namespace BankOcr.Business.Tests.Ocr;

public class OcrServiceBase
{
    protected OcrService GetService()
    {
        return new OcrService();
    }

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

    public static IEnumerable GetOcrNumberTestCaseData()
    {
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "| || || || || || || || || |\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            "000000000"
        ).SetName("Zeros");
        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            "111111111"
        ).SetName("Ones");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "\n",
            "222222222"
        ).SetName("Twos");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            "333333333"
        ).SetName("Threes");
        yield return new TestCaseData(
            "                           \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            "444444444"
        ).SetName("Fours");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            "555555555"
        ).SetName("Fives");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            "666666666"
        ).SetName("Sixes");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            "777777777"
        ).SetName("Sevens");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            "888888888"
        ).SetName("Eights");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            "999999999"
        ).SetName("Nines");
        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            "  | _| _||_||_ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _|\n" +
            "\n",
            "123456789"
        ).SetName("One to nine");
    }

    public static IEnumerable GetOcrFileContentsTestCaseData()
    {
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "| || || || || || || || || |\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new List<string> { "000000000" }
        ).SetName("1 row");
        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n" +
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "\n",
            new List<string> { "111111111", "222222222" }
        ).SetName("2 rows");
        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n" +
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "\n" +
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new List<string> { "111111111", "222222222", "333333333" }
        ).SetName("3 rows");
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n" +
            "                           \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n" +
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n" +
            "    _  _     _  _  _  _  _ \n" +
            "  | _| _||_||_ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _|\n" +
            "\n",
            new List<string> { "666666666", "444444444", "999999999", "123456789" }
        ).SetName("4 rows");
    }

    public static IEnumerable GetInvalidNumberTestCaseData()
    {
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _    \n" +
            "| || || || || || || ||_   |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            "000000051",
            true
        ).SetName("Valid");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| || ||_   |  |  | _ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            "49006771?",
            false
        ).SetName("Last character invalid");

        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            "  | _| _||_| _ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _ \n" +
            "\n",
            "1234?678?",
            false
        ).SetName("Multiple invalid characters");
    }
}