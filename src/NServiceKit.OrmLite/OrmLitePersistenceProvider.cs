//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2010 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack: new BSD license.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NServiceKit.Common;
using NServiceKit.Common.Utils;
using NServiceKit.DataAccess;

namespace NServiceKit.OrmLite
{
    /// <summary>
    /// Allow for code-sharing between OrmLite, IPersistenceProvider and ICacheClient.
    /// </summary>
	public class OrmLitePersistenceProvider
		: IBasicPersistenceProvider
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		protected string ConnectionString { get; set; }

        /// <summary>true to dispose connection.</summary>
		protected bool DisposeConnection = true;

        /// <summary>The connection.</summary>
		protected IDbConnection connection;

        /// <summary>Gets the connection.</summary>
        /// <value>The connection.</value>
		public IDbConnection Connection
		{
			get
			{
				if (connection == null)
				{
				    var connStr = this.ConnectionString;
                    connection = connStr.OpenDbConnection();
				}
				return connection;
			}
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLitePersistenceProvider class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
		public OrmLitePersistenceProvider(string connectionString)
		{
			ConnectionString = connectionString;
		}

        /// <summary>
        /// Initializes a new instance of the NServiceKit.OrmLite.OrmLitePersistenceProvider class.
        /// </summary>
        /// <param name="connection">The connection.</param>
		public OrmLitePersistenceProvider(IDbConnection connection)
		{
			this.connection = connection;
			this.DisposeConnection = false;
		}

        /// <summary>Creates the command.</summary>
        /// <returns>The new command.</returns>
		private IDbCommand CreateCommand()
		{
			var cmd = this.Connection.CreateCommand();
			cmd.CommandTimeout = OrmLiteConfig.CommandTimeout;
			return cmd;
		}

        /// <summary>Gets by identifier.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The by identifier.</returns>
		public T GetById<T>(object id)
			where T : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				return dbCmd.GetByIdOrDefault<T>(id);
			}
		}

        /// <summary>Gets by identifiers.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="ids">The identifiers.</param>
        /// <returns>The by identifiers.</returns>
		public IList<T> GetByIds<T>(ICollection ids)
			where T : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				return dbCmd.GetByIds<T>(ids);
			}
		}

        /// <summary>Stores the given entity.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>A T.</returns>
		public T Store<T>(T entity)
			where T : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				return InsertOrUpdate(dbCmd, entity);
			}
		}

        /// <summary>Inserts an or update.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="dbCmd"> The database command.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>A T.</returns>
		private static T InsertOrUpdate<T>(IDbCommand dbCmd, T entity)
			where T : class, new()
		{
			var id = IdUtils.GetId(entity);
			var existingEntity = dbCmd.GetByIdOrDefault<T>(id);
			if (existingEntity != null)
			{
				existingEntity.PopulateWith(entity);
				dbCmd.Update(entity);

				return existingEntity;
			}

			dbCmd.Insert(entity);
			return entity;
		}

        /// <summary>Stores all.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
		public void StoreAll<TEntity>(IEnumerable<TEntity> entities) 
			where TEntity : class, new()
		{
			using (var dbCmd = CreateCommand())
			using (var dbTrans = this.Connection.BeginTransaction())
			{
				foreach (var entity in entities)
				{
					InsertOrUpdate(dbCmd, entity);
				}
				dbTrans.Commit();
			}
		}

        /// <summary>Deletes the given entity.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="entity">The entity.</param>
		public void Delete<T>(T entity)
			where T : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				dbCmd.Delete(entity);
			}
		}

        /// <summary>Deletes the by identifier described by ID.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="id">The identifier.</param>
		public void DeleteById<T>(object id) where T : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				dbCmd.DeleteById<T>(id);
			}
		}

        /// <summary>Deletes the by identifiers described by ids.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="ids">The identifiers.</param>
		public void DeleteByIds<T>(ICollection ids) where T : class, new()
		{
			using (var dbCmd = this.CreateCommand())
			{
				dbCmd.DeleteByIds<T>(ids);
			}
		}

        /// <summary>Deletes all.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
		public void DeleteAll<TEntity>() where TEntity : class, new()
		{
			using (var dbCmd = CreateCommand())
			{
				dbCmd.DeleteAll<TEntity>();
			}
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
		public void Dispose()
		{
			if (!DisposeConnection) return;
			if (this.connection == null) return;
			
			this.connection.Dispose();
			this.connection = null;
		}
	}
}