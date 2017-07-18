using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PTV.Framework
{
    [RegisterService(typeof(EnvironmentHelper),RegisterType.Singleton)]
    public class EnvironmentHelper
    {
        private ExecutingEnvironment environmentType;

        public ExecutingEnvironment GetExecutingEnvironment()
        {
            if (environmentType == ExecutingEnvironment.Unknown)
            {
                var executingAssembly = Assembly.GetEntryAssembly().GetName().Name;
                switch (executingAssembly)
                {
                    case "PTV.Application.Web": environmentType = ExecutingEnvironment.Web; break;
                    case "PTV.Application.Api": environmentType = ExecutingEnvironment.UiApi; break;
                    case "PTV.Application.OpenApi": environmentType = ExecutingEnvironment.OpenApi; break;
                    default: environmentType = ExecutingEnvironment.Unknown; break;
                }
            }
            return environmentType;
        }
    }



    public enum ExecutingEnvironment
    {
        Unknown,
        Web,
        UiApi,
        OpenApi
    }
}
