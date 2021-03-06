using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace MSFramework.Ef.Design
{
	/// <summary>
	/// 设计时数据上下文实例工厂基类，用于执行数据迁移
	/// </summary>
	public abstract class DesignTimeDbContextFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
		where TDbContext : DbContext
	{
		/// <summary>
		/// 创建一个数据上下文实例
		/// </summary>
		/// <param name="args">参数</param>
		/// <returns></returns>
		public virtual TDbContext CreateDbContext(string[] args)
		{
			var services = GetServiceProvider();
			var dbContextFactory = services.CreateScope().ServiceProvider.GetRequiredService<DbContextFactory>();
			var options = dbContextFactory.GetDbContextOptions(typeof(TDbContext));
			// design time should not use transaction
			options.UseTransaction = false;
			return dbContextFactory.Create(options) as TDbContext;
		}

		protected abstract IServiceProvider GetServiceProvider();
	}
}