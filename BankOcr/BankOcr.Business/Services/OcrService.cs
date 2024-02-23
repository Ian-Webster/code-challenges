using System.Text;
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
    /// Converts a single OCR "digit" to a number
    /// </summary>
    /// <remarks>
    /// An OCR digit consists of a matrix of 3 columns by 3 rows.
    /// Each row can have up to 2 pipes and one underscore.
    /// The position of the pipes and underscores in the matrix determine the digit - they are make
    /// a human readable digit as per an LCD alarm clock.
    /// e.g. 3 would look like;
    ///  -
    ///   |
    ///  _
    ///   |
    ///  _ 
    /// </remarks>
    /// <param name="ocrDigit">the digit to convert</param>
    /// <returns>a short 0 - 9</returns>
    public char ConvertOcrDigitToNumber(string ocrDigit)
    {
        var segments = ocrDigit.Split("\n");
        var digit = new OcrDigit
        {
            Top = segments[0].Contains('_'),
            Middle = new HashSet<Positions>(GetSegmentPositions(segments[1])),
            Bottom = new HashSet<Positions>(GetSegmentPositions(segments[2]))
        };

        var result = ParseOcrDigitSegments(digit);

        return result ?? '?';
    }

    /// <summary>
    /// This method is used when ConvertOcrDigitToNumber can't determine the digit
    /// </summary>
    /// <remarks>
    /// The method will try taking and adding a single pipe or underscore to the OCR digit in an attempt to
    /// make it a valid digit.
    /// Will return all possible valid digits
    /// </remarks>
    /// <param name="ocrDigit"></param>
    /// <returns></returns>
    public List<char>? GuessOcrDigit(string ocrDigit)
    {
        // get the segments as they are currently
        var segments = ocrDigit.Split("\n");
        var currentDigit = new OcrDigit
        {
            Top = segments[0].Contains('_'),
            Middle = new HashSet<Positions>(GetSegmentPositions(segments[1])),
            Bottom = new HashSet<Positions>(GetSegmentPositions(segments[2]))
        };

        // generate all possible variations of the current digit - making single changes at a time
        var possibleVariations = GenerateVariations(currentDigit);

        // test each variation to see if it's a valid digit
        var possibleDigits = new List<char>();
        foreach (var variation in possibleVariations)
        {
            var result = ParseOcrDigitSegments(variation);
            if (result != null)
            {
                possibleDigits.Add(result.Value);
            }
        }

        return possibleDigits.Any()? possibleDigits : null;
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
        var rowSegments = orcRow.Split("\n");
        var accountNumber = new StringBuilder();
        var digitOcr = new StringBuilder();
        var digits = new List<(int Index, string DigitOcr, bool IsBad)>();
        for (var digitIndex = 0; digitIndex < 9; digitIndex++)
        {
            // read a digit from the row, we need i*3 characters from the first three lines
            for (var lineIndex = 0; lineIndex < 3; lineIndex++)
            {
                digitOcr.Append($"{rowSegments[lineIndex].Substring(digitIndex * 3, 3)}\n");
            }
            var ocrString = digitOcr.ToString();
            var parsedDigit = ConvertOcrDigitToNumber(ocrString);
            digits.Add((digitIndex, digitOcr.ToString(), parsedDigit == '?'));
            accountNumber.Append(parsedDigit);
            digitOcr.Clear();
        }

        var result = new AccountNumberRow
        {
            Data = new AccountNumber
            {
                Number = accountNumber.ToString(), 
                Status = AccountNumberStatus.Error // assume error as the default status
            }
        };

        List<string>? guessedNumbers;

        if (digits.Any(d => d.IsBad))
        {
            guessedNumbers = GuessAlternativeAccountNumbers(digits
                .Where(d => d.IsBad)
                .ToDictionary(key => key.Index, v => v.DigitOcr), 
                result.Data.Number);

            if (guessedNumbers != null)
            {
                // we managed to make some guesses
                result = ValidateGuessAndUpdateRow(result, guessedNumbers);

                // we found a single valid guess
                if (result.Data.Status == AccountNumberStatus.Ok) return result;

                // we found multiple matches
                if (result.Data.Status == AccountNumberStatus.Ambiguous) return result;
            }
        }

        // we either didn't have any bad digits or we couldn't guess any valid replacements

        // check if we had any invalid characters fall through the attempt to find alternatives
        if (result.Data.Number.Contains('?'))
        {
            // invalid character and didn't guess any alternatives
            result.Data.Status = AccountNumberStatus.Illegible;
            return result;
        }

        // check if the account number is valid - if it is return
        if (_accountNumberService.AccountNumberIsValid(result.Data.Number)) return result;

        // account number is invalid
        // have a go at guessing some alternatives and see if any of them are valid
        guessedNumbers = GuessAlternativeAccountNumbers(digits.ToDictionary(key => key.Index, v => v.DigitOcr), result.Data.Number);
        if (guessedNumbers != null)
        {
            // we managed to make some guesses
            result = ValidateGuessAndUpdateRow(result, guessedNumbers);

            // we found a single valid guess
            if (result.Data.Status == AccountNumberStatus.Ok) return result;

            // we found multiple valid guesses
            if (result.Data.Status == AccountNumberStatus.Ambiguous) return result;
        }

        // failed to guess any valid alternatives, return what we have
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
        var numberOfRows = ocrFileContents.Length / CharactersPerOcrRow;
        var result = new List<AccountNumberRow>();
        for (var ocrRowIndex = 0; ocrRowIndex < numberOfRows; ocrRowIndex++)
        {
            var ocrRow = ocrFileContents.Substring(ocrRowIndex * CharactersPerOcrRow, CharactersPerOcrRow);
            result.Add(ConvertOcrNumberToAccountNumber(ocrRow));
        }

        return  result;
    }

    /// <summary>
    /// Returns the segment positions for a given segment
    /// </summary>
    /// <param name="segment">segment to check</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private List<Positions> GetSegmentPositions(string segment)
    {
       return segment switch
       {
           "|  " => new List<Positions> { Positions.Left },
           " _ " => new List<Positions> { Positions.Bottom },
           "  |" => new List<Positions> { Positions.Right },
           "|_ " => new List<Positions> { Positions.Left, Positions.Bottom },
           "|_|" => new List<Positions> { Positions.Left, Positions.Bottom, Positions.Right },
           " _|" => new List<Positions> { Positions.Bottom, Positions.Right },
           "| |" => new List<Positions> { Positions.Left, Positions.Right },
           _ => new List<Positions> { Positions.None }
       };
    }

    /// <summary>
    /// Parses the combination of segments set on the given digit and returns the matching number
    /// </summary>
    /// <remarks>
    /// If the code fails to match to any digit will return null
    /// </remarks>
    /// <param name="digit"></param>
    /// <returns></returns>
    private char? ParseOcrDigitSegments(OcrDigit digit)
    {
        // zero
        if (digit.Top
            && digit.MiddleContains(Positions.Left, Positions.Right)
            && digit.BottomContains(Positions.Left, Positions.Bottom, Positions.Right))
        {
            return '0';
        }

        // one
        if (digit.Top == false
            && digit.MiddleContains(Positions.Right)
            && digit.BottomContains(Positions.Right))
        {
            return '1';
        }

        // two
        if (digit.Top
            && digit.MiddleContains(Positions.Bottom, Positions.Right)
            && digit.BottomContains(Positions.Left, Positions.Bottom))
        {
            return '2';
        }

        // three
        if (digit.Top
            && digit.MiddleContains(Positions.Bottom, Positions.Right)
            && digit.BottomContains(Positions.Bottom, Positions.Right))
        {
            return '3';
        }

        // four
        if (digit.Top == false
            && digit.MiddleContains(Positions.Left, Positions.Bottom, Positions.Right)
            && digit.BottomContains(Positions.Right))
        {
            return '4';
        }

        // five
        if (digit.Top
            && digit.MiddleContains(Positions.Left, Positions.Bottom)
            && digit.BottomContains(Positions.Bottom, Positions.Right))
        {
            return '5';
        }

        // six
        if (digit.Top
            && digit.MiddleContains(Positions.Left, Positions.Bottom)
            && digit.BottomContains(Positions.Left, Positions.Bottom, Positions.Right))
        {
            return '6';
        }

        // seven
        if (digit.Top
            && digit.MiddleContains(Positions.Right)
            && digit.BottomContains(Positions.Right))
        {
            return '7';
        }

        // eight
        if (digit.Top
            && digit.MiddleContains(Positions.Left, Positions.Bottom, Positions.Right)
            && digit.BottomContains(Positions.Left, Positions.Bottom, Positions.Right))
        {
            return '8';
        }

        // nine
        if (digit.Top
            && digit.MiddleContains(Positions.Left, Positions.Bottom, Positions.Right)
            && digit.BottomContains(Positions.Bottom, Positions.Right))
        {
            return '9';
        }

        // invalid digit
        return null;
    }

    /// <summary>
    /// Generates possible variations for the given digit
    /// </summary>
    /// <remarks>
    /// The variations will only ever by a single change from the original digit
    /// Changes can be additions or subtractions to any one of the segments
    /// </remarks>
    /// <param name="original"></param>
    /// <returns></returns>
    private List<OcrDigit> GenerateVariations(OcrDigit original)
    {
        var variations = new List<OcrDigit>
        {
            // Generate variations for the top segment
            // The top segment can only ever have Positions.None or Positions.Bottom set
            // So we can just flip the value here
            new() { Top = !original.Top, Middle = original.Middle, Bottom = original.Bottom }
        };

        // Middle and bottom segments can have any combination of left, right and bottom segments set
        // So we need to generate all possible combinations of the segments

        // Generate variations for the middle segment
        foreach (Positions position in Enum.GetValues(typeof(Positions)))
        {
            if (position == Positions.None) continue; // changing the segment to none won't yield a valid digit so we can skip it

            var newMiddle = new HashSet<Positions>(original.Middle);

            if (newMiddle.Contains(position))
            {
                newMiddle.Remove(position);
            }
            else
            {
                newMiddle.Add(position);
            }

            variations.Add(new OcrDigit { Top = original.Top, Middle = newMiddle, Bottom = original.Bottom });
        }

        // Generate variations for the bottom segment
        foreach (Positions position in Enum.GetValues(typeof(Positions)))
        {
            if (position == Positions.None) continue;

            var newBottom = new HashSet<Positions>(original.Bottom);
            if (newBottom.Contains(position))
            {
                newBottom.Remove(position);
            }
            else
            {
                newBottom.Add(position);
            }

            variations.Add(new OcrDigit { Top = original.Top, Middle = original.Middle, Bottom = newBottom });
        }

        return variations;
    }

    /// <summary>
    /// Generates a list of alternative numbers replacing the given list of digits in the given account number
    /// </summary>
    /// <remarks>
    /// If this code fails to generate any alternative numbers will return null
    /// </remarks>
    /// <param name="digits"></param>
    /// <param name="accountNumber"></param>
    /// <returns></returns>
    private List<string>? GuessAlternativeAccountNumbers(Dictionary<int, string> digits, string accountNumber)
    {
        var result = new List<string>();

        // try to guess some alternative numbers
        foreach (var digit in digits)
        {
            // iterate each digit and generate possible valid digits
            var possibleDigits = GuessOcrDigit(digit.Value);

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