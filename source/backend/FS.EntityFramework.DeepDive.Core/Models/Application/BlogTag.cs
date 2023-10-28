using System;

namespace FS.EntityFramework.DeepDive.Core.Models.Application;

/// <summary>
/// Relational table between <see cref="Blog"/> and <see cref="Tag"/>.
/// </summary>
public class BlogTag
{
    /// <summary>
    /// The unique identifier for <see cref="Blog"/>.
    /// </summary>
    public Guid BlogId { get; set; }

    /// <summary>
    /// The unique identifier for <see cref="Tag"/>.
    /// </summary>
    public Guid TagId { get; set; }
}