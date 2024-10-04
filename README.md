# STX.EFCore.Client

A Standard compliant client to wrap EF Core operations that can be used in a Storage Broker.  This includes bulk operations from the popular Entity Framework Extensions for EF Core library. (Z.EntityFramework.Extensions.EFCore)

## Main Features

- InsertAsync
- SelectAllAsync
- SelectAsync
- UpdateAsync
- DeleteAsync
- BulkInsertAsync
- BulkReadAsync
- BulkUpdateAsync
- BulkDeleteAsync


## How do I use this?

Currently a storage broker looks something this:

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

With the client it will look like this and it addresses the sequencing issue that we currently have in brokers.

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

        private async ValueTask<T> InsertAsync<T>(T @object) where T : class =>
            await efCoreClient.InsertAsync(@object);

        private async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            await efCoreClient.SelectAllAsync<T>();

        private async ValueTask<T> SelectAsync<T>(params object[] @objectIds) where T : class =>
            await efCoreClient.SelectAsync<T>(@objectIds);

        private async ValueTask<T> UpdateAsync<T>(T @object) where T : class =>
            await efCoreClient.UpdateAsync(@object);

        private async ValueTask<T> DeleteAsync<T>(T @object) where T : class =>
            await efCoreClient.DeleteAsync(@object);

        private async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkInsertAsync<T>(objects);
            
        private async ValueTask BulkReadAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkReadAsync<T>(objects);

        private async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkUpdateAsync<T>(objects);

        private async ValueTask BulkDeleteAsync<T>(IEnumerable<T> objects) where T : class =>
            await efCoreClient.BulkDeleteAsync<T>(objects);
    }
```
