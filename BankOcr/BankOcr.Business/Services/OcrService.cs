using System.Linq;
using BankOcr.Business.Enums;

namespace BankOcr.Business.Services;

public class OcrService
{
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
    /// <param name="ocr">the digit to convert</param>
    /// <returns>a short 0 - 9</returns>
    public byte ConvertOcrDigitToNumber(string ocr)
    {
        var pipeCount = GetPipeCount(ocr);
        var underscoreCount = GetUnderscoreCount(ocr);

        // there are four pipes - must be either 0 or 8
        if (pipeCount == 4)
        {
            return (byte) (underscoreCount == 3 ? 8 : 0);
        }

        // two pipes and a single underscore must be 7
        if (pipeCount == 2 && underscoreCount == 1) return 7;

        // if the second character isn't a pipe must be  1 or 4
        if (ocr[1] != '_')
        {
            return (byte)(pipeCount == 2 ? 1 : 4);
        }

        // for the remaining digits we have to check the segments, counting pipes / underscores isn't 
        // enough to establish which digit we might have

        var segments = ocr.Split("\n");
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

    private SegmentPositions GetSegmentPositions(string segment)
    {
       return segment switch
       {
           " _ " => SegmentPositions.Bottom,
           "  |" => SegmentPositions.Right,
           "|  " => SegmentPositions.Left,
           "|_ " => SegmentPositions.Left | SegmentPositions.Bottom,
           "|_|" => SegmentPositions.Left | SegmentPositions.Bottom | SegmentPositions.Right,
           " _|" => SegmentPositions.Bottom | SegmentPositions.Right,
           _ => throw new ArgumentException("Invalid segment")
       };
    }

    private int GetPipeCount(string ocr)
    {
        return ocr.Count(c => c == '|');
    }

    private int GetUnderscoreCount(string ocr)
    {
        return ocr.Count(c => c == '_');
    }
}