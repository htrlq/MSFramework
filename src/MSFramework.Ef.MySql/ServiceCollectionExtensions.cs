using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace MSFramework.Ef.MySql
{
	public static class ServiceCollectionExtensions
	{
		public static EntityFrameworkBuilder AddMySql<TDbContext>(
			this EntityFrameworkBuilder builder, IConfiguration configuration) where TDbContext : DbContextBase
		{
			builder.Services.AddMySql<TDbContext>(configuration);
			return builder;
		}

		public static EntityFrameworkBuilder AddMySql<TDbContext1, TDbContext2>(
			this EntityFrameworkBuilder builder, IConfiguration configuration) where TDbContext1 : DbContextBase
			where TDbContext2 : DbContextBase
		{
			builder.Services.AddMySql<TDbContext1>(configuration);
			builder.Services.AddMySql<TDbContext2>(configuration);
			return builder;
		}

		public static EntityFrameworkBuilder AddMySql<TDbContext1, TDbContext2, TDbContext3>(
			this EntityFrameworkBuilder builder, IConfiguration configuration) where TDbContext1 : DbContextBase
			where TDbContext2 : DbContextBase
			where TDbContext3 : DbContextBase
		{
			builder.Services.AddMySql<TDbContext1>(configuration);
			builder.Services.AddMySql<TDbContext2>(configuration);
			builder.Services.AddMySql<TDbContext3>(configuration);

			return builder;
		}

		public static IServiceCollection AddMySql<TDbContext>(
			this IServiceCollection services, IConfiguration configuration) where TDbContext : DbContextBase
		{
			var action = new Action<DbContextOptionsBuilder>(x =>
			{
				var dbContextType = typeof(TDbContext);
				var entryAssemblyName = dbContextType.Assembly.GetName().Name;

				var store = EntityFrameworkOptionsCollection.LoadFrom(configuration);
				var option = store.Get(dbContextType);

				if (option.DbContextType != dbContextType)
				{
					throw new ArgumentException("DbContextType is not correct");
				}

				if (option.EnableSensitiveDataLogging)
				{
					x.EnableSensitiveDataLogging();
				}

				// todo:
				// if (option.LazyLoadingProxiesEnabled)
				// {
				// 	x.UseLazyLoadingProxies();
				// }

				x.UseMySql(option.ConnectionString, options =>
				{
					options.MigrationsAssembly(entryAssemblyName);
					options.CharSet(CharSet.Utf8Mb4);
				});
			});

			services.AddDbContext<TDbContext>(action);

			return services;
		}
	}
}