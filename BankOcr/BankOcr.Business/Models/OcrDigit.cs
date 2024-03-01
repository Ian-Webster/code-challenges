using BankOcr.Business.Enums;

namespace BankOcr.Business.Models;

/// <summary>
/// Models an individual OCR digit.
/// </summary>
/// <remarks>
/// A digit is represented by a 3x3 grid of segments. Each segment can be on or off.
/// The top row can only ever have it's bottom set.
/// The middle and bottom rows can have left, right and bottom segments set - any combination of these including all one at once
/// </remarks>
public class OcrDigit
{
    /// <summary>
    /// Top can only ever by true or false, true indicates the bottom segment is set.
    /// </summary>
    public bool Top { get; set; }

    /// <summary>
    /// Middle can have any combination of left, right and bottom segments set.
    /// </summary>
    public HashSet<Positions> Middle { get; set; } = new HashSet<Positions>();

    /// <summary>
    /// Bottom can have any combination of left, right and bottom segments set.
    /// </summary>
    public HashSet<Positions> Bottom { get; set; } = new HashSet<Positions>();

    /// <summary>
    /// Determines if the middle segment contains all of the specified positions.
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    public bool MiddleContains(params Positions[] positions)
    {
        return Middle.SetEquals(positions);
    }

    /// <summary>
    /// Determines if the bottom segment contains all of the specified positions.
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    public bool BottomContains(params Positions[] positions)
    {
        return Bottom.SetEquals(positions);
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
    public static char ConvertOcrDigitToNumber(string ocrDigit)
    {
        var segments = ocrDigit.Split("\n");
        var digit = new OcrDigit
        {
            Top = segments[0].Contains('_'), // top segment can only have (or not have) an underscore
            Middle = new HashSet<Positions>(GetSegmentPositions(segments[1])),
            Bottom = new HashSet<Positions>(GetSegmentPositions(segments[2]))
        };

        var result = ParseOcrDigitSegments(digit);

        return result ?? '?'; // if the result is null we couldn't determine the digit, return illegal character instead
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
    public static List<char>? GuessOcrDigit(string ocrDigit)
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
                // digit was valid, add it to the list
                possibleDigits.Add(result.Value);
            }
        }

        // return any valid digits we found otherwise if we found none return null
        return possibleDigits.Any() ? possibleDigits : null;
    }

    /// <summary>
    /// Parses the combination of segments set on the given digit and returns the matching number
    /// </summary>
    /// <remarks>
    /// If the code fails to match to any digit will return null
    /// </remarks>
    /// <param name="digit"></param>
    /// <returns></returns>
    private static char? ParseOcrDigitSegments(OcrDigit digit)
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
    /// Returns the segment positions for a given segment
    /// </summary>
    /// <param name="segment">segment to check</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static List<Positions> GetSegmentPositions(string segment)
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
    /// Generates possible variations for the given digit
    /// </summary>
    /// <remarks>
    /// The variations will only ever by a single change from the original digit
    /// Changes can be additions or subtractions to any one of the segments
    /// </remarks>
    /// <param name="original"></param>
    /// <returns></returns>
    private static List<OcrDigit> GenerateVariations(OcrDigit original)
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

        // Generate variations for the middle and bottom segments
        foreach (Positions position in Enum.GetValues(typeof(Positions)))
        {
            if (position == Positions.None) continue; // changing the segment to none won't yield a valid digit so we can skip it

            var newMiddle = new HashSet<Positions>(original.Middle);

            // if the segment is already set we remove it, if it's not set we add it
            if (newMiddle.Contains(position))
            {
                newMiddle.Remove(position);
            }
            else
            {
                newMiddle.Add(position);
            }

            // add the new variation to the list
            variations.Add(new OcrDigit { Top = original.Top, Middle = newMiddle, Bottom = original.Bottom });

            // repeat the process for the bottom segment
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
}