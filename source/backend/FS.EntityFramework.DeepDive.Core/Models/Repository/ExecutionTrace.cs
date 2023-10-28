using System.Collections.Generic;

namespace FS.EntityFramework.DeepDive.Core.Models.Repository;

/// <summary>
/// Execution trace of a service call.
/// </summary>
public class ExecutionTrace
{
    /// <summary>
    /// Source code of the top-level method of the service call.
    /// </summary>
    public string? Sourcecode { get; set; }

    /// <summary>
    /// Database traces of the service call.
    /// </summary>
    public List<DatabaseTrace> DatabaseTraces { get; set; } = [];
}