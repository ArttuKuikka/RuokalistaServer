using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RuokalistaServer.Attributes
{
    public class Feature: ActionFilterAttribute
    {
        private readonly Func<bool> _condition;

        public Feature(string type)
        {
            switch(type)
            {
                case "IG":
                    _condition = () => GlobalConfig.IGEnabled;
                    break;
                case "Infotv":
                    _condition = () => GlobalConfig.InfotvEnabled;
                    break;
                case "Aanestys":
                    _condition = () => GlobalConfig.AanestysEnabled;
                    break;
                case "API":
                    _condition = () => GlobalConfig.APIEnabled;
                    break;
                default:
                    _condition = () => false;
                    break;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_condition())
            {
                filterContext.Result = new ForbidResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
