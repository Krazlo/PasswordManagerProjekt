using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PwM_Library
{
    public class HttpService
    {
        private readonly HttpClient _client;

        public HttpService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.None
            };

            _client = new HttpClient();
        }

        public async Task<string> GetAsync(string uri)
        {
            using HttpResponseMessage response = await _client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
