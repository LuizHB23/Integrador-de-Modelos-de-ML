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

            foreach (var op in new[] { "*", "/", "+", "-" })
            {
                int idx = expr.IndexOf(op);
                if (idx > 0)
                {
                    var esquerda = expr.Substring(0, idx);
                    var direita = expr.Substring(idx + 1);
                    return new BinarioExpression(
                        ParseExpression(esquerda, contexto, defaultDf),
                        op,
                        ParseExpression(direita, contexto, defaultDf)
                    );
                }
            }

            // agora aceita só o nome da coluna
            if (contexto.TryGetValue(defaultDf, out var dfObj) && dfObj is DataFrame df)
            {
                var colName = expr.Trim('\'', '\"').Trim();
                var colunaBase = df.PegarColunaBase(colName);
                if (colunaBase != null)
                    return new ColunaExpression(defaultDf, colName, colunaBase.TipoDado);
            }

            // tenta converter para número
            if (Single.TryParse(expr, out var floatVal))
                return new ValueExpression(floatVal);

            return new VariavelUsoExpression(expr);
        }
    }
}
