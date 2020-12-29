using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShowIssueTracker.Models
{
    public class RestClientNew
    {
        public RestClientNew(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            Headers = new Dictionary<string, string>();
        }

        public RestClientNew(string endpoint, HttpVerb method, string postData, Dictionary<string, string> headerDictionary)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            Headers = headerDictionary;
        }

        public string ContentType { get; set; }

        public string EndPoint { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public HttpVerb Method { get; set; }

        public string PostData { get; set; }

        public async Task<string> MakeRequest()
        {
            return await MakeRequest(string.Empty);
        }

        public async Task<string> MakeRequest(string parameters)
        {
            var url = EndPoint + parameters;
            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                using (var client = new HttpClient())
                {
                    foreach (var header in Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    HttpContent cnt = new StringContent(PostData, Encoding.UTF8, "application/json");

                    using (var result = await client.PostAsync(url, cnt))
                    {
                        if (result.StatusCode != HttpStatusCode.OK)
                        {

                            var status = result.StatusCode.ToString();
                            return status;
                        }

                        using (var content = result.Content)
                        {
                            return await content.ReadAsStringAsync();
                        }
                    }

                }
            }

            else
            {


                using (var client = new HttpClient())
                {
                    foreach (var header in Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    using (var response = await client.GetAsync(url))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            var status = response.StatusCode.ToString();
                            return status;
                        }

                        using (var content = response.Content)
                        {
                            return await content.ReadAsStringAsync();
                        }
                    }
                }
            }
        }
    }
}
