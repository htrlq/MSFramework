using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSFramework.Domain;

namespace MSFramework.AspNetCore
{
	public class MSFrameworkControllerBase : ControllerBase
	{
		protected IMSFrameworkSession Session { get; }

		protected ILogger Logger { get; }

		protected MSFrameworkControllerBase(IMSFrameworkSession session, ILogger logger)
		{
			Session = session;
			Logger = logger;
		}

		protected IActionResult Ok(dynamic value, string msg = "")
		{
			return new ApiResult(value, msg);
		}

		protected IActionResult Failed(string msg = "", int code = 20000)
		{
			if (code < 20000 && code >= 30000)
			{
				throw new MSFrameworkException("Failed code should be less than 30000 and greater than 20000");
			}

			return new ApiResult(new
			{
				success = false,
				code,
				msg
			})
			{
				StatusCode = 500
			};
		}
	}
}