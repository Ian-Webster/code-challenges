using System.Text;
using BankOcr.Business.Enums;
using BankOcr.Business.Models;

namespace BankOcr.Business.Services;

/// <summary>
/// Handles converting OCR data to account numbers
/// </summary>
public class OcrService
{
    public const byte CharactersPerOcrRow = 85;

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
        var segmentPosition0 = GetSegmentPositions(segments[0]);
        var segmentPosition1 = GetSegmentPositions(segments[1]);
        var segmentPosition2 = GetSegmentPositions(segments[2]);

        // zero
        if (segmentPosition0 == SegmentPositions.Bottom 
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Right)
            && segmentPosition2 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '0';
        }

        // one
        if (segmentPosition0 == SegmentPositions.None
            && segmentPosition1 == SegmentPositions.Right
            && segmentPosition2 == SegmentPositions.Right)
        {
            return '1';
        }

        // two
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Bottom | SegmentPositions.Right)
            && segmentPosition2 == (SegmentPositions.Left | SegmentPositions.Bottom))
        {
            return '2';
        }

        // three
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Bottom | SegmentPositions.Right)
            && segmentPosition2 == (SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '3';
        }

        // four
        if (segmentPosition0 == SegmentPositions.None
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right)
            && segmentPosition2 == SegmentPositions.Right)
        {
            return '4';
        }

        // five
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Bottom)
            && segmentPosition2 == (SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '5';
        }

        // six
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Bottom)
            && segmentPosition2 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '6';
        }

        // seven
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == SegmentPositions.Right
            && segmentPosition2 == SegmentPositions.Right)
        {
            return '7';
        }

        // eight
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right)
            && segmentPosition2 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '8';
        }

        // nine
        if (segmentPosition0 == SegmentPositions.Bottom
            && segmentPosition1 == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right)
            && segmentPosition2 == (SegmentPositions.Bottom | SegmentPositions.Right))
        {
            return '9';
        }

        // unknown / invalid character
        return '?';

        // leaving this old code here as it might help with the guessing requirement later
        /*var pipeCount = GetPipeCount(ocrDigit);
        var underscoreCount = GetUnderscoreCount(ocrDigit);

        // there are four pipes - must be either 0 or 8
        if (pipeCount == 4)
        {
            return (byte) (underscoreCount == 3 ? 8 : 0);
        }

        // two pipes and a single underscore must be 7
        if (pipeCount == 2 && underscoreCount == 1) return 7;

        // if the second character isn't a pipe must be  1 or 4
        if (ocrDigit[1] != '_')
        {
            return (byte)(pipeCount == 2 ? 1 : 4);
        }

        // for the remaining digits we have to check the segments, counting pipes / underscores isn't
        // enough to establish which digit we might have


        var row2SegmentPositions = GetSegmentPositions(segments[1]);
        var row3SegmentPositions = GetSegmentPositions(segments[2]);
        if (pipeCount == 2 && underscoreCount == 3)
        {
            // must be 2, 3 or 5
            if (row2SegmentPositions == (SegmentPositions.Bottom | SegmentPositions.Right)
                && row3SegmentPositions == (SegmentPositions.Bottom | SegmentPositions.Right))
            {
                // must be three
                return 3;
            }

            // must be 2 or 5
            return row2SegmentPositions == (SegmentPositions.Bottom | SegmentPositions.Right) ? (byte)2 : (byte)5;
        }

        // only 6 and 9 left
        // if the bottom segment has all three positions it must be 6
        return row3SegmentPositions == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right) ? (byte)6 : (byte)9;*/
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
        result.Status = result.Number.Contains('?') ? "ILL" : null;
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
    private SegmentPositions GetSegmentPositions(string segment)
    {
       return segment switch
       {
           "|  " => SegmentPositions.Left,
           " _ " => SegmentPositions.Bottom,
           "  |" => SegmentPositions.Right,
           "|_ " => SegmentPositions.Left | SegmentPositions.Bottom,
           "|_|" => SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right,
           " _|" => SegmentPositions.Bottom | SegmentPositions.Right,
           "| |" => SegmentPositions.Left | SegmentPositions.Right,
           _ => SegmentPositions.None
       };
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