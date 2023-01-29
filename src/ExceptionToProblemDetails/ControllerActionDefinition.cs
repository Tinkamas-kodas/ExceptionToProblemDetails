using System;
using System.Collections.Generic;

namespace ExceptionToProblemDetails
{
    public class ControllerActionDefinition
    {
        private const string ControllerName = "controller";
        private const int ControllerNameLength = 10;
        private const string ActionName = "action";
        public string Controller { get; }
        public string Action { get; }

        public ControllerActionDefinition(string controller, string action)
        {
            Controller = controller.EndsWith(ControllerName, StringComparison.CurrentCultureIgnoreCase)
                ? controller.Substring(0, controller.Length - ControllerNameLength)
                : controller;
            Action = action;
        }

        public bool MatchRoute(IDictionary<string, object> routeValues)
        {
            if (Controller == null)
                return true;
            
            if (!routeValues.ContainsKey(ControllerName) || !(routeValues[ControllerName] is string controller) ||
                !controller.Equals(Controller, StringComparison.CurrentCultureIgnoreCase)) return false;
            
            if (Action == null)
                return true;

            return routeValues.ContainsKey(ActionName) && routeValues[ActionName] is string action &&
                   action.Equals(Action, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}