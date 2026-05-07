# STX.EFCore.Client

A general-purpose EF Core client for common data operations, designed for use in a Storage Broker.

## Main Features

| Method | Description |
|---|---|
| `InsertAsync` | Inserts a single entity and returns it detached. |
| `SelectAllAsync` | Returns an `IQueryable<T>` of all entities of the given type. |
| `SelectAsync` | Finds and returns a single entity by its primary key(s). |
| `UpdateAsync` | Updates a single entity and returns it detached. |
| `DeleteAsync` | Deletes a single entity and returns it detached. |
| `BulkInsertAsync` | Inserts a collection of entities, optionally within a transaction. |
| `BulkReadAsync` | Reads a collection of entities by matching against the provided objects. |
| `BulkUpdateAsync` | Updates a collection of entities, optionally within a transaction. |
| `BulkDeleteAsync` | Deletes a collection of entities, optionally within a transaction. |
| `BulkUpsertAsync` | Inserts new entities and updates existing ones in a single batch, optionally within a transaction. Existence is determined by primary key. |
| `ExistsAsync` | Returns `true` if an entity with the supplied primary key(s) exists in the data store. |

All methods accept an optional `CancellationToken`. The bulk write operations (`BulkInsertAsync`, `BulkUpdateAsync`, `BulkDeleteAsync`, `BulkUpsertAsync`) also accept a `useTransaction` flag (defaults to `true`) that wraps the operation in a database transaction and rolls back automatically on failure.

## Exception Behaviour

`EFCoreClient` does not wrap exceptions. All EF Core and .NET exceptions propagate directly to the caller, giving consumers full visibility and control:

| Exception | When thrown |
|---|---|
| `ArgumentNullException` | A required argument (`object`, `objectIds`, or `objects` collection) is `null`. |
| `DbUpdateException` | The database rejects a write - e.g. unique constraint, foreign key violation, or `NOT NULL` failure. |
| `DbUpdateConcurrencyException` | A concurrency conflict is detected on update or delete (subclass of `DbUpdateException`). |
| `InvalidOperationException` | The entity type is not registered in the `DbContext` model, or an incompatible primary key is supplied. |
| `OperationCanceledException` | The operation was cancelled via the `CancellationToken`. |

## Thread Safety

`EFCoreClient` is **not thread-safe**. All operations on a single instance share the same underlying `DbContext`, which is not designed for concurrent access.

Create one `EFCoreClient` instance **per request or per scope** — not as a singleton. In an ASP.NET Core application this means constructing it inside your scoped `StorageBroker`, which is itself registered as a scoped service:

```cs
// ✅ Correct — one EFCoreClient per scope via the scoped StorageBroker
services.AddDbContext<StorageBroker>();  // scoped by default

// ❌ Wrong — a singleton EFCoreClient shared across all requests
services.AddSingleton<IEFCoreClient>(new EFCoreClient(myDbContext));
```

This is standard EF Core behaviour. See [DbContext Lifetime, Configuration, and Initialization](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) for details.

## How do I use this?

### Before — manual broker implementation

A storage broker previously required manually managing `EntityState` for every operation:

```cs
public partial class StorageBroker : EFxceptionsContext, IStorageBroker
{
    private readonly IConfiguration configuration;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AddConfigurations(modelBuilder);
    }

    private static void AddConfigurations(ModelBuilder modelBuilder)
    {
        . . .
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        . . .
    }

    private async ValueTask<T> InsertAsync<T>(T @object)
        where T : class
    {
        this.Entry(@object).State = EntityState.Added;
        await this.SaveChangesAsync();
        this.Entry(@object).State = EntityState.Detached;

        return @object;
    }

    private async ValueTask<IQueryable<T>> SelectAllAsync<T>()
        where T : class =>
            this.Set<T>();

    private async ValueTask<T> SelectAsync<T>(params object[] @objectIds)
        where T : class =>
            await this.FindAsync<T>(objectIds);

    private async ValueTask<T> UpdateAsync<T>(T @object)
        where T : class
    {
        this.Entry(@object).State = EntityState.Modified;
        await this.SaveChangesAsync();
        this.Entry(@object).State = EntityState.Detached;

        return @object;
    }

    private async ValueTask<T> DeleteAsync<T>(T @object)
        where T : class
    {
        this.Entry(@object).State = EntityState.Deleted;
        await this.SaveChangesAsync();
        this.Entry(@object).State = EntityState.Detached;

        return @object;
    }
}
```

### After — using `EFCoreClient`

Pass `this` (your `DbContext`) to the `EFCoreClient` constructor and delegate all operations to it. This eliminates manual state management, adds structured exception handling, cancellation support, and bulk operation support out of the box:

```cs
public partial class StorageBroker : EFxceptionsContext, IStorageBroker
{
    private readonly IConfiguration configuration;
    private readonly IEFCoreClient efCoreClient;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.Database.Migrate();
        this.efCoreClient = new EFCoreClient(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AddConfigurations(modelBuilder);
    }

    private static void AddConfigurations(ModelBuilder modelBuilder)
    {
        . . .
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        . . .
    }

    private async ValueTask<T> InsertAsync<T>(T @object, CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.InsertAsync(@object, cancellationToken);

    private async ValueTask<IQueryable<T>> SelectAllAsync<T>(CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.SelectAllAsync<T>(cancellationToken);

    private async ValueTask<T> SelectAsync<T>(object[] objectIds, CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.SelectAsync<T>(objectIds, cancellationToken);

    private async ValueTask<T> UpdateAsync<T>(T @object, CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.UpdateAsync(@object, cancellationToken);

    private async ValueTask<T> DeleteAsync<T>(T @object, CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.DeleteAsync(@object, cancellationToken);

    private async ValueTask BulkInsertAsync<T>(
        IEnumerable<T> objects,
        bool useTransaction = true,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.BulkInsertAsync(objects, useTransaction, cancellationToken);

    private async ValueTask<IEnumerable<T>> BulkReadAsync<T>(
        IEnumerable<T> objects,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.BulkReadAsync(objects, cancellationToken);

    private async ValueTask BulkUpdateAsync<T>(
        IEnumerable<T> objects,
        bool useTransaction = true,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.BulkUpdateAsync(objects, useTransaction, cancellationToken);

    private async ValueTask BulkDeleteAsync<T>(
        IEnumerable<T> objects,
        bool useTransaction = true,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.BulkDeleteAsync(objects, useTransaction, cancellationToken);

    private async ValueTask BulkUpsertAsync<T>(
        IEnumerable<T> objects,
        bool useTransaction = true,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.BulkUpsertAsync(objects, useTransaction, cancellationToken);

    private async ValueTask<bool> ExistsAsync<T>(
        object[] objectIds,
        CancellationToken cancellationToken = default)
        where T : class =>
            await efCoreClient.ExistsAsync<T>(objectIds, cancellationToken);
}
```
