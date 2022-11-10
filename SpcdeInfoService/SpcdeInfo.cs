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
            totalCount = 0;

            string uri = $"https://apis.data.go.kr/B090041/openapi/service/SpcdeInfoService/{endpoint}?" +
                         $"_type=json&serviceKey={ApiKeyEncoded}&numOfRows={numOfRows}&pageNo={pageNo}&solYear={solYear}";
            if (solMonth is not null)
                uri += $"&solMonth={solMonth:00}";

            var get = Get<JObject>(uri);
            if (get?["response"]?["body"] is not JObject body) // failed to GET
            {
                return Array.Empty<DateInfo>();
            }

            if (body["items"] is not JObject items) // "items" not found
            {
                return Array.Empty<DateInfo>();
            }

            if (!items.HasValues) // items is empty
            {
                return Array.Empty<DateInfo>();
            }

            if (items["item"] is not JArray days) // has only 1 item
            {
                if (items["item"] is not JObject item
                    || item.ToObject<DateInfo>() is not DateInfo di)
                    return Array.Empty<DateInfo>();

                totalCount = 1;
                return new DateInfo[1] { di };
            }
            else // has >= 2 items
            {
                totalCount = Convert.ToInt32((body["totalCount"] as JValue)?.Value);
                return (from JObject d in days select d.ToObject<DateInfo>()).ToArray()
                    ?? Array.Empty<DateInfo>();
            }

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
