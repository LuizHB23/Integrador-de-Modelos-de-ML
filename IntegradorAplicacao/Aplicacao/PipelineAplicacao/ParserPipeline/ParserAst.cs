using IntegradorDominio.AST;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ParserPipeline
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
            var idx = IndexOfAtribuicao(linha);

            var variavel = linha.Substring(0, idx).Trim();
            var expressao = linha.Substring(idx + 1).Trim();

            return new AtribuicaoMetodoPipeline(variavel, ParseChamadaMetodo(expressao));
        }

        private ChamadaMetodoPipeline ParseChamadaMetodo(string expressao)
        {
            var partes = SplitPorPontoMetodo(expressao);

            var objetoInicial = partes[0];

            var chamada = new ChamadaMetodoPipeline(objetoInicial);

            for (int i = 1; i < partes.Count; i++)
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

            var partes = SplitInteligente(argumentos);

            foreach (var parte in partes)
            {
                var valorPartes = parte.Trim();

                int idxPrimeiroIgual = valorPartes.IndexOf('=');

                if (idxPrimeiroIgual > 0)
                {
                    var nomeParametro = valorPartes.Substring(0, idxPrimeiroIgual).Trim();

                    // O valor é TUDO o que vem depois do primeiro '=', sem mais splits
                    var valorParametro = valorPartes.Substring(idxPrimeiroIgual + 1).Trim();

                    // Limpeza padrão de aspas para strings simples
                    valorParametro = valorParametro.Trim().Trim('"');

                    lista.Add(new ArgumentoMetodoPipeline(nomeParametro, valorParametro));
                }
                else
                {
                    lista.Add(new ArgumentoMetodoPipeline(null, valorPartes));
                }
            }

            return lista;
        }

        private List<string> SplitInteligente(string input)
        {
            var resultado = new List<string>();
            var atual = new StringBuilder();

            int nivelColchetes = 0;
            int nivelChaves = 0;
            bool dentroAspas = false;

            foreach (char caracter in input)
            {
                if (caracter == '"')
                {
                    dentroAspas = !dentroAspas;
                }

                if (!dentroAspas)
                {
                    if (caracter == '[') nivelColchetes++;
                    else if (caracter == ']') nivelColchetes--;

                    else if (caracter == '{') nivelChaves++;
                    else if (caracter == '}') nivelChaves--;
                }

                if (caracter == ',' && !dentroAspas && nivelColchetes == 0 && nivelChaves == 0)
                {
                    resultado.Add(atual.ToString().Trim());
                    atual.Clear();
                }
                else
                {
                    atual.Append(caracter);
                }
            }

            if (atual.Length > 0)
                resultado.Add(atual.ToString().Trim());

            return resultado;
        }

        private int IndexOfAtribuicao(string texto)
        {
            bool dentroAspas = false;
            int nivelParenteses = 0;

            for (int i = 0; i < texto.Length; i++)
            {
                char c = texto[i];

                if (c == '"')
                    dentroAspas = !dentroAspas;

                if (!dentroAspas)
                {
                    if (c == '(') nivelParenteses++;
                    else if (c == ')') nivelParenteses--;

                    if (c == '=' && nivelParenteses == 0)
                    {
                        bool ehComparador =
                            (i > 0 && (texto[i - 1] == '>' || texto[i - 1] == '<' || texto[i - 1] == '!' || texto[i - 1] == '=')) ||
                            (i < texto.Length - 1 && texto[i + 1] == '=');

                        if (!ehComparador)
                            return i;
                    }
                }
            }

            return -1;
        }
        private List<string> SplitPorPontoMetodo(string input)
        {
            var resultado = new List<string>();
            var atual = new StringBuilder();

            bool dentroAspas = false;
            int nivelParenteses = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '"')
                    dentroAspas = !dentroAspas;

                if (!dentroAspas)
                {
                    if (c == '(') nivelParenteses++;
                    else if (c == ')') nivelParenteses--;

                    // 🔥 só quebra no ponto certo
                    if (c == '.' && nivelParenteses == 0)
                    {
                        // verifica se é número decimal
                        bool ehDecimal = EhPontoDecimal(input, i);

                        if (!ehDecimal)
                        {
                            resultado.Add(atual.ToString().Trim());
                            atual.Clear();
                            continue;
                        }
                    }
                }

                atual.Append(c);
            }

            if (atual.Length > 0)
                resultado.Add(atual.ToString().Trim());

            return resultado;
        }

        private bool EhPontoDecimal(string input, int index)
        {
            // precisa ter algo antes e depois
            if (index <= 0 || index >= input.Length - 1)
                return false;

            // verifica se antes tem número (incluindo vários dígitos)
            int i = index - 1;
            bool temNumeroAntes = false;

            while (i >= 0 && char.IsDigit(input[i]))
            {
                temNumeroAntes = true;
                i--;
            }

            // verifica se depois tem número
            i = index + 1;
            bool temNumeroDepois = false;

            while (i < input.Length && char.IsDigit(input[i]))
            {
                temNumeroDepois = true;
                i++;
            }

            return temNumeroAntes && temNumeroDepois;
        }
    }
}