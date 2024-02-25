using Newtonsoft.Json.Linq;

namespace EDF_Zenflex_Exporter
{
    public class ApiCaller : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();

        public ApiCaller()
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            _client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        }

        public async Task<string> GetDayTypeAsync(string date)
        {
            string dayType = String.Empty;

            try
            {
                string url = $"https://particulier.edf.fr/services/rest/opm/getOPMStatut?dateRelevant={date}";
                using HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
#if DEBUG
                Console.WriteLine($"{date} {responseString}");                
#endif

                if (!string.IsNullOrEmpty(responseString))
                {
                    var responseJson = JObject.Parse(responseString);
                    dayType = responseJson["couleurJourJ"].Value<string>();
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.InnerException);
            }

            return dayType;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
