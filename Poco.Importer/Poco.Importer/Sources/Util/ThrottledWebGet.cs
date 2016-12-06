using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Poco.Importer.Sources.Util
{
    class ThrottledWebGet
    {
        TimeSpan Throttling;

        public ThrottledWebGet(int throttlingMs)
        {
            Throttling = TimeSpan.FromMilliseconds(throttlingMs);
        }

        public string Get(string path)
        {
            System.Threading.Thread.Sleep(Throttling);
            var request = WebRequest.Create(path);
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        public dynamic GetJsonObject(string path)
        {
            return JObject.Parse(Get(path));
        }

        public dynamic GetJsonArray(string path)
        {
            return JArray.Parse(Get(path));
        }
    }
}
