using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using RuokalistaServer.Auth;

namespace RuokalistaServer.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class OnlyRootAllowed: Attribute, IAuthorizationFilter
	{

		public void OnAuthorization(AuthorizationFilterContext context)
		{

			var user = context.HttpContext.User;


			if(user.Identity != null)
			{
				if (!user.Identity.IsAuthenticated || user.Identity.Name != GlobalConfig.RootUser)
				{
					// If not authorized, return a 403 Forbidden result
					context.Result = new ForbidResult();
				}
			}
			else
			{
				context.Result = new UnauthorizedResult();
			}


		}


	}
}
