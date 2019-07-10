using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using AnalisadorFeed.Config;
using AnalisadorFeed.Helpers;
using AnalisadorFeed.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AnalisadorFeed.Controllers
{
    [Route("api/[controller]")]
    public class RSSFeedController : Controller
    {
        private readonly IOptions<FeedConfig> config;
        private readonly Arquivo _arquivo;

        public RSSFeedController(IOptions<FeedConfig> iConfig, IHostingEnvironment hostingEnvironmen)
        {
            _arquivo = new Arquivo(hostingEnvironmen);
            config = iConfig;
        }


        // GET api/values
        [HttpGet]
        public IActionResult Get(int qnt = 10)
        {
            try
            {

                XmlReader reader = XmlReader.Create(config.Value.Url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();


                //Obtem os topicos
                List<Topico> topicos = (from c in feed.Items.OrderByDescending(t => t.PublishDate).Take(qnt)
                                        select new Topico { Titulo = c.Title.Text, Sumario = Auxiliar.RetirarHtml(c.Summary.Text), DataPublicacao = c.PublishDate.UtcDateTime }).ToList();




                //Obtem o texto full de todos os sumarios.
                string sumarios = topicos.Select(i => i.Sumario).Aggregate((i, j) => i + " " + j);



                //Retira as stopwords do texto.
                List<String> texto = Auxiliar.RetirarPalavrasDeParada(sumarios,_arquivo);


                //Faz o rank geral de palavras.
                //O rank só levou em conta o sumario dos topicos.
                var rank = texto.GroupBy(x => x)
                   .Select(x => new
                   {
                       palavra = x.Key,
                       incidencia = x.Count()
                   })
                   .OrderByDescending(x => x.incidencia)
                   .Take(10).ToList();


                return StatusCode(200, new
                {
                    topicos,
                    rankpalavras = rank
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ops! Ocorreu um erro no processamento");
            }
        }
    }
}