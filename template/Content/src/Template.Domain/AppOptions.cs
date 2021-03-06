using Microsoft.Extensions.Configuration;

namespace Template.Domain
{
	public class AppOptions
	{
		private readonly IConfiguration _configuration;

		public AppOptions(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string ApiName => _configuration["ApiName"];
		public string ApiSecret => _configuration["ApiSecret"];
		public string Authority => _configuration["Authority"];
		public bool RequireHttpsMetadata => bool.Parse(_configuration["RequireHttpsMetadata"]);

		public string DefaultConnectionString => _configuration["DbContexts:AppDbContext:ConnectionString"];
	}
}