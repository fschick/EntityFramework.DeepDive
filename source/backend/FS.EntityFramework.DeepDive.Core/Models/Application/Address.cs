namespace FS.EntityFramework.DeepDive.Core.Models.Application;

/// <summary>
/// A postal address.
/// </summary>
public class Address
{
    /// <summary>
    /// Street of the address.
    /// </summary>
    public required string Street { get; set; }

    /// <summary>
    /// Postal code of the address.
    /// </summary>
    public required string PostalCode { get; set; }

    /// <summary>
    /// City of the address.
    /// </summary>
    public required string City { get; set; }
}