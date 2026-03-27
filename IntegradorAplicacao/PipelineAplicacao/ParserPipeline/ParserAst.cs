using IntegradorDominio.Pipeline.AST;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ParserPipeline
{
    public class ParserAst
    {
        public Dictionary<string, List<string>> ParserCorpo(string codigo)
        {
            var linhas = codigo
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            var corpo = linhas
                .Skip(1)
                .Where(l => l != "{" && l != "}")
                .ToList();

            var metodoNomeCorpo = new Dictionary<string, List<string>>();
            metodoNomeCorpo.Add(ExtrairNomeMetodo(linhas[0]), corpo);

            return metodoNomeCorpo;
        }

        public MetodoPipeline Parse(Dictionary<string, List<string>> metodoNomeCorpo)
        {             
            var metodoElemento = metodoNomeCorpo.First();
            var nomeMetodo = metodoElemento.Key;
            var metodo = new MetodoPipeline(nomeMetodo);

            foreach (var linha in metodoElemento.Value)
            {
                metodo.Comandos.Add(ParseLinha(linha));
            }

            return metodo;
        }

        private string ExtrairNomeMetodo(string linha)
        {
            var idx = linha.IndexOf("(");
            return linha.Substring(0, idx);
        }

        private ComandoMetodoPipeline ParseLinha(string linha)
        {
            if (linha.StartsWith("return"))
                return ParseReturn(linha);

            return ParseAtribuicao(linha);
        }

        private RetornoMetodoPipeline ParseReturn(string linha)
        {
            var valor = linha.Replace("return", "").Trim();

            return new RetornoMetodoPipeline(valor);
        }

        private AtribuicaoMetodoPipeline ParseAtribuicao(string linha)
        {
            var idx = linha.IndexOf('=');

            var variavel = linha.Substring(0, idx).Trim();
            var expressao = linha.Substring(idx + 1).Trim();

            return new AtribuicaoMetodoPipeline(variavel, ParseExpressao(expressao));
        }

        private ExpressaoMetodoPipeline ParseExpressao(string expressao)
        {
            var partes = expressao.Split('.');

            var objetoInicial = partes[0];

            var chamada = new ChamadaMetodoPipeline(objetoInicial);

            for (int i = 1; i < partes.Length; i++)
            {
                var parte = partes[i];

                var idxParenteses = parte.IndexOf("(");
                var nomeMetodo = parte.Substring(0, idxParenteses);

                var argumentos = parte.Substring(idxParenteses + 1).TrimEnd(')');

                chamada.Metodos.Add(new MetodoChainPipeline(nomeMetodo, ParseArgumentos(argumentos)));
            }

            return chamada;
        }

        private List<ArgumentoMetodoPipeline> ParseArgumentos(string argumentos)
        {
            var lista = new List<ArgumentoMetodoPipeline>();

            if (string.IsNullOrWhiteSpace(argumentos))
                return lista;

            var partes = argumentos.Split(',');

            foreach (var parte in partes)
            {
                var valorPartes = parte.Trim();

                if (valorPartes.Contains("="))
                {
                    var valor = valorPartes.Split('=');

                    lista.Add(new ArgumentoMetodoPipeline(valor[0].Trim(), valor[1].Trim().Trim('"')));
                }
                else
                {
                    lista.Add(new ArgumentoMetodoPipeline(null, valorPartes));
                }
            }

            return lista;
        }
    }
}

