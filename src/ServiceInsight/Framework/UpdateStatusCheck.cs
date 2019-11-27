namespace ServiceInsight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    //using System.Threading.Tasks;
    using RestSharp;
    using RestSharp.Deserializers;
    using ServiceInsight.ExtensionMethods;

    public class UpdateStatusCheck
    {
        public string CheckTheLatestVersion()
        //public async Task<string> CheckTheLatestVersion()
        {
            var client = new RestClient("https://s3.us-east-1.amazonaws.com");

            var request = new RestRequest("platformupdate.particular.net/serviceinsight.txt")
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };

            client.ClearHandlers();
            client.AddJsonDeserializer(new JsonDeserializer());
            
            var rs = client.Execute<List<Release>>(request);
            //var rs = await client.ExecuteTaskAsync<List<Release>>(request);

            return rs.Data.OrderByDescending(r=>r.Published).First().Tag;
            //return rs.Data.First().Tag;
        }


        class Release
        {
            public string Tag { get; set; }
            public DateTime Published { get; set; }
        }
    }
}
