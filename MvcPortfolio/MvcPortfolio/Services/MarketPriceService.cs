using System.Globalization;
using System.Text.Json;

namespace MvcInvest.Services
{
    public class MarketPriceService
    {
        private readonly HttpClient _httpClient;

        public MarketPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetCurrentPriceAsync(string ticker, string exchange)
        {
            string apiKey = "RVXOF6QX9557RFJU"; 
            string url =
                $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY" +
                $"&symbol={ticker}.{exchange}&outputsize=compact&apikey={apiKey}";

            //Console.WriteLine(url);

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(json);

            // Navigate to "Time Series (Daily)"
            if (!document.RootElement.TryGetProperty("Time Series (Daily)", out JsonElement timeSeries))
                throw new Exception("Price data not found. Check ticker or API limit.");

            // Get the most recent trading day
            JsonProperty latestDay = timeSeries.EnumerateObject().First();

            // You can choose open or close; here we use close
            string closePriceString = latestDay.Value
                .GetProperty("4. close")
                .GetString();

            decimal finalValue = decimal.Parse(closePriceString, CultureInfo.InvariantCulture);
            //Console.WriteLine(document.RootElement.ToString());
            return finalValue;
        }

        /*
        public async Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string> tickers)
        {
            
            // 1. Convert tickers to API format
            var symbols = string.Join(",", tickers);

            // 2. Call API (example URL)
            var response = await _httpClient.GetAsync(
                $"quotes?symbols={symbols}");

            response.EnsureSuccessStatusCode();

            // 3. Parse response
            var json = await response.Content.ReadAsStringAsync();
            
            return ParsePrices(json);
        } */

        private Dictionary<string, decimal> ParsePrices(string json)
        {
            // deserialize JSON → Dictionary<string, decimal>
            throw new NotImplementedException();
        }
    }
}
