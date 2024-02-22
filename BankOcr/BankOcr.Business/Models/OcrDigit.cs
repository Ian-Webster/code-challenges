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
    public HashSet<Positions> Middle { get; set; }

    /// <summary>
    /// Bottom can have any combination of left, right and bottom segments set.
    /// </summary>
    public HashSet<Positions> Bottom { get; set; }

    public void SetMiddle(params Positions[] positions)
    {
        foreach (var position in positions)
        {
            Middle.Add(position);
        }
    }

    public void SetBottom(params Positions[] positions)
    {
        foreach (var position in positions)
        {
            Bottom.Add(position);
        }
    }

    public bool MiddleContains(params Positions[] positions)
    {
        return Middle.SetEquals(positions);
        //return positions.All(position => Middle.Contains(position));
    }

    public bool BottomContains(params Positions[] positions)
    {
        return Bottom.SetEquals(positions);
        //return positions.All(position => Bottom.Contains(position));
    }
}