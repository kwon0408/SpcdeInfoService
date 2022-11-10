using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpcdeInfoService
{
    public class SpcdeInfo
    {
        private readonly string apiKey;
        private string ApiKeyEncoded 
            => apiKey.Replace("+", "%2B").Replace("/", "%2F").Replace("=", "%3D");
        public static Version ApiVersion => new(1, 4);


        public SpcdeInfo(string apiKey)
        {
            this.apiKey =
                apiKey
                .Replace("%2B", "+").Replace("%2F", "/").Replace("%3D", "=")
                .Replace("%2b", "+").Replace("%2f", "/").Replace("%3d", "=");
        }

        public DateInfo[] GetHoliDeInfo(
            out int totalCount,
            int solYear, 
            int? solMonth = null,
            int numOfRows = 10,
            int pageNo = 1
            )
            => GetInfo("getHoliDeInfo", out totalCount, solYear, solMonth, numOfRows, pageNo);

        public DateInfo[] GetRestDeInfo(
            out int totalCount,
            int solYear, 
            int? solMonth = null,
            int numOfRows = 10,
            int pageNo = 1
            )
            => GetInfo("getRestDeInfo", out totalCount, solYear, solMonth, numOfRows, pageNo);

        public DateInfo[] GetAnniversaryInfo(
            out int totalCount,
            int solYear, 
            int? solMonth = null,
            int numOfRows = 10,
            int pageNo = 1
            )
            => GetInfo("getAnniversaryInfo", out totalCount, solYear, solMonth, numOfRows, pageNo);

        public DateInfo[] Get24DivisionsInfo(
            out int totalCount,
            int solYear,
            int? solMonth = null,
            int numOfRows = 10,
            int pageNo = 1
            )
            => GetInfo("get24DivisionsInfo", out totalCount, solYear, solMonth, numOfRows, pageNo);

        public DateInfo[] GetSundryDayInfo(
            out int totalCount,
            int solYear,
            int? solMonth = null,
            int numOfRows = 10,
            int pageNo = 1
            )
            => GetInfo("getSundryDayInfo", out totalCount, solYear, solMonth, numOfRows, pageNo);

        private DateInfo[] GetInfo(
            string endpoint,
            out int totalCount,
            int solYear,
            int? solMonth,
            int numOfRows,
            int pageNo
            )
        {
            string uri = $"https://apis.data.go.kr/B090041/openapi/service/SpcdeInfoService/{endpoint}?" +
                         $"_type=json&serviceKey={ApiKeyEncoded}&numOfRows={numOfRows}&pageNo={pageNo}&solYear={solYear}";
            if (solMonth is not null)
                uri += $"&solMonth={solMonth}";

            var get = Get<JObject>(uri);
            if (get?["response"]?["body"] is not JObject body // get failed
                || body["items"] is not JObject items // no items
                || !items.HasValues // items is empty
                || items["item"] is not JArray days)
            {
                totalCount = 0;
                return Array.Empty<DateInfo>();
            }            
            totalCount = Convert.ToInt32((body["totalCount"] as JValue)?.Value);
            return (from JObject d in days select d.ToObject<DateInfo>()).ToArray()
                ?? Array.Empty<DateInfo>();
        }

        private static T? Get<T>(string requestUri)
        {
            try
            {
                using HttpClient http = new(new HttpClientHandler() { UseCookies = true, CookieContainer = new() });

                var task = Task.Run(() => http.GetAsync(requestUri));
                task.Wait();
                var response = task.Result;


                var task2 = Task.Run(() => response.Content.ReadAsStringAsync());
                task2.Wait();
                var result = task2.Result;

                var t = JsonConvert.DeserializeObject<T>(
                    result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    });
                var x = t ?? default;
                return x;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
