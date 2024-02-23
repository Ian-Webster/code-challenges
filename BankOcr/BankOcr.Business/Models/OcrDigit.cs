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
}