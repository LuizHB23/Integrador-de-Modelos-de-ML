using IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha;
using System.Text;
using System.Xml;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser
{
    public class ParserMap
    {
        public List<NodeMap> Parse(string input)
        {
            input = input.Trim();

            if (input.StartsWith("["))
                input = input.Substring(1, input.Length - 2);

            var partes = SplitInteligente(input);

            var lista = new List<NodeMap>();

            foreach (var parte in partes)
            {
                lista.Add(ParseElemento(parte));
            }

            return lista;
        }

        private NodeMap ParseElemento(string trecho)
        {
            trecho = trecho.Trim();

            if (trecho.StartsWith("line"))
                return ParseLine(trecho);

            if (trecho.StartsWith("if"))
                return ParseIf(trecho);

            if (trecho.StartsWith("for"))
                return ParseFor(trecho);

            throw new Exception($"DSL inválida: {trecho}");
        }

        private NodeMap ParseLine(string trecho)
        {
            var idx = trecho.IndexOf(':');
            var valor = trecho.Substring(idx + 1).Trim().Trim('"');

            return new LineMap(valor);
        }

        private NodeMap ParseIf(string trecho)
        {
            var conteudo = ExtrairConteudoChaves(trecho);
            var partes = SplitInteligente(conteudo);

            string condicao = null;
            var corpo = new List<NodeMap>();

            foreach (var parte in partes)
            {
                if (parte.StartsWith("condicion"))
                    condicao = ExtrairValor(parte);
                else
                    corpo.Add(ParseElemento(parte));
            }

            return new IfMap(condicao, corpo);
        }

        private NodeMap ParseFor(string trecho)
        {
            var conteudo = ExtrairConteudoChaves(trecho);
            var partes = SplitInteligente(conteudo);

            string condicao = null;
            var corpo = new List<NodeMap>();

            foreach (var parte in partes)
            {
                if (parte.StartsWith("condicion"))
                    condicao = ExtrairValor(parte);
                else
                    corpo.Add(ParseElemento(parte));
            }

            return new ForMap(condicao, corpo);
        }

        private string ExtrairConteudoChaves(string texto)
        {
            var inicio = texto.IndexOf('{') + 1;
            var fim = texto.LastIndexOf('}');
            return texto.Substring(inicio, fim - inicio);
        }

        private string ExtrairValor(string parte)
        {
            var idx = parte.IndexOf(':');
            return parte.Substring(idx + 1).Trim().Trim('"');
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
                    dentroAspas = !dentroAspas;

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
    }
}
