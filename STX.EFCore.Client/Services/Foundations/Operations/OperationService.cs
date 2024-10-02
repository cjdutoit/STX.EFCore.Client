// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace STX.EFCore.Client.Services.Foundations.Operations
{
    public class OperationService : IOperationService
    {
        private readonly DbContext dbContext;

        public OperationService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async ValueTask<T> InsertAsync<T>(T @object) where T : class
        {
            try
            {
                dbContext.Entry(@object).State = EntityState.Added;
                await dbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask<IQueryable<T>> SelectAllAsync<T>() where T : class =>
            dbContext.Set<T>();

        public async ValueTask<T> SelectAsync<T>(params object[] objectIds) where T : class =>
            await dbContext.FindAsync<T>(objectIds);

        public async ValueTask<T> UpdateAsync<T>(T @object) where T : class
        {
            try
            {
                dbContext.Entry(@object).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask<T> DeleteAsync<T>(T @object) where T : class
        {
            try
            {
                dbContext.Entry(@object).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();

                return @object;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dbContext.Entry(@object).State = EntityState.Detached;
            }
        }

        public async ValueTask BulkInsertAsync<T>(IEnumerable<T> objects) where T : class
        {
            try
            {
                objects.ToList().ForEach(@object => dbContext.Entry(@object).State = EntityState.Added);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                objects.ToList().ForEach(@object => dbContext.Entry(@object).State = EntityState.Detached);
            }
        }

        public async ValueTask BulkUpdateAsync<T>(IEnumerable<T> objects) where T : class
        {
            try
            {
                objects.ToList().ForEach(@object => dbContext.Entry(@object).State = EntityState.Modified);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                objects.ToList().ForEach(@object => dbContext.Entry(@object).State = EntityState.Detached);
            }
        }
    }
}
