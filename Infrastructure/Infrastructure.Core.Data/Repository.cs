// -----------------------------------------------------------------------
//  <copyright file="Repository.cs" company="DLYB">
//      Copyright (c) 2014-2015 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 15:22</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Infrastructure.Utility;
using Infrastructure.Utility.Data;
using Infrastructure.Utility.Extensions;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using System.Security.Claims;


namespace Infrastructure.Core.Data
{
    /// <summary>
    /// EntityFramework的仓储实现
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 初始化一个<see cref="Repository{TEntity, TKey}"/>类型的新实例
        /// </summary>
        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbSet = ((DbContext)unitOfWork).Set<TEntity>();
        }

        private int _LoginUserID = 0;
        private string _LoginUserName = null;

        /// <summary>
        /// 登录ID
        /// </summary>
        public int LoginUserID
        {
            get
            {
                if (_LoginUserID == 0)
                {
                    //Request.GetOwinContext().Authentication.User.Claims
                    //(ClaimsPrincipal)Thread.CurrentPrincipal
                    var claim = ClaimsPrincipal.Current.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier);
                    if (claim == null || string.IsNullOrEmpty(claim.Value))
                    {
                        return 0;
                    }
                    else
                    {
                        return int.Parse(claim.Value);
                    }

                }
                else { return _LoginUserID; }
            }

            set { _LoginUserID = value; }
        }
        public string LoginUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_LoginUserName))
                {
                    var claim = ClaimsPrincipal.Current.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Name);
                    if (claim == null)
                    {
                        return null;
                    }
                    else
                    {
                        return claim.Value;
                    }

                }
                else { return _LoginUserName; }
            }

            set { _LoginUserName = value; }
        }

        /// <summary>
        /// 获取 当前单元操作对象
        /// </summary>
        public IUnitOfWork UnitOfWork { get { return _unitOfWork; } }

        /// <summary>
        /// 获取 当前实体类型的查询数据集
        /// </summary>
        public IQueryable<TEntity> Entities { get { return _dbSet; } }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Insert(TEntity entity)
        {
            entity.CheckNotNull("entity");
            SetAddEntity(entity);
            _dbSet.Add(entity);
            return SaveChanges();
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Insert(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            foreach (var a in entities)
            {
                SetAddEntity(a);
            }
            _dbSet.AddRange(entities);
            return SaveChanges();
        }

        /// <summary>
        /// 以DTO为载体批量插入实体
        /// </summary>
        /// <typeparam name="TAddDto">添加DTO类型</typeparam>
        /// <param name="dtos">添加DTO信息集合</param>
        /// <param name="checkAction">添加信息合法性检查委托</param>
        /// <param name="updateFunc">由DTO到实体的转换委托</param>
        /// <returns>业务操作结果</returns>
        public OperationResult Insert<TAddDto>(ICollection<TAddDto> dtos,
            Action<TAddDto> checkAction = null,
            Func<TAddDto, TEntity, TEntity> updateFunc = null)
            where TAddDto : IAddDto
        {
            dtos.CheckNotNull("dtos");
            List<string> names = new List<string>();
            foreach (TAddDto dto in dtos)
            {
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(dto);
                    }
                    TEntity entity = dto.MapTo<TEntity>();
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                    Insert(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                string name = GetNameValue(dto);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”添加成功".FormatWith(names.ExpandAndToString())
                        : "{0}个信息添加成功".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Delete(TEntity entity)
        {
            entity.CheckNotNull("entity");

            string strName = "";

            if (SetDeleteEntity(entity, ref strName))
            {
                Update(entity, new List<string>() { strName });
            }
            else
            {
                _dbSet.Remove(entity);
            }
            return SaveChanges();
        }

        /// <summary>
        /// 删除多条
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual int Delete(TKey[] keys)
        {
            return Delete(a => keys.Contains(a.Id));
        }

        /// <summary>
        /// 删除指定编号的实体
        /// </summary>
        /// <param name="key">实体编号</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Delete(TKey key)
        {
            CheckEntityKey(key, "key");
            TEntity entity = _dbSet.Find(key);


            return entity == null ? 0 : Delete(entity);
        }

        /// <summary>
        /// 删除所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            predicate.CheckNotNull("predicate");
            TEntity[] entities = _dbSet.Where(predicate).ToArray();
            return entities.Length == 0 ? 0 : Delete(entities);
        }

        /// <summary>
        /// 批量删除删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();

            //_dbSet.RemoveRange(entities);
            foreach (var a in entities)
            {
                Delete(a);
            }

            return SaveChanges();
        }

        /// <summary>
        /// 以标识集合批量删除实体
        /// </summary>
        /// <param name="ids">标识集合</param>
        /// <param name="checkAction">删除前置检查委托</param>
        /// <param name="deleteFunc">删除委托，用于删除关联信息</param>
        /// <returns>业务操作结果</returns>
        public OperationResult Delete(ICollection<TKey> ids, Action<TEntity> checkAction = null, Func<TEntity, TEntity> deleteFunc = null)
        {
            ids.CheckNotNull("ids");
            List<string> names = new List<string>();
            foreach (TKey id in ids)
            {
                TEntity entity = _dbSet.Find(id);
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(entity);
                    }
                    if (deleteFunc != null)
                    {
                        entity = deleteFunc(entity);
                    }
                    Delete(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                string name = GetNameValue(entity);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”删除成功".FormatWith(names.ExpandAndToString())
                        : "{0}个信息删除成功".FormatWith(ids.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="entity">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        public virtual int Update(TEntity entity)
        {
            entity.CheckNotNull("entity");

            var lst = GetModifyColumns(entity);

            return Update(entity, lst);
            //return SaveChanges();
        }


        public int Update(TEntity entity, List<string> lst)
        {
            entity.CheckNotNull("entity");
            _dbSet.Attach(entity);
            var SetEntry = ((IObjectContextAdapter)((DbContext)_unitOfWork)).ObjectContext.
                ObjectStateManager.GetObjectStateEntry(entity);
            foreach (var t in lst)
            {
                SetEntry.SetModifiedProperty(t);
            }

            ((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
            return SaveChanges();
        }

        /// <summary>
        /// 以DTO为载体批量更新实体
        /// </summary>
        /// <typeparam name="TEditDto">更新DTO类型</typeparam>
        /// <param name="dtos">更新DTO信息集合</param>
        /// <param name="checkAction">更新信息合法性检查委托</param>
        /// <param name="updateFunc">由DTO到实体的转换委托</param>
        /// <returns>业务操作结果</returns>
        public OperationResult Update<TEditDto>(ICollection<TEditDto> dtos,
            Action<TEditDto> checkAction = null,
            Func<TEditDto, TEntity, TEntity> updateFunc = null)
            where TEditDto : IEditDto<TKey>
        {
            dtos.CheckNotNull("dtos");
            List<string> names = new List<string>();
            foreach (TEditDto dto in dtos)
            {
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(dto);
                    }
                    TEntity entity = _dbSet.Find(dto.Id);
                    if (entity == null)
                    {
                        return new OperationResult(OperationResultType.QueryNull);
                    }
                    entity = dto.MapTo(entity);
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                    Update(entity);
                    //((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
                }
                catch (Exception e)
                {
                    return new OperationResult(OperationResultType.Error, e.Message);
                }
                string name = GetNameValue(dto);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperationResult(OperationResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”更新成功".FormatWith(names.ExpandAndToString())
                        : "{0}个信息更新成功".FormatWith(dtos.Count))
                : new OperationResult(OperationResultType.NoChanged);
        }

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="id">编辑的实体标识</param>
        /// <returns>是否存在</returns>
        public bool CheckExists(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            TKey defaultId = default(TKey);
            var entity = _dbSet.Where(predicate).Select(m => new { m.Id }).SingleOrDefault();
            bool exists = (!(typeof(TKey).IsValueType) && id.Equals(null)) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !entity.Id.Equals(id);
            return exists;
        }

        /// <summary>
        /// 查找指定主键的实体
        /// </summary>
        /// <param name="key">实体主键</param>
        /// <returns>符合主键的实体，不存在时返回null</returns>
        public TEntity GetByKey(TKey key)
        {
            CheckEntityKey(key, "key");
            return _dbSet.Find(key);
        }

        /// <summary>
        /// 获取贪婪加载导航属性的查询数据集
        /// </summary>
        /// <param name="path">属性表达式，表示要贪婪加载的导航属性</param>
        /// <returns>查询数据集</returns>
        public IQueryable<TEntity> GetInclude<TProperty>(Expression<Func<TEntity, TProperty>> path)
        {
            path.CheckNotNull("path");
            return _dbSet.Include(path);
        }

        /// <summary>
        /// 获取贪婪加载多个导航属性的查询数据集
        /// </summary>
        /// <param name="paths">要贪婪加载的导航属性名称数组</param>
        /// <returns>查询数据集</returns>
        public IQueryable<TEntity> GetIncludes(params string[] paths)
        {
            paths.CheckNotNull("paths");
            IQueryable<TEntity> source = _dbSet;
            foreach (string path in paths)
            {
                source = source.Include(path);
            }
            return source;
        }

        /// <summary>
        /// 创建一个原始 SQL 查询，该查询将返回此集中的实体。 
        /// 默认情况下，上下文会跟踪返回的实体；可通过对返回的 DbRawSqlQuery 调用 AsNoTracking 来更改此设置。 请注意返回实体的类型始终是此集的类型，而不会是派生的类型。 如果查询的一个或多个表可能包含其他实体类型的数据，则必须编写适当的 SQL 查询以确保只返回适当类型的实体。 与接受 SQL 的任何 API 一样，对任何用户输入进行参数化以便避免 SQL 注入攻击是十分重要的。 您可以在 SQL 查询字符串中包含参数占位符，然后将参数值作为附加参数提供。 您提供的任何参数值都将自动转换为 DbParameter。 context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @p0", userSuppliedAuthor); 或者，您还可以构造一个 DbParameter 并将它提供给 SqlQuery。 这允许您在 SQL 查询字符串中使用命名参数。 context.Set(typeof(Blog)).SqlQuery("SELECT * FROM dbo.Posts WHERE Author = @author", new SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <param name="trackEnabled">是否跟踪返回实体</param>
        /// <param name="sql">SQL 查询字符串。</param>
        /// <param name="parameters">要应用于 SQL 查询字符串的参数。 如果使用输出参数，则它们的值在完全读取结果之前不可用。 这是由于 DbDataReader 的基础行为而导致的，有关详细信息，请参见 http://go.microsoft.com/fwlink/?LinkID=398589。</param>
        /// <returns></returns>
        public IEnumerable<TEntity> SqlQuery(string sql, bool trackEnabled = true, params object[] parameters)
        {
            return trackEnabled
                ? _dbSet.SqlQuery(sql, parameters)
                : _dbSet.SqlQuery(sql, parameters).AsNoTracking();
        }


        public int SqlExcute(string sql, params object[] parameters)
        {
            return ((DbContext)_unitOfWork).Database.ExecuteSqlCommand(sql, parameters);
        }

#if NET45

        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> InsertAsync(TEntity entity)
        {
            entity.CheckNotNull("entity");
            _dbSet.Add(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步批量插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.AddRange(entities);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(TEntity entity)
        {
            entity.CheckNotNull("entity");
            _dbSet.Remove(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步删除指定编号的实体
        /// </summary>
        /// <param name="key">实体编号</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(TKey key)
        {
            CheckEntityKey(key, "key");
            TEntity entity = await _dbSet.FindAsync(key);
            return entity == null ? 0 : await DeleteAsync(entity);
        }

        /// <summary>
        /// 异步删除所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            predicate.CheckNotNull("predicate");
            TEntity[] entities = await _dbSet.Where(predicate).ToArrayAsync();
            return entities.Length == 0 ? 0 : await DeleteAsync(entities);
        }

        /// <summary>
        /// 异步批量删除删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.RemoveRange(entities);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步更新实体对象
        /// </summary>
        /// <param name="entity">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> UpdateAsync(TEntity entity)
        {
            entity.CheckNotNull("entity");
            ((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步检查实体是否存在
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="id">编辑的实体标识</param>
        /// <returns>是否存在</returns>
        public async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey))
        {
            TKey defaultId = default(TKey);
            var entity = await _dbSet.Where(predicate).Select(m => new { m.Id }).SingleOrDefaultAsync();
            bool exists = (!(typeof(TKey).IsValueType) && id.Equals(null)) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !entity.Id.Equals(id);
            return exists;
        }

        /// <summary>
        /// 异步查找指定主键的实体
        /// </summary>
        /// <param name="key">实体主键</param>
        /// <returns>符合主键的实体，不存在时返回null</returns>
        public async Task<TEntity> GetByKeyAsync(TKey key)
        {
            CheckEntityKey(key, "key");
            return await _dbSet.FindAsync(key);
        }

#endif

        #region 私有方法

        private int SaveChanges()
        {
            return _unitOfWork.TransactionEnabled ? 0 : _unitOfWork.SaveChanges();
        }

#if NET45

        private async Task<int> SaveChangesAsync()
        {
            return _unitOfWork.TransactionEnabled ? 0 : await _unitOfWork.SaveChangesAsync();
        }

#endif

        private static void CheckEntityKey(object key, string keyName)
        {
            key.CheckNotNull("key");
            keyName.CheckNotNull("keyName");

            Type type = key.GetType();
            if (type == typeof(int))
            {
                ((int)key).CheckGreaterThan(keyName, 0);
            }
            else if (type == typeof(string))
            {
                ((string)key).CheckNotNullOrEmpty(keyName);
            }
            else if (type == typeof(Guid))
            {
                ((Guid)key).CheckNotEmpty(keyName);
            }
        }

        private static string GetNameValue(object value)
        {
            dynamic obj = value;
            try
            {
                return obj.Name;
            }
            catch
            {
                return null;
            }
        }


        protected bool SetDeleteEntity(TEntity oSrc, ref string strName)
        {

            Type t = typeof(TEntity);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                object value;
                if (pi.Name.ToLower() == "isdeleted") { value = true; }
                else { continue; }
                if (value != null)
                {
                    pi.SetValue(oSrc, value, null);
                    strName = pi.Name;
                    return true;
                }
            }

            return false;

        }

        protected void SetAddEntity(TEntity oSrc)
        {

            Type t = typeof(TEntity);
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                object value;
                if (pi.Name.ToLower() == "createddate") { value = DateTime.Now; }
                else if (pi.Name.ToLower() == "createduserid") { value = pi.PropertyType == typeof(System.String) ? LoginUserName : (object)LoginUserID; }
                else if (pi.Name.ToLower() == "updateddate") { value = DateTime.Now; }
                else if (pi.Name.ToLower() == "updateduserid") { value = pi.PropertyType == typeof(System.String) ? LoginUserName : (object)LoginUserID; }
                else if (pi.Name.ToLower() == "isdeleted") { value = false; }
                else { continue; }
                if (value != null)
                {
                    pi.SetValue(oSrc, value, null);
                }
            }

        }

        protected List<string> GetModifyColumns(TEntity oSrc)
        {
            Type t = typeof(TEntity);
            PropertyInfo[] properties = t.GetProperties();
            List<string> lstRet = new List<string>();

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    object value = null;
                    if (pi.Name.ToLower() == "updateddate") { value = DateTime.Now; pi.SetValue(oSrc, value, null); }
                    else if (pi.Name.ToLower() == "updateduserid")
                    {

                        if (pi.PropertyType == typeof(System.String))
                        {
                            value = LoginUserName;

                        }
                        else
                        {
                            value = LoginUserID;
                        }

                        pi.SetValue(oSrc, value, null);

                    }
                    else
                    {
                        value = pi.GetValue(oSrc, null);
                    }
                    if (value != null && (!pi.PropertyType.IsGenericType || pi.PropertyType.IsValueType))
                    {

                        lstRet.Add(pi.Name);
                    }
                    else
                    {

                    }
                }
            }
            return lstRet;
        }

        #endregion
    }
}