using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser
{
    public class ParserExpression
    {
        public NodeExpression ParseLine(string line, Dictionary<string, object> contexto, DataFrame dataFrame)
        {
            if (line.StartsWith("line:"))
                line = line.Substring("line:".Length).Trim();

            int eqIdx = line.IndexOf('=');
            if (eqIdx < 0) throw new Exception("Linha de atribuição inválida: " + line);

            var leftRaw = line.Substring(0, eqIdx).Trim();
            var rightRaw = line.Substring(eqIdx + 1).Trim();

            // Limpa aspas e espaços do left
            string left = leftRaw.Trim('\'', '\"').Trim();

            // Parse da expressão da direita
            NodeExpression rightNode = ParseExpression(rightRaw, contexto, dataFrame.NomeContexto);

            // Descobre tipo real da coluna
            Type tipoColuna = typeof(object); // fallback genérico
            if (contexto.TryGetValue(dataFrame.NomeContexto, out var dfObj) && dfObj is DataFrame df)
            {
                var colunaBase = df.PegarColunaBase(left); // agora aceita só o nome da coluna

                if (colunaBase != null)
                {
                    tipoColuna = colunaBase.TipoDado;
                    return new ColunaAtribuicaoExpression(dataFrame.NomeContexto, left, rightNode, tipoColuna);
                }
            }

            return new VariavelExpression(left, rightNode, dataFrame.QuantidadeLinhas);

        }

        private NodeExpression ParseExpression(string expr, Dictionary<string, object> contexto, string defaultDf)
        {
            expr = expr.Trim();

            // 1. TRATAMENTO DE PARÊNTESES (Se a expressão estiver toda entre parênteses, removemos)
            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                // Verifica se o parêntese que abre é o mesmo que fecha no final
                if (TemParentesesCorrespondentes(expr))
                    return ParseExpression(expr.Substring(1, expr.Length - 2), contexto, defaultDf);
            }

            // 2. PRECEDÊNCIA MATEMÁTICA (Procuramos o operador de MENOR precedência primeiro)
            // Procuramos de trás para frente para manter a associatividade à esquerda
            string[] operadores = { "+", "-", "*", "/" };

            // Primeiro procuramos + e -, depois * e /
            // Isso garante que o + seja a "raiz" e o * seja feito antes
            foreach (var op in new[] { "+", "-", "*", "/" })
            {
                int nivelParenteses = 0;
                for (int i = expr.Length - 1; i >= 0; i--)
                {
                    char c = expr[i];
                    if (c == ')') nivelParenteses++;
                    else if (c == '(') nivelParenteses--;

                    // Só quebramos se o operador estiver fora de qualquer parêntese
                    if (nivelParenteses == 0 && expr[i].ToString() == op)
                    {
                        var esquerda = expr.Substring(0, i);
                        var direita = expr.Substring(i + 1);
                        return new BinarioExpression(
                            ParseExpression(esquerda, contexto, defaultDf),
                            op,
                            ParseExpression(direita, contexto, defaultDf)
                        );
                    }
                }
            }

            // 3. IDENTIFICAÇÃO DE FOLHAS (Números, Colunas ou Variáveis)

            // Tenta converter para número
            if (Single.TryParse(expr, out var floatVal))
                return new ValueExpression(floatVal);

            // Tenta ver se é uma coluna no DataFrame
            if (contexto.TryGetValue(defaultDf, out var dfObj) && dfObj is DataFrame df)
            {
                var colName = expr.Trim('\'', '\"').Trim();
                var colunaBase = df.PegarColunaBase(colName);
                if (colunaBase != null)
                    return new ColunaExpression(defaultDf, colName, colunaBase.TipoDado);
            }

            // Se não é nada acima, é o uso de uma variável (como o seu 'x')
            return new VariavelUsoExpression(expr.Trim());
        }

        // Método auxiliar para validar parênteses
        private bool TemParentesesCorrespondentes(string s)
        {
            int nivel = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(') nivel++;
                else if (s[i] == ')') nivel--;
                if (nivel == 0 && i < s.Length - 1) return false;
            }
            return nivel == 0;
        }
    }
}
