using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Poco.Core.DB
{
    public class DBClient
    {
        MySqlClient Client;
        Dictionary<string, MySqlParameterCollection> DerivedParameters = new Dictionary<string, MySqlParameterCollection>();

        public DBClient(string connectionString)
        {
            Client = new MySqlClient(connectionString);
        }

        public int UpdateEntityCategory(string category)
        {
            var ds = Client.ExecuteStoredProcedure(
                "i_updateEntityCategory",
                new Dictionary<string, object> 
                { 
                    { "@_category", category.ToString() } 
                });

            return Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
        }

        const int ELLIPSIS_LENGTH = 3;

        string FormatValue(string name, string value, MySqlParameterCollection args)
        {
            var v = MySqlHelper.EscapeString(value ?? string.Empty);

            int maxLength = args[name].Size;

            if (v.Length <= maxLength) return v;

            var sb = new StringBuilder(v.Substring(0, maxLength - ELLIPSIS_LENGTH));
            sb.Append('.', ELLIPSIS_LENGTH);
            return sb.ToString();
        }

        MySqlParameterCollection DeriveParameters(string spName)
        {
            // Check the cache or update it.
            MySqlParameterCollection dps;
            return
                DerivedParameters.TryGetValue(spName, out dps)
                ?
                dps // parameter declaration is cached - return it.
                :
                DerivedParameters[spName] = Client.DeriveParameters(spName);    // Actively cache the parameter declaration.
        }

        public UpdateEntityResult UpdateEntity(string name, int categoryId, string subCategory, string blurb, string imageUrl, int nativePopularity)
        {
            var args = DeriveParameters("i_updateEntity");

            var ds = Client.ExecuteStoredProcedure(
                "i_updateEntity",
                new Dictionary<string, object>
                {
                    { "@_name",                 FormatValue("@_name", name, args) },
                    { "@_category_id",          categoryId },
                    { "@_sub_category",         FormatValue("@_sub_category", subCategory, args) },
                    { "@_blurb",                FormatValue("@_blurb", blurb, args) },
                    { "@_image_url",            FormatValue("@_image_url", imageUrl, args) },
                    { "@_native_popularity",    nativePopularity },
                });

            return new UpdateEntityResult()
            {
                InsertOccurred = Convert.ToInt32(ds.Tables[0].Rows[0]["inserted"]) > 0,
                UpdateOccurred = Convert.ToInt32(ds.Tables[0].Rows[0]["updated"]) > 0,
            };
        }

        public GetPostImportStatsResult GetPostImportStats()
        {
            var ds = Client.ExecuteStoredProcedure(
                "i_getPostImportStats",
                null);

            var result = new GetPostImportStatsResult();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                result.Items.Add(new GetPostImportStatsResult.Item()
                {
                    Category = Convert.ToString(r["category"]),
                    Count = Convert.ToInt32(r["count"]),
                });
            }

            return result;
        }
        
        public int VoteEntity(int id, bool up, int by)
        {
            if (by < 0)
                throw new ArgumentException("by must by >0");
            
            var ds = Client.ExecuteStoredProcedure(
                "c_voteEntity",
                new Dictionary<string, object>
                {
                    { "@_id", id },
                    { "@_direction", up ? 1 : -1 },
                    { "@_by", by },
                });

            return Convert.ToInt32(ds.Tables[0].Rows[0]["score"]);
        }

        public List<Entity> GetEntities()
        {
            var ds = Client.ExecuteStoredProcedure(
                "v_getEntities",
                null);

            return ds.Tables[0].Rows.OfType<DataRow>().Select(r =>
                new Entity()
                {
                    Id = Convert.ToInt32(r["id"]),
                    CategoryId = Convert.ToInt32(r["category_id"]),
                    Category = Convert.ToString(r["category"]),
                    Name = Convert.ToString(r["name"]),
                    NativePopularity = Convert.ToInt32(r["native_popularity"]),
                    Score = Convert.ToInt32(r["score"]),
                    TotalVotes = Convert.ToInt32(r["total_votes"]),
                })
                .ToList();
        }
    }
}
