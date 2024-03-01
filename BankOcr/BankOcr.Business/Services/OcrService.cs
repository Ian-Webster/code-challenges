using System.Text;
using System.Text.RegularExpressions;
using BankOcr.Business.Enums;
using BankOcr.Business.Models;

namespace BankOcr.Business.Services;

/// <summary>
/// Handles converting OCR data to account numbers
/// </summary>
public class OcrService
{
    private readonly IAccountNumberService _accountNumberService;
    public const byte CharactersPerOcrRow = 85;

    public OcrService(IAccountNumberService accountNumberService)
    {
        _accountNumberService = accountNumberService;
    }

    /// <summary>
    /// Converts a single "row" of OCR data to an account number
    /// </summary>
    /// <remarks>
    /// An OCR "row" consists of 3 lines of 27 characters, each digit is 3 by 3 characters.
    /// there is an additional blank line under each "row" of digits.
    /// </remarks>
    /// <param name="orcRow"></param>
    /// <returns></returns>
    public AccountNumberRow ConvertOcrNumberToAccountNumber(string orcRow)
    {
        var rowSegments = orcRow.Split("\n"); // we expect 4 segments, 3 for the digits and 1 for the blank line
        var accountNumber = new StringBuilder();
        var digitOcr = new StringBuilder();
        var digits = new List<(int Index, string DigitOcr, bool IsBad)>();
        for (var digitIndex = 0; digitIndex < 9; digitIndex++) // each row should be 9 digits long
        {
            // read a digit from the row, we need i*3 characters from the first three lines
            for (var lineIndex = 0; lineIndex < 3; lineIndex++)
            {
                digitOcr.Append($"{rowSegments[lineIndex].Substring(digitIndex * 3, 3)}\n");
            }
            var ocrString = digitOcr.ToString(); // capture the OCR string for the digit
            var parsedDigit = OcrDigit.ConvertOcrDigitToNumber(ocrString); // attempt to parse the digit
            digits.Add((digitIndex, digitOcr.ToString(), parsedDigit == '?')); // track the digits so if we need to make guesses later we know where each one belongs
            accountNumber.Append(parsedDigit); // append the parsed digit to the account number
            digitOcr.Clear(); // reset the string builder for the next digit
        }

        var result = new AccountNumberRow { Data = new AccountNumber { Number = accountNumber.ToString(), Status = AccountNumberStatus.Error} };

        // is the number valid?
        if (!digits.Any(c => c.IsBad) && _accountNumberService.AccountNumberIsValid(result.Data.Number))
        {
            // account number is valid return ok
            result.Data.Status = AccountNumberStatus.Ok;
            return result;
        }

        // number is invalid or has bad digits - we'll have to try and guess some alternatives
        var guessedNumbers = GuessAlternativeAccountNumbers(digits.ToDictionary(key => key.Index, v => v.DigitOcr), result.Data.Number);
        if (guessedNumbers != null)
        {
            // we managed to make some guesses
            result = ValidateGuessAndUpdateRow(result, guessedNumbers);

            // we found a single valid guess
            if (result.Data.Status == AccountNumberStatus.Ok) return result;

            // we found multiple valid guesses
            if (result.Data.Status == AccountNumberStatus.Ambiguous) return result;
        }

        // check if we had any invalid characters fall through the attempt to find alternatives
        if (result.Data.Number.Contains('?'))
        {
            // invalid character and didn't guess any alternatives
            result.Data.Status = AccountNumberStatus.Illegible;
            return result;
        }

        // must be a bad number
        result.Data.Status = AccountNumberStatus.Error;
        return result;
    }

    /// <summary>0
    /// Read the contents of OCR file and return a list of account numbers
    /// </summary>
    /// <remarks>
    /// An OCR row consists of 85 characters, 9 characters for each digit (*9) and 4 newlines.
    /// </remarks>
    /// <param name="ocrFileContents"></param>
    /// <returns></returns>
    public List<AccountNumberRow> GetAccountNumbersFromOcrFileContents(string ocrFileContents)
    {
        if (string.IsNullOrEmpty(ocrFileContents)) return new List<AccountNumberRow>(); // ignore empty strings
        var numberOfRows = ocrFileContents.Length / CharactersPerOcrRow;
        var result = new List<AccountNumberRow>();
        for (var ocrRowIndex = 0; ocrRowIndex < numberOfRows; ocrRowIndex++)
        {
            // iterate each row and convert it to an account number
            var ocrRow = ocrFileContents.Substring(ocrRowIndex * CharactersPerOcrRow, CharactersPerOcrRow);
            result.Add(ConvertOcrNumberToAccountNumber(ocrRow));
        }

        return  result;
    }

