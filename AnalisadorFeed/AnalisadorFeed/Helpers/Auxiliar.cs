using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnalisadorFeed.Helpers
{
    public static class Auxiliar
    {
        public static string RetirarHtml(string texto)
        {
            string textoSquebra = Regex.Replace(texto, @"\t|\n|\r", "");

            return WebUtility.HtmlDecode(Regex.Replace(textoSquebra, "\\<[^\\>]*\\>", String.Empty));
        }

        public static List<string> RetirarPalavrasDeParada(string texto, Arquivo arquivo)
        {
            texto = texto.Replace(".", "").Replace(",", "");

            List<string> stop_words = arquivo.ObterLista();

            List<string> lst = texto.Split(' ').ToList();

            stop_words.ForEach((item) =>
            {
                lst.RemoveAll(x => x == item.Trim());
            });

            return lst;
        }
    }
}
