using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Data;

namespace Poco.Core.Configuration
{
    using DB;

    /// <summary>
    /// Marks an item as mappable to the app.config file by name.
    /// </summary>
    public class ConfigurationEntryAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ConfigurationEntryAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public abstract class ConfigurationSource
    {
        protected class ConfigurationSourceException : Exception
        {
            public ConfigurationSourceException(string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        object ConvertType(object value, Type type)
        {
            // Convert.ChangeType does not handle Enums, so we need to intercept this.
            if (type.IsEnum)
                // If the field is an enum, assume that the user has passed a string.
                return Enum.Parse(type, value.ToString());

            // Else fall back to the catch-all:
            return System.Convert.ChangeType(value, type);
        }

        protected object Convert(Type type, string name, object value)
        {
            if (System.Convert.IsDBNull(value))
                return null;

            // Handle null cases:
            if (value == null) return null;

            try
            {
                return Nullable.GetUnderlyingType(type) != null 
                    ?
                    // Then we know that type = typeof(T?) for some T.
                    ConvertType(value, Nullable.GetUnderlyingType(type))
                    :
                    ConvertType(value, type);
            }
            catch (Exception e)
            {
                Trace.TraceError(string.Format("Failed to convert setting '{0}':'{1}' to required type {2}: {3}", name, value, type.Name, e));
                throw new ConfigurationSourceException(string.Format("Failed to convert setting '{0}':'{1}' to required type {2}.", name, value, type.Name), e);
            }
        }

        public abstract object GetValue(Type type, string name, object defaultValue);
    }

    public class AppConfigSource : ConfigurationSource
    {
        public override object GetValue(Type type, string name, object defaultValue)
        {
            if (ConfigurationManager.AppSettings[name] == null)
                return defaultValue;

            return Convert(type, name, ConfigurationManager.AppSettings[name]);
        }
    }

    public class MySqlDbSource : ConfigurationSource
    {
        /*
         * 
         * Assumes that the stored procedure spName returns configuration values as a table with columns:
         * name: Name of the configuration setting.
         * value: Value for that configuration setting.
         * 
         */

        MySqlClient Client;

        Dictionary<string, object> Settings = new Dictionary<string, object>();

        public MySqlDbSource(string connectionString, string nameSpace, string spName = "_getConfiguration")
        {
            Client = new MySqlClient(connectionString);

            var ds = Client.ExecuteStoredProcedure(spName, 
                new Dictionary<string, object> 
                { 
                    { "@_namespace", nameSpace } 
                });

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                try
                {
                    Settings[r["name"].ToString()] = r["value"];
                }
                catch (Exception e)
                {
                    Trace.TraceError("Failed to fetch configuration settings while calling {0}", spName);
                    throw new ConfigurationSourceException(string.Format("Failed to fetch configuration settings while calling {0}", spName), e);
                }
            }
        }

        public override object GetValue(Type type, string name, object defaultValue)
        {
            object value;
            if (!Settings.TryGetValue(name, out value))
                return defaultValue;

            return Convert(type, name, value);
        }
    }

    public class ConfigurationReader<TConfiguration> where TConfiguration : new()
    {
        /*
         * Works on a POD object which has raw fields mapped to entries in an abstracted source
         * using the ConfigurationEntryAttribute.
         * 
         * E.g.
         * class Config
         * {
         *     [ConfigurationEntry("Blurb", "Whether or not to insert the blurb (which may swell up the database!)")]
         *     public bool Blurb = true;
         *     
         *     [ConfigurationEntry("DatabaseConnectionString", "Connection string to the MySQL database to insert into.")]
         *     public string DatabaseConnectionString = null;
         * }
         * 
         * var c = ConfigurationReader<Config>.FromSource(new AppConfigSource()); 
         * or 
         * var c = ConfigurationReader<Config>.FromSource(new MySqlDbSource("...", "_getConfiguration"));
         * 
         */


        class AttributedField<TAttribute>
        {
            public FieldInfo Field { get; set; }
            public TAttribute Attribute { get; set; }

            public AttributedField(FieldInfo field, TAttribute attribute)
            {
                Field = field;
                Attribute = attribute;
            }

            public static IEnumerable<AttributedField<TAttribute>> GetAttributedFields(Type type)
            {
                return type
                    .GetFields()
                    .Where(f => f.IsPublic && f.GetCustomAttributes(typeof(TAttribute), true).Any())
                    .Select(f =>
                        new AttributedField<TAttribute>(f,
                            (TAttribute)f.GetCustomAttributes(typeof(TAttribute), true).First()
                            )
                        );
            }
        }

        static IEnumerable<AttributedField<ConfigurationEntryAttribute>> GetConfigurationEntires()
        {
            return AttributedField<ConfigurationEntryAttribute>.GetAttributedFields(typeof(TConfiguration));
        }

        static object GetValue(AttributedField<ConfigurationEntryAttribute> f, TConfiguration c, ConfigurationSource configurationSource, Dictionary<string, object> overrides)
        {
            string entryName = f.Attribute.Name;

            object overrideValue;
            if (overrides == null || !overrides.TryGetValue(entryName, out overrideValue))
                // Use the provided source.
                return configurationSource.GetValue(f.Field.FieldType, entryName, f.Field.GetValue(c));
            else
                // Use the provided set of override values.
                return overrideValue;
        }

        public static TConfiguration FromSource(ConfigurationSource configurationSource, Dictionary<string, object> overrides = null)
        {
            var c = new TConfiguration();
            
            foreach (var f in GetConfigurationEntires())
            {
                f.Field.SetValue(
                    c, GetValue(f, c, configurationSource, overrides)
                    );
            }

            return c;
        }

        public static void WriteParameterDescriptions(TextWriter writer)
        {
            // For default values.
            var def = new TConfiguration();

            foreach (var f in GetConfigurationEntires())
                writer.WriteLine("{0}:{1} (Default:{2})", f.Field.Name, f.Attribute.Description, f.Field.GetValue(def) ?? "null");
        }

        public static void WriteParameterValues(TextWriter writer, TConfiguration c)
        {
            foreach (var f in GetConfigurationEntires())
                writer.WriteLine("{0}:{1}", f.Field.Name, f.Field.GetValue(c));
        }
    }
}
