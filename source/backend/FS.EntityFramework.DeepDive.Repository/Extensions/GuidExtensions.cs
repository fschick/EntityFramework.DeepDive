using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FS.EntityFramework.DeepDive.Repository.Extensions;

public static class MyDbFunctionsExtensions
{
    private static readonly MethodInfo _guidLike = typeof(MyDbFunctionsExtensions).GetMethod(nameof(Like), new[] { typeof(Guid), typeof(string) })!;
    private static readonly MethodInfo _nullableGuidLike = typeof(MyDbFunctionsExtensions).GetMethod(nameof(Like), new[] { typeof(Guid?), typeof(string) })!;

    public static bool Like(this Guid guid, string? pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return false;

        return Regex.IsMatch(guid.ToString(), pattern.ToRegexPattern());
    }

    public static bool Like(this Guid? guid, string? pattern)
    {
        var bothParametersAreEmpty = guid == null && string.IsNullOrEmpty(pattern);
        if (bothParametersAreEmpty)
            return true;

        var oneParameterIsEmpty = guid == null || string.IsNullOrEmpty(pattern);
        if (oneParameterIsEmpty)
            return false;

        return Regex.IsMatch(guid!.Value.ToString(), pattern!.ToRegexPattern());
    }

    public static void RegisterGuidLikeFunction(this ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDbFunction(_guidLike)
            .HasTranslation(CreateLikeExpression);

        modelBuilder
            .HasDbFunction(_nullableGuidLike)
            .HasTranslation(CreateLikeExpression);
    }

    private static SqlExpression CreateLikeExpression(IReadOnlyList<SqlExpression> parameters)
        => new LikeExpression(parameters[0], parameters[1], null, null);

    private static string ToRegexPattern(this string pattern)
        => pattern.Replace("%", ".*").Replace("_", ".");
}