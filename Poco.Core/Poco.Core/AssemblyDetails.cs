using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Poco.Core
{
    public static class AssemblyDetails
    {
        static TAttribute GetAssemblyAttribute<TAttribute>(bool inherit = false) where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(TAttribute), inherit);
        }

        public static string Title
        {
            get
            {
                return GetAssemblyAttribute<AssemblyTitleAttribute>().Title;
                // Can also use System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase) if this attribute does not exist.
            }
        }

        public static string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string Description
        {
            get
            {
                return GetAssemblyAttribute<AssemblyDescriptionAttribute>().Description;
            }
        }

        public static string Product
        {
            get
            {
                return GetAssemblyAttribute<AssemblyProductAttribute>().Product;
            }
        }

        public static string Copyright
        {
            get
            {
                return GetAssemblyAttribute<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        public static string Company
        {
            get
            {
                return GetAssemblyAttribute<AssemblyCompanyAttribute>().Company;
            }
        }
    }
}
