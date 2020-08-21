// -----------------------------------------------------------------------
//  <copyright file="IdentityService.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 17:21</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Entity;
using Infrastructure.Web.Domain.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;


namespace Infrastructure.Web.Domain.Services
{
    /// <summary>
    /// 业务实现——身份认证模块
    /// </summary>
    public partial class SysRoleService : BaseService<SysRole>, ISysRoleService
    {

         public RoleManager<SysRole, int> RoleContext{set;get;}
         public SysRoleService(IUnitOfWork dbContext)
             : base(dbContext)
        {
            RoleContext = new RoleManager<SysRole, int>(new RoleStore((DbContext)dbContext));
        }


    }

    /// <summary>
    ///     EntityFramework based implementation
    /// </summary>
    /// <typeparam name="SysRole"></typeparam>
    /// <typeparam name="Int32"></typeparam>
    /// <typeparam name="SysUserRole"></typeparam>
    public class RoleStore : IQueryableRoleStore<SysRole, Int32>

    {
        private bool _disposed;
        private BaseService<SysRole> _roleStore;

        /// <summary>
        ///     Constructor which takes a db context and wires up the stores with default instances using the context
        /// </summary>
        /// <param name="context"></param>
        public RoleStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            _roleStore = new BaseService<SysRole>((IUnitOfWork)context);
        }

        /// <summary>
        ///     Context for the store
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        ///     If true will call dispose on the DbContext during Dipose
        /// </summary>
        public bool DisposeContext { get; set; }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Task<SysRole> FindByIdAsync(Int32 roleId)
        {
            ThrowIfDisposed();
            return _roleStore.Repository.GetByKeyAsync(roleId);
        }

        /// <summary>
        ///     Find a role by name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Task<SysRole> FindByNameAsync(string roleName)
        {
            ThrowIfDisposed();
            return _roleStore.Repository.Entities.FirstOrDefaultAsync(u => u.Name.ToUpper() == roleName.ToUpper());
        }

        /// <summary>
        ///     Insert an entity
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task CreateAsync(SysRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Repository.Insert(role);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        ///     Mark an entity for deletion
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task DeleteAsync(SysRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Repository.Delete(role);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        ///     Update an entity
        /// </summary>
        /// <param name="role"></param>
        public virtual async Task UpdateAsync(SysRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _roleStore.Repository.Update(role);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        ///     Returns an IQueryable of users
        /// </summary>
        public IQueryable<SysRole> Roles
        {
            get { return _roleStore.Repository.Entities; }
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
            _roleStore = null;
        }
    }

}