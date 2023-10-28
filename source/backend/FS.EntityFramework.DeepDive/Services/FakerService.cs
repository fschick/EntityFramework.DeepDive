using Bogus;
using FS.EntityFramework.DeepDive.Core.Configuration;
using FS.EntityFramework.DeepDive.Interfaces;
using System.Collections.Generic;

namespace FS.EntityFramework.DeepDive.Services;

/// <inheritdoc />
public class FakerService : IFakerService
{
    private const string LOCALE = "de";
    private const int SEED = 1538597;
    private readonly Dictionary<DatabaseType, Faker> _fakers = new();

    /// <inheritdoc />
    public Faker this[DatabaseType databaseType] => GetFaker(databaseType);

    private Faker GetFaker(DatabaseType databaseType)
    {
        if (!_fakers.ContainsKey(databaseType))
            _fakers.Add(databaseType, CreateFaker());

        return _fakers[databaseType];
    }

    private static Faker CreateFaker()
    {
        var faker = new Faker(LOCALE) { Random = new Randomizer(SEED) };
        return faker;
    }
}