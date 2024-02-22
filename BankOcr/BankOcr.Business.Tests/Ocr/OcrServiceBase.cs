using System.Collections;
using System.Text;
using BankOcr.Business.Models;
using BankOcr.Business.Services;
using NSubstitute;

namespace BankOcr.Business.Tests.Ocr;

public class OcrServiceBase
{
    protected IAccountNumberService MockAccountNumberService;

    [SetUp]
    protected void Setup()
    {
        MockAccountNumberService = Substitute.For<IAccountNumberService>();
    }

    protected OcrService GetService()
    {
        return new OcrService(MockAccountNumberService);
    }

    protected void SetAccountNumberServiceValidation(bool isValid)
    {
        MockAccountNumberService.AccountNumberIsValid(Arg.Any<string>()).Returns(isValid);
    }

    protected void SetAccountNumberServiceGetValidAccountNumbers(List<string> accountNumbers)
    {
        MockAccountNumberService.GetValidAccountNumbers(Arg.Any<List<string>>()).Returns(accountNumbers);
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

    /*public static IEnumerable GetOcrNumberTestCaseData()
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
    }*/

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

    /*public static IEnumerable GetInvalidNumberTestCaseData()
    {
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _    \n" +
            "| || || || || || || ||_   |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            "000000051",
            "ERR"
        ).SetName("Valid");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| || ||_   |  |  | _ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            "49006771?",
            "ILL"
        ).SetName("Last character invalid");

        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            "  | _| _||_| _ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _ \n" +
            "\n",
            "1234?678?",
            "ILL"
        ).SetName("Multiple invalid characters");
    }*/

    public static IEnumerable GetAccountValidationTestCaseData()
    {
        // valid
        // 711111111
        yield return new TestCaseData(
            " _                         \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            "711111111", 
            true
        ).SetName("Valid 711111111");
        // 123456789
        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            "  | _| _||_||_ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _|\n" +
            "\n",
            "123456789", 
            true
        ).SetName("Valid 123456789");
        // 490867715
        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| ||_||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            "490867715", 
            true
        ).SetName("Valid 490867715");

        // invalid
        // 888888888
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            "888888888", 
            false
        ).SetName("Invalid 888888888");
        // 490067715
        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| || ||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            "490067715", 
            false
        ).SetName("Invalid 490067715");
        // 012345678
        yield return new TestCaseData(
            " _     _  _     _  _  _  _ \n" +
            "| |  | _| _||_||_ |_   ||_|\n" +
            "|_|  ||_  _|  | _||_|  ||_|\n" +
            "\n",
            "012345678", 
            false
        ).SetName("Invalid 012345678");
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
            new List<char> {'1'}
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

    public static IEnumerable GetOcrNumberTestCaseData()
    {
        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "711111111",
                    Status = "Ok"
                }
            },
            new List<string> { "711111111" }
        ).SetName("711111111");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "777777177",
                    Status = "Ok"
                }
            },
            new List<string> { "777777177" }
        ).SetName("777777177");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _|| || || || || || || || |\n" +
            "|_ |_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "200800000",
                    Status = "Ok"
                }
            },
            new List<string> { "200800000" }
        ).SetName("200800000");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "333393333",
                    Status = "Ok"
                }
            },
            new List<string> { "333393333" }
        ).SetName("333393333");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "888888888",
                    Status = "AMB"
                },
                PossibleMatches = new List<string> { "888886888", "888888880", "888888988" }
            },
            new List<string> { "888886888", "888888880", "888888988" }
        ).SetName("888888888");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "555555555",
                    Status = "AMB"
                },
                PossibleMatches = new List<string> { "555655555", "559555555" }
            },
            new List<string> { "555655555", "559555555" }
        ).SetName("555555555");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "666666666",
                    Status = "AMB"
                },
                PossibleMatches = new List<string> { "666566666", "686666666" }
            },
            new List<string> { "666566666", "686666666" }
        ).SetName("666666666");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "999999999",
                    Status = "AMB"
                },
                PossibleMatches = new List<string> { "899999999", "993999999", "999959999" }
            },
            new List<string> { "899999999", "993999999", "999959999" }
        ).SetName("999999999");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| || ||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "490067715",
                    Status = "AMB"
                },
                PossibleMatches = new List<string> { "490067115", "490067719", "490867715" }
            },
            new List<string> { "490067115", "490067719", "490867715" }
        ).SetName("490067715");

        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            " _| _| _||_||_ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "123456789",
                    Status = "Ok"
                }
            },
            new List<string> { "123456789" }
        ).SetName("123456789");

        yield return new TestCaseData(
            " _     _  _  _  _  _  _    \n" +
            "| || || || || || || ||_   |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "000000051",
                    Status = "Ok"
                }
            },
            new List<string> { "000000051" }
        ).SetName("000000051");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| ||_||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Models.AccountNumber()
                {
                    Number = "490867715",
                    Status = "Ok"
                }
            },
            new List<string> { "490867715" }
        ).SetName("490867715");
    }

}