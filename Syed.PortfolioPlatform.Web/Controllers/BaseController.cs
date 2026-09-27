using Microsoft.AspNetCore.Mvc;
using Syed.PortfolioPlatform.Web.Configurations;

namespace Syed.PortfolioPlatform.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly PortfolioSettings Settings;

        protected BaseController(PortfolioSettings settings)
        {
            Settings = settings;
        }
    }
}
