using FS.EntityFramework.DeepDive.Core.Configuration;
using FS.EntityFramework.DeepDive.Core.Models.Application;
using FS.EntityFramework.DeepDive.Repository.Extensions;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Repository.DbContexts;

public sealed class DeepDiveDbContext : MultiDbContext
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ISqlTraceService _sqlTraceService;
    private readonly TableLockInterceptor? _tableLockInterceptor;

    public DbSet<Blog> Blogs { get; set; } = default!;

    public DbSet<Author> Authors { get; set; } = default!;

    public DbSet<Tag> Tags { get; set; } = default!;

    public IReadOnlyCollection<string> ExecutedSqlCommands
        => _sqlTraceService.ExecutedSqlCommands;

    public DeepDiveDbContext(DatabaseType databaseType, string connectionString, IOptions<DeepDiveConfiguration> configuration, IWebHostEnvironment environment, ILoggerFactory loggerFactory, ISqlTraceService sqlTraceService, TableLockInterceptor? tableLockInterceptor)
        : base(databaseType, connectionString)
    {
        _environment = environment;
        _loggerFactory = loggerFactory;
        _sqlTraceService = sqlTraceService;
        _tableLockInterceptor = tableLockInterceptor;
    }

    #region EF Core CLI stuff
    public DeepDiveDbContext(IOptions<DeepDiveConfiguration> configuration, IWebHostEnvironment environment, ILoggerFactory loggerFactory, ISqlTraceService sqlTraceService)
        : base(GetDatabaseType(), GetConnectionString(configuration))
    {
        _environment = environment;
        _loggerFactory = loggerFactory;
        _sqlTraceService = sqlTraceService;
    }
    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        RegisterTableLockInterceptor(optionsBuilder);
        if (DatabaseType == DatabaseType.Sqlite)
            optionsBuilder.RegisterObfuscateFunction(DatabaseType);

        ConfigureLogging(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.RegisterGuidLikeFunction();
        if (DatabaseType != DatabaseType.Sqlite)
            modelBuilder.RegisterObfuscateFunction(DatabaseType);
        ConfigureBlog(modelBuilder.Entity<Blog>());
        ConfigureAuthor(modelBuilder.Entity<Author>());
    }

    public override int SaveChanges()
    {
        HandleBlogCreated();
        HandleBlogStatus();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleBlogCreated();
        HandleBlogStatus();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleBlogCreated();
        HandleBlogStatus();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        HandleBlogCreated();
        HandleBlogStatus();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void HandleBlogCreated()
    {
        var trackedBlogs = ChangeTracker.Entries<Blog>().ToList();
        foreach (var blog in trackedBlogs)
        {
            switch (blog.State)
            {
                case EntityState.Added:
                    blog.Entity.Created = DateTime.UtcNow;
                    break;
                default:
                    blog.Property(x => x.Created).IsModified = false;
                    break;
            }
        }
    }

    private void HandleBlogStatus()
    {
        var blogsWithChangesStatus = ChangeTracker
            .Entries<Blog>()
            .Where(blog => blog.Property(x => x.Status).IsModified)
            .ToList();

        foreach (var blog in blogsWithChangesStatus)
        {
            switch (blog.Entity.Status)
            {
                case BlogStatus.Draft:
                    blog.Entity.Published = null;
                    break;
                case BlogStatus.Published:
                    blog.Entity.Published = DateTime.UtcNow;
                    break;
            }
        }
    }

    #region Model configuration
    private static void ConfigureBlog(EntityTypeBuilder<Blog> blogBuilder)
    {
        blogBuilder
            .HasOne(blog => blog.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        blogBuilder
            .HasMany(blog => blog.Tags)
            .WithMany(tag => tag.Blogs)
            .UsingEntity<BlogTag>(
                builder => builder.HasOne<Tag>().WithMany().HasForeignKey(x => x.TagId),
                builder => builder.HasOne<Blog>().WithMany().HasForeignKey(x => x.BlogId)
            );

        blogBuilder
            .Property(x => x.Status)
            .HasDefaultValue(BlogStatus.Draft);

        var enumToStringConverter = new EnumToStringConverter<BlogStatus>();
        blogBuilder
            .Property(e => e.Status)
            .HasConversion(enumToStringConverter);

        // Conversion can also be done with lambdas:
        // blogBuilder
        //    .Property(e => e.Status)
        //    .HasConversion(
        //        blogStatus => blogStatus.ToString(),
        //        valueString => Enum.Parse<BlogStatus>(valueString)
        //    );

        // Or using a built-in converter:
        // blogBuilder
        //    .Property(x => x.Status)
        //    .HasConversion<string>();
    }

    private void ConfigureAuthor(EntityTypeBuilder<Author> authorBuilder)
    {
        authorBuilder
            .OwnsOne(
                author => author.Address,
                navigationBuilder =>
                {
                    if (DatabaseType != DatabaseType.MySql)
                        navigationBuilder.ToJson();
                });
    }
    #endregion Model configuration

    private void ConfigureLogging(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .EnableSensitiveDataLogging(_environment.IsDevelopment())
            .EnableDetailedErrors(_environment.IsDevelopment())
            .UseLoggerFactory(_loggerFactory)
            .AddInterceptors(_sqlTraceService);
    }

    private void RegisterTableLockInterceptor(DbContextOptionsBuilder optionsBuilder)
    {
        if (DatabaseType == DatabaseType.SqlServer && _tableLockInterceptor != null)
            optionsBuilder.AddInterceptors(_tableLockInterceptor);
    }
}