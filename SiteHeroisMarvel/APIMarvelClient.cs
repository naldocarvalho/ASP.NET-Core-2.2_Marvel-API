using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SiteHeroisMarvel.Models;

namespace SiteHeroisMarvel
{
    public class APIMarvelClient
    {
        private static string[] HEROIS = new string[]
        {
            "Captain America", "Iron Man", "Thor", "Hulk",
            "Wolverine", "Spider-Man", "Black Panther",
            "Doctor Strange", "Daredevil"
        };

        private HttpClient _client;
        private IConfiguration _configuration;

        public APIMarvelClient(
            HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _configuration = configuration;
        }

        public Personagem ObterDadosPersonagem()
        {
            string heroi = HEROIS[new Random().Next(0, 9)];

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string ts = DateTime.Now.Ticks.ToString();
            string publicKey =
                _configuration.GetSection("MarvelComicsAPI:PublicKey").Value;
            string hash = GerarHash(ts, publicKey,
                _configuration.GetSection("MarvelComicsAPI:PrivateKey").Value);

            string url = _configuration.GetSection("MarvelComicsAPI:BaseURL").Value +
                $"characters?ts={ts}&apikey={publicKey}&hash={hash}&" +
                $"name={Uri.EscapeUriString(heroi)}";
            HttpResponseMessage response = _client.GetAsync(
                url).Result;

            response.EnsureSuccessStatusCode();
            string conteudo =
                response.Content.ReadAsStringAsync().Result;

            dynamic resultado = JsonConvert.DeserializeObject(conteudo);

            Personagem personagem = new Personagem();
            personagem.Nome = resultado.data.results[0].name;
            personagem.Descricao = resultado.data.results[0].description;
            personagem.UrlImagem = resultado.data.results[0].thumbnail.path + "." +
                resultado.data.results[0].thumbnail.extension;
            personagem.UrlWiki = resultado.data.results[0].urls[1].url;

            return personagem;
        }

        private string GerarHash(
            string ts, string publicKey, string privateKey)
        {
            byte[] bytes =
                Encoding.UTF8.GetBytes(ts + privateKey + publicKey);
            var gerador = MD5.Create();
            byte[] bytesHash = gerador.ComputeHash(bytes);
            return BitConverter.ToString(bytesHash)
                .ToLower().Replace("-", String.Empty);
        }
    }
}