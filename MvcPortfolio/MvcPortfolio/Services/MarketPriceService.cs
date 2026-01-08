using Microsoft.EntityFrameworkCore.Storage;
using MvcPortfolio.Models;
using System.Globalization;
using System.Text.Json;
using StackExchange.Redis;

namespace MvcPortfolio.Services
{
    public class MarketPriceService
    {
        private readonly HttpClient _httpClient;

        public MarketPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IncrementCallCount()
        {
            string text = await File.ReadAllTextAsync("CallCounter.txt");
            string[] parts = text.Split(' ');  // split by space

            int count = int.Parse(parts[0]) + 1;   // first part is the number
            string datePart = parts.Length > 1 ? parts[1] : null;  // second part (optional)

            if (DateTime.TryParse(datePart, out DateTime date))
            {
                Console.WriteLine(date);
            }
            else
            {
                Console.WriteLine("Invalid date format");
            }

            /*
                        string text = await File.ReadAllTextAsync("callCounter.txt");
                        int count = int.Parse(text.Trim()) + 1;
                        await File.WriteAllTextAsync("CallCounter.txt", count.ToString() + DateTime.Now.Date);
            */
            if(date != DateTime.Now.Date)
            {
                Console.WriteLine(date + " and " + DateTime.Now.Date);
                count = 1;
            }


            await File.WriteAllTextAsync("CallCounter.txt", count.ToString() + " " + DateTime.Now.Date);

            return true;
        }

        public async Task<int> GetCallCount()
        {
            /*
            string text = await File.ReadAllTextAsync("callCounter.txt");
            int count = int.Parse(text.Trim());
            */
            string text = await File.ReadAllTextAsync("CallCounter.txt");
            string[] parts = text.Split(' ');  // split by space

            int count = int.Parse(parts[0]);   // first part is the number

            return count;
        }

        public async Task<decimal> GetCurrentPriceAsync(string ticker, string exchange)
        {
            IncrementCallCount();

            string apiKey = "RVXOF6QX9557RFJU"; 
            string url =
                $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY" +
                $"&symbol={ticker}.{exchange}&outputsize=compact&apikey={apiKey}";

            Console.WriteLine(url);

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
            Console.WriteLine(document.RootElement.ToString());
            return finalValue;
        }

        /*
        public async Task<Dictionary<string, decimal>> GetCurrentPricesAsync(IEnumerable<string> tickers, IEnumerable<string> exchanges)
        {
            string apiKey = "RVXOF6QX9557RFJU";
            string url =
                $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY" +
                $"&symbol={ticker}.{exchange}&outputsize=compact&apikey={apiKey}";

            //Console.WriteLine(url);

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(url);

            // 1. Convert tickers to API format
            var symbols = string.Join(",", tickers);

            // 2. Call API (example URL)
            var response = await _httpClient.GetAsync(
                $"quotes?symbols={symbols}");

            response.EnsureSuccessStatusCode();

            // 3. Parse response
            var json = await response.Content.ReadAsStringAsync();
            
            return ParsePrices(json);
        } 
        */

        private Dictionary<string, decimal> ParsePrices(string json)
        {
            // deserialize JSON → Dictionary<string, decimal>
            throw new NotImplementedException();
        }
    }
}
