using RestSharp;
using System.Net;

namespace DownloadImagens
{
    public class DownloadService
    {
        private readonly string _storage;

        public DownloadService()
        {
            _storage = $"C://DropBox/DropBox/Katya2/";
        }

        public void ExecutarDownload()
        {
            var imagens = new List<string>();

            var urlGaleria = "https://www.listal.com/katya-boldareva/pictures/";

            var paginas = new List<string> { "1", "2", "3", "4", "5", "6", "7" };

            foreach (var pagina in paginas)
            {
                Console.WriteLine(pagina);

                var clientPagina = new RestClient(urlGaleria + pagina);

                var requestPagina = new RestRequest(string.Empty, Method.Get);

                var retornoPagina = clientPagina.Get(requestPagina);

                if (retornoPagina.StatusCode == HttpStatusCode.OK)
                {
                    var tableHtml = (retornoPagina?.Content?.Split("\n")) ?? throw new Exception();

                    tableHtml = tableHtml.Where(x => x.Contains("viewimage")).ToArray();

                    tableHtml = tableHtml.Where(x => x.Contains("imagewrap-inner")).ToArray();

                    foreach (var linhaHtml in tableHtml)
                    {
                        var urlLinhaHtml = linhaHtml.Replace("<div class='imagewrap-inner' style='padding-top:px'><a href='", string.Empty);
                        urlLinhaHtml = urlLinhaHtml.Replace("<div class='imagewrap-inner' style='padding-top:5px'><a href='", string.Empty);
                        urlLinhaHtml = urlLinhaHtml.Replace("<div class='imagewrap-inner' style='padding-top:14px'><a href='", string.Empty);
                        urlLinhaHtml = urlLinhaHtml.Replace("<div class='imagewrap-inner' style='padding-top:1px'><a href='", string.Empty);
                        urlLinhaHtml = urlLinhaHtml.Replace("<div class='imagewrap-inner' style='padding-top:38px'><a href='", string.Empty);

                        var urlLinha = urlLinhaHtml.Split("'");

                        urlLinha[0] = urlLinha[0].Replace("\t", string.Empty);
                        urlLinha[0] = urlLinha[0].Replace("https://www.listal.com/viewimage/", string.Empty);

                        var urlCompleta = "https://iv1.lisimg.com/image/" + urlLinha[0] + "/10000full-katya-boldareva.jpg";

                        Console.WriteLine(urlCompleta);

                        imagens.Add(urlCompleta);
                    }
                }
            }

            var total = imagens.Count;
            var contador = 1;

            foreach (var imagem in imagens)
            {
                Console.WriteLine("Baixando a Imagem " + contador + " De " + total);

                bool tentarNovamente = true;

                while (tentarNovamente)
                {
                    try
                    {
                        var options = new RestClientOptions(imagem)
                        {
                            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                        };

                        var clientDownload = new RestClient(options);

                        var requestDownload = new RestRequest(string.Empty, Method.Get);

                        var retornoDownload = clientDownload.Get(requestDownload);

                        if (requestDownload == null || retornoDownload.RawBytes == null)
                            throw new Exception();

                        File.WriteAllBytes(_storage + "Katya_" + Guid.NewGuid() + ".jpeg", retornoDownload.RawBytes);

                        tentarNovamente = false;

                        contador++;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Executando nova tentativa...");
                        tentarNovamente = true;
                    }
                }
            }
        }
    }
}
