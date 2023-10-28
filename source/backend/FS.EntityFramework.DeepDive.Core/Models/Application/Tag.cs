using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.EntityFramework.DeepDive.Core.Models.Application;

/// <summary>
/// Represents a tag.
/// </summary>
public class Tag
{
    /// <summary>
    /// The unique identifier for the tag.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    /// The blogs associated with the tag.
    /// </summary>
    public List<Blog>? Blogs { get; set; }
}