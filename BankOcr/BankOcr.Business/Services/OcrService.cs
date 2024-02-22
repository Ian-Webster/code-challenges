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
    public List<char> GuessOcrDigit(string ocrDigit)
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

        return possibleDigits;
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
    public AccountNumber ConvertOcrNumberToAccountNumber(string orcRow)
    {
        var rowSegments = orcRow.Split("\n");
        var accountNumber = new StringBuilder();
        var digitOcr = new StringBuilder();
        for (var digitIndex = 0; digitIndex < 9; digitIndex++)
        {
            // read a digit from the row, we need i*3 characters from the first three lines
            for (var lineIndex = 0; lineIndex < 3; lineIndex++)
            {
                digitOcr.Append($"{rowSegments[lineIndex].Substring(digitIndex * 3, 3)}\n");
            }
            accountNumber.Append(ConvertOcrDigitToNumber(digitOcr.ToString()));
            digitOcr.Clear();
        }

        var result = new AccountNumber { Number = accountNumber.ToString() };
        if (result.Number.Contains('?'))
        {
            result.Status = "ILL";
            return result;
        }

        if (_accountNumberService.AccountNumberIsValid(result.Number)) return result;

        result.Status = "ERR";
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
    public List<AccountNumber> GetAccountNumbersFromOcrFileContents(string ocrFileContents)
    {
        var numberOfRows = ocrFileContents.Length / CharactersPerOcrRow;
        var result = new List<AccountNumber>();
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

    public List<OcrDigit> GenerateVariations(OcrDigit original)
    {
        var variations = new List<OcrDigit>
        {
            // Generate variations for the top segment
            new OcrDigit { Top = !original.Top, Middle = original.Middle, Bottom = original.Bottom }
        };

        // Generate variations for the middle segment
        foreach (Positions position in Enum.GetValues(typeof(Positions)))
        {
            if (position == Positions.None) continue;

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
    /// Gets the count of pipes in the OCR string
    /// </summary>
    /// <param name="ocr"></param>
    /// <returns></returns>
    private int GetPipeCount(string ocr)
    {
        return ocr.Count(c => c == '|');
    }

    /// <summary>
    /// Gets the count of underscores in the OCR string
    /// </summary>
    /// <param name="ocr"></param>
    /// <returns></returns>
    private int GetUnderscoreCount(string ocr)
    {
        return ocr.Count(c => c == '_');
    }
}