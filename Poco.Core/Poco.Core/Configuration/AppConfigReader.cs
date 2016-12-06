using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Poco.Core.Configuration
{
    class AppConfigReader
    {
        public class AppConfigException : Exception
        {
            public AppConfigException(string message) : base(message) { }
        }

        object Convert(Type type, string name, string value)
        {
            try
            {
                return System.Convert.ChangeType(value, type);
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Format("Failed to convert setting '{0}':'{1}' to required type {2}: {3}", name, value, type.Name, e));
                throw new AppConfigException(string.Format("Failed to convert setting '{0}':'{1}' to required type {2}.", name, value, type.Name));
            }
        }

        public object GetRequiredValue(Type type, string name)
        {
            if (ConfigurationManager.AppSettings[name] == null)
                throw new AppConfigException(string.Format("Failed to find expected setting '{0}' in .config file.", name));

            return Convert(type, name, ConfigurationManager.AppSettings[name]);
        }

        public T GetRequiredValue<T>(string name)
        {
            return (T)GetRequiredValue(typeof(T), name);
        }

        public object GetOptionalValue(Type type, string name, object defaultValue)
        {
            if (ConfigurationManager.AppSettings[name] == null)
                return defaultValue;

            return Convert(type, name, ConfigurationManager.AppSettings[name]);
        }

        public T GetOptionalValue<T>(string name, T defaultValue)
        {
            return (T)GetOptionalValue(typeof(T), name, defaultValue);
        }
    }
}
