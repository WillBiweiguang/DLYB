// -----------------------------------------------------------------------
//  <copyright file="DatabaseInitializer.cs" company="DLYB">
//      Copyright (c) 2014 DLYB. All rights reserved.
//  </copyright>
//  <last-editor>@DLYB</last-editor>
//  <last-date>2015-04-22 1:57</last-date>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Core.Data.Migrations;
using Infrastructure.Utility;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Logging;
using System.ComponentModel.DataAnnotations.Schema;


namespace Infrastructure.Core.Data
{
    /// <summary>
    /// 数据库初始化操作类
    /// </summary>
    public class DatabaseInitializer
    {
        private static readonly ICollection<Assembly> MapperAssemblies = new List<Assembly>();

        /// <summary>
        /// 获取 数据实体映射配置信息集合
        /// </summary>
        public static ICollection<IEntityMapper> EntityMappers { get { return GetAllEntityMapper(); } }

        private static List<IEntityMapper> MyEntityMappers = new List<IEntityMapper>();

        /// <summary>
        /// 设置数据库初始化，策略为自动迁移到最新版本
        /// </summary>
        public static void Initialize()
        {
            CodeFirstDbContext context = new CodeFirstDbContext();
            IDatabaseInitializer<CodeFirstDbContext> initializer;
            if (!context.Database.Exists())
            {
                initializer = new CreateDatabaseIfNotExistsWithSeed();
            }
            else
            {
                initializer = new NullDatabaseInitializer<CodeFirstDbContext>();

                // initializer = new MigrateDatabaseToLatestVersion<CodeFirstDbContext, MigrationsConfiguration>();
            }

            Database.SetInitializer(initializer);

            //EF预热，解决EF6第一次加载慢的问题
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            StorageMappingItemCollection mappingItemCollection = (StorageMappingItemCollection)objectContext.ObjectStateManager
                .MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
            mappingItemCollection.GenerateViews(new List<EdmSchemaError>());
            context.Dispose();
        }
        /// <summary>
        /// 设置Mysql初始化，不适用CodeFirst
        /// </summary>
        public static void InitializeMySql() {
            MySqlDbContext context = new MySqlDbContext();
            IDatabaseInitializer<MySqlDbContext> initializer = new NullDatabaseInitializer<MySqlDbContext>();

            //if (!context.Database.Exists())
            //{
            //    initializer = new CreateDatabaseIfNotExistsWithSeed();
            //}
            //else
            //{
            //    initializer = new NullDatabaseInitializer<CodeFirstDbContext>();

            //    // initializer = new MigrateDatabaseToLatestVersion<CodeFirstDbContext, MigrationsConfiguration>();
            //}

            Database.SetInitializer(initializer);

            //EF预热，解决EF6第一次加载慢的问题
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            StorageMappingItemCollection mappingItemCollection = (StorageMappingItemCollection)objectContext.ObjectStateManager
                .MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
            mappingItemCollection.GenerateViews(new List<EdmSchemaError>());
            context.Dispose();
        }
        /// <summary>
        /// 添加需要搜索实体映射的程序集到检索集合中
        /// </summary>
        public static void AddMapperAssembly(Assembly assembly)
        {
            assembly.CheckNotNull("assembly");
            if (MapperAssemblies.Any(m => m == assembly))
            {
                return;
            }
            MapperAssemblies.Add(assembly);
        }

        public static void AddMapper(IEntityMapper em)
        {
            MyEntityMappers.Add(em);
        }

        /// <summary>
        /// EF用mapping
        /// </summary>
        /// <returns></returns>
        private static ICollection<IEntityMapper> GetAllEntityMapper()
        {
            //Type baseType = typeof(IEntityMapper);
            //Type[] mapperTypes = MapperAssemblies.SelectMany(assembly => assembly.GetTypes())
            //    .Where(type => baseType.IsAssignableFrom(type) && type != baseType && !type.IsAbstract).ToArray();
            //List<IEntityMapper> result = mapperTypes.Select(type => Activator.CreateInstance(type) as IEntityMapper).ToList();
            //result.AddRange(MyEntityMappers);


            //查找所有需要自动Mapping的实体
            ITypeFinder typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var mapperTypes1 = typeFinder.FindClassesOfType<IEntityMapper>().Where(type => !type.IsAbstract && !type.IsGenericType).ToList();

            var mapperTypes0 = typeFinder.FindClassesOfType<IEntity>().Where(
                type => !type.IsAssignableFrom(typeof(IViewModel))
                    && type.GetInterfaces().Length > 0 && !type.GetInterfaces().ToList().Exists(e => e == typeof(IViewModel))
                && !type.IsAbstract
                && !mapperTypes1.Exists(b => b.BaseType.GenericTypeArguments.Length > 0 && b.BaseType.GenericTypeArguments[0] == type)
                ).ToList();

            var result1 = mapperTypes1.Select(type => Activator.CreateInstance(type) as IEntityMapper).ToList();
            MyEntityMappers.AddRange(result1);





            //自动Mapping
            foreach (var startUpTaskType in mapperTypes0)
            {
                //去除NotMappedAttribute的实体
                if (startUpTaskType.CustomAttributes.Count(a => a.AttributeType == typeof(NotMappedAttribute)) > 0)
                {
                    continue;
                }

                var Types = typeof(EntityConfigurationAll<,>);
                // Type[] typeArgs = { startUpTaskType,typeof(Int32) };

                var ins = Types.MakeGenericType(startUpTaskType, typeof(Int32));

                var Config = Activator.CreateInstance(ins) as IEntityMapper;
                MyEntityMappers.Add(Config);

            }

            //var a = new EntityConfigurationBase<IEntity,int>();


            return MyEntityMappers;

        }
    }
}