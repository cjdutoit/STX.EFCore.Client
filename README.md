# STX.EFCore.Client

A Standard compliant client to wrap EF Core operations that can be used in a Storage Broker.

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

All methods accept an optional `CancellationToken`. The bulk write operations (`BulkInsertAsync`, `BulkUpdateAsync`, `BulkDeleteAsync`) also accept a `useTransaction` flag (defaults to `true`) that wraps the operation in a database transaction and rolls back automatically on failure.

## Exception Model

`EFCoreClient` translates lower-level exceptions into three client-level exception categories:

| Exception | When thrown |
|---|---|
| `EFCoreClientValidationException` | Invalid input (e.g. null object or null collection). |
| `EFCoreClientDependencyException` | A storage/database error occurred (e.g. `DbUpdateException`). |
| `EFCoreClientServiceException` | An unexpected service-level error occurred. |

`OperationCanceledException` is never wrapped — it propagates directly to the caller.

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
}
```
