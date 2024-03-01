using System.Collections;
using BankOcr.Business.Enums;
using BankOcr.Business.Models;
using BankOcr.Business.Services;
using NSubstitute;

namespace BankOcr.Business.Tests.Services.Ocr;

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

    protected void SetAccountNumberServiceGetValidAccountNumbers(List<string> accountNumbers)
    {
        MockAccountNumberService.GetValidAccountNumbers(Arg.Any<List<string>>()).Returns(accountNumbers);
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

    public static IEnumerable GetOcrNumberTestCaseData()
    {
        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "| || || || || || || || || |\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "000000000",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "000000000" }
        ).SetName("OK - 000000000");

        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "711111111",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "711111111" }
        ).SetName("OK - 711111111");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "777777177",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "777777177" }
        ).SetName("OK - 777777177");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _|| || || || || || || || |\n" +
            "|_ |_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "200800000",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "200800000" }
        ).SetName("OK - 200800000");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "333393333",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "333393333" }
        ).SetName("OK - 333393333");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "888888888",
                    Status = AccountNumberStatus.Ambiguous
                },
                PossibleMatches = new List<string> { "888886888", "888888880", "888888988" }
            },
            new List<string> { "888886888", "888888880", "888888988" }
        ).SetName("AMB - 888888888");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "555555555",
                    Status = AccountNumberStatus.Ambiguous
                },
                PossibleMatches = new List<string> { "555655555", "559555555" }
            },
            new List<string> { "555655555", "559555555" }
        ).SetName("AMB - 555555555");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "666666666",
                    Status = AccountNumberStatus.Ambiguous
                },
                PossibleMatches = new List<string> { "666566666", "686666666" }
            },
            new List<string> { "666566666", "686666666" }
        ).SetName("AMB - 666666666");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_||_||_||_||_||_||_||_||_|\n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "999999999",
                    Status = AccountNumberStatus.Ambiguous
                },
                PossibleMatches = new List<string> { "899999999", "993999999", "999959999" }
            },
            new List<string> { "899999999", "993999999", "999959999" }
        ).SetName("AMB - 999999999");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| || ||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "490067715",
                    Status = AccountNumberStatus.Ambiguous
                },
                PossibleMatches = new List<string> { "490067115", "490067719", "490867715" }
            },
            new List<string> { "490067115", "490067719", "490867715" }
        ).SetName("AMB - 490067715");

        yield return new TestCaseData(
            "    _  _     _  _  _  _  _ \n" +
            " _| _| _||_||_ |_   ||_||_|\n" +
            "  ||_  _|  | _||_|  ||_| _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "123456789",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "123456789" }
        ).SetName("OK - 123456789");

        yield return new TestCaseData(
            " _     _  _  _  _  _  _    \n" +
            "| || || || || || || ||_   |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "000000051",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "000000051" }
        ).SetName("OK - 000000051");

        yield return new TestCaseData(
            "    _  _  _  _  _  _     _ \n" +
            "|_||_|| ||_||_   |  |  ||_ \n" +
            "  | _||_||_||_|  |  |  | _|\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "490867715",
                    Status = AccountNumberStatus.Ok
                }
            },
            new List<string> { "490867715" }
        ).SetName("OK - 490867715");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _    \n" +
            "| || || || || || || | _|  |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "000000031",
                    Status = AccountNumberStatus.Error
                }
            },
            null
        ).SetName("ERR - 000000031");

        yield return new TestCaseData(
            " _     _  _  _  _  _  _    \n" +
            "|_|  || || || || || | _|  |\n" +
            "|_|  ||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "810000031",
                    Status = AccountNumberStatus.Error
                }
            },
            null
        ).SetName("ERR - 810000031");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _    \n" +
            "|_|  || || || || || ||_   |\n" +
            " _|  ||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "970000051",
                    Status = AccountNumberStatus.Error
                }
            },
            null
        ).SetName("ERR - 970000051");


        yield return new TestCaseData(
            " _  _  _  _  _  _  _       \n" +
            "| || || || || || || | _   |\n" +
            "|_||_||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "0000000?1",
                    Status = AccountNumberStatus.Illegible
                }
            },
            null
        ).SetName("ILL - 0000000?1");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _       \n" +
            "|  | || || || || || | _   |\n" +
            "| ||_||_||_||_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "?000000?1",
                    Status = AccountNumberStatus.Illegible
                }
            },
            null
        ).SetName("ILL - ?000000?1");

        yield return new TestCaseData(
            " _  _  _     _  _  _       \n" +
            "|  | || |  || || || | _   |\n" +
            "| ||_||_||_ |_||_||_| _|  |\n" +
            "\n",
            new AccountNumberRow
            {
                Data = new Business.Models.AccountNumber()
                {
                    Number = "?00?000?1",
                    Status = AccountNumberStatus.Illegible
                }
            },
            null
        ).SetName("ILL - ?00?000?1");
    }

    public static IEnumerable GetValidateOcrFileFailureTestCaseData()
    {
        yield return new TestCaseData(
            "",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file is empty"
            }
        ).SetName("Empty file");

        yield return new TestCaseData(
            null,
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file is empty"
            }
        ).SetName("Null contents file");

        yield return new TestCaseData(
            "123",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - 123");

        yield return new TestCaseData(
            "abc",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - abc");

        yield return new TestCaseData(
            "!%^",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - !%^");

        yield return new TestCaseData(
            "¬`+",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - ¬`");

        yield return new TestCaseData(
            "\\/",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - \\/");

        yield return new TestCaseData(
            "|_r",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = "OCR file contains illegal characters"
            }
        ).SetName("Illegal characters - |_r");

        yield return new TestCaseData(
            "                           \n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "  |  |  |  |  |  |  |  |  |\n" +
            "\n" +
            " _  _  _  _  _  _  _  _  _ \n" +
            " _| _| _| _| _| _| _| _| _|\n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "\n",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = $"OCR file is not divisible by the expected number of characters per row ({OcrService.CharactersPerOcrRow})"
            }
        ).SetName("Not divisible - test 1");

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
            "    _  _     _ " +
            "  | _| _||_||_ " +
            "  ||_  _|  | _|" +
            "\n",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = $"OCR file is not divisible by the expected number of characters per row ({OcrService.CharactersPerOcrRow})"
            }
        ).SetName("Not divisible - test 2");

        yield return new TestCaseData(
            " _  _  _  _  _  _  _  _  _ \n" +
            "|_ |_ |_ |_ |_ |_ |_ |_ |_ \n" +
            "|_||_||_||_||_||_||_||_||_|\n",
            new OcrFileValidationResult
            {
                IsValid = false,
                ValidationFailure = $"OCR file is not divisible by the expected number of characters per row ({OcrService.CharactersPerOcrRow})"
            }
        ).SetName("Not divisible - test 3");
    }
}