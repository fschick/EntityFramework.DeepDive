using System;
using System.ComponentModel.DataAnnotations;

namespace FS.EntityFramework.DeepDive.Core.Models.Application;

/// <summary>
/// Represents an author.
/// </summary>
public class Author
{
    /// <summary>
    /// The unique identifier of the author.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the author.
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// The address of the author.
    /// </summary>
    public Address? Address { get; set; }
}