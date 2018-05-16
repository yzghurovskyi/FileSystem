﻿using FileSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem.DAL.Repositories {
    public class BaseRepository<T, TContext> : IDisposable  where T : FileSystemElement, new() where TContext : DbContext {
        protected readonly TContext context;
        protected readonly IApplicationUserAccessor userAccessor;

        public BaseRepository(TContext context, IApplicationUserAccessor userAccessor) {
            this.context = context;
            this.userAccessor = userAccessor;
        }

        public int Add(T entity) {
            entity.UserId = userAccessor.GetUserId();
            context.Add(entity);
            return entity.Id;
        }

        public void Update(T entity) {
            if (entity.UserId != userAccessor.GetUserId())
                throw new UnauthorizedAccessException("Cannot modify an entity that does not belong to current user");
            context.Update(entity);
        }

        public void Remove(T entity) {
            if (entity.UserId != userAccessor.GetUserId())
                throw new UnauthorizedAccessException("Cannot remove an entity that does not belong to current user");
            context.Remove(entity);
        }

        public void Remove(int id) {
            var entity = context.Find<T>(id);
            if (entity.UserId != userAccessor.GetUserId())
                throw new UnauthorizedAccessException("Cannot remove an entity that does not belong to current user");
            context.Remove(entity);
        }

        private void Save() {
            context.SaveChanges();
        }

        
        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
