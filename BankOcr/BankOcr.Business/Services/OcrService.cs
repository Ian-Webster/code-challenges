using System.Text;
using BankOcr.Business.Enums;

namespace BankOcr.Business.Services;

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
    public byte ConvertOcrDigitToNumber(string ocrDigit)
    {
        var pipeCount = GetPipeCount(ocrDigit);
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

        var segments = ocrDigit.Split("\n");
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
        return row3SegmentPositions == (SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right) ? (byte)6 : (byte)9;
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
    public string ConvertOcrNumberToAccountNumber(string orcRow)
    {
        var rowSegments = orcRow.Split("\n");
        var result = new StringBuilder();
        var digitOcr = new StringBuilder();
        for (var digitIndex = 0; digitIndex < 9; digitIndex++)
        {
            // read a digit from the row, we need i*3 characters from the first three lines
            for (var lineIndex = 0; lineIndex < 3; lineIndex++)
            {
                digitOcr.Append($"{rowSegments[lineIndex].Substring(digitIndex * 3, 3)}\n");
            }
            result.Append(ConvertOcrDigitToNumber(digitOcr.ToString()));
            digitOcr.Clear();
        }

        return result.ToString();
    }

    /// <summary>0
    /// Read the contents of OCR file and return a list of account numbers
    /// </summary>
    /// <remarks>
    /// An OCR row consists of 85 characters, 9 characters for each digit (*9) and 4 newlines.
    /// </remarks>
    /// <param name="ocrFileContents"></param>
    /// <returns></returns>
    public List<string> GetAccountNumbersFromOcrFileContents(string ocrFileContents)
    {
        var numberOfRows = ocrFileContents.Length / CharactersPerOcrRow;
        var result = new List<string>();
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
           _ => throw new ArgumentException("Invalid segment")
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