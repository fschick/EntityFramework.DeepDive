using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FS.EntityFramework.DeepDive.Core.Models.Application;

/// <summary>
/// Represents a blog.
/// </summary>
public class Blog
{
    /// <summary>
    /// Unique identifier for the blog.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Title of the blog.
    /// </summary>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Unique identifier for the author of the blog.
    /// </summary>
    [Required]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Author of the blog.
    /// </summary>
    public Author? Author { get; set; }

    /// <summary>
    /// Tags associated with the blog.
    /// </summary>
    public List<Tag> Tags { get; set; } = [];

    /// <summary>
    /// Status of the blog.
    /// </summary>
    public BlogStatus Status { get; set; }

    /// <summary>
    /// Date and time when the blog was published.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Date and time when the blog was last edited.
    /// </summary>
    public DateTime? Published { get; set; }
}