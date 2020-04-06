using System;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Identity.Api.Extensions
{
    internal static class ControllerExtensions
    {
        internal static string Action(this Controller controller, string action, object values)
        {
            const string controllerSuffix = "Controller";

            var name = controller.GetType().Name;
            var controllerNameForRoute = name.EndsWith(controllerSuffix, StringComparison.Ordinal)
                ? name.Substring(0, name.Length - controllerSuffix.Length)
                : name;

            return controller.Url.Action(action, controllerNameForRoute, values, controller.Request.Scheme);
        }
    }
}