    /// <summary>
    /// Validates the contents of an OCR file
    /// </summary>
    /// <remarks>
    /// Will check for;
    /// empty/ null string
    /// illegal characters
    /// that the file is divisible by the expected number of characters per row
    /// </remarks>
    /// <param name="ocrFileContents"></param>
    /// <returns></returns>
    public OcrFileValidationResult ValidateOcrFile(string ocrFileContents)
    {
        var result = new OcrFileValidationResult
        {
            IsValid = true
        };

        if (string.IsNullOrEmpty(ocrFileContents))
        {
            result.IsValid = false;
            result.ValidationFailure = "OCR file is empty";
            return result;
        }

        if (!Regex.IsMatch(ocrFileContents, @"^[\|_\s\n]*$"))
        {
            result.IsValid = false;
            result.ValidationFailure = "OCR file contains illegal characters";
            return result;
        }

        if (ocrFileContents.Length % CharactersPerOcrRow != 0)
        {
            result.IsValid = false;
            result.ValidationFailure =
                $"OCR file is not divisible by the expected number of characters per row ({CharactersPerOcrRow})";
            return result;
        }

        return result;
    } 

    /// <summary>
    /// Generates a list of alternative numbers replacing the given list of digits in the given account number
    /// </summary>
    /// <remarks>
    /// If this code fails to generate any alternative numbers will return null
    /// </remarks>
    /// <param name="digits">the list of OCR digits along with their index in the account number</param>
    /// <param name="accountNumber">the original account number as initially  parsed</param>
    /// <returns></returns>
    private List<string>? GuessAlternativeAccountNumbers(Dictionary<int, string> digits, string accountNumber)
    {
        var result = new List<string>();

        // try to guess some alternative numbers
        foreach (var digit in digits)
        {
            // iterate each digit and generate possible valid digits
            var possibleDigits = OcrDigit.GuessOcrDigit(digit.Value);

            if (possibleDigits == null) continue; // didn't find any valid guesses

            foreach (var possibleDigit in possibleDigits)
            {
                // replace the bad digit with the possible valid digit
                var replacementNumber = accountNumber.ToArray();
                replacementNumber[digit.Key] = possibleDigit;
                result.Add(new string(replacementNumber));
            }
        }
        
        return result.Any() ? result : null;
    }

    /// <summary>
    /// Validates the given guesses to see if any are valid and sets the given account row data accordingly
    /// </summary>
    /// <remarks>
    /// If no valid matches the method will return the account row data unchanged
    /// If a single valid match is found the account number will be replaced with the valid match and the status set to "Ok"
    /// If there are multiple valid matches the possible matches will be set and the status set to "AMB"
    /// </remarks>
    /// <param name="data"></param>
    /// <param name="guessedNumbers"></param>
    /// <returns></returns>
    private AccountNumberRow ValidateGuessAndUpdateRow(AccountNumberRow data, List<string> guessedNumbers)
    {
        // Check if any of the guesses are valid account numbers
        var validAccounts = _accountNumberService.GetValidAccountNumbers(guessedNumbers);

        // Didn't find any valid guesses
        if (validAccounts == null || !validAccounts.Any()) return data;

        if (validAccounts.Count == 1)
        {
            // we got a valid match - replace the number with the valid match and return the data
            data.Data.Number = validAccounts.First();
            data.Data.Status = AccountNumberStatus.Ok;
            return data;
        }

        // we got more than one valid guess - return with the possible matches and a state of ambiguous
        data.PossibleMatches = validAccounts;
        data.Data.Status = AccountNumberStatus.Ambiguous;
        return data;
    }
}