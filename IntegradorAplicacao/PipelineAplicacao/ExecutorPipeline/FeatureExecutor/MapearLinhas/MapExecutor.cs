using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.For;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.If;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.MapearLinhas;
using IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha;
using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas
{
    public class MapExecutor : FeatureExecutorBase<Map>
    {
        private readonly ParserExpression _parserExpression;
        private readonly ParserMap _parserMap;
        private Dictionary<string, object> _caseLinhas;

        public MapExecutor(Map operacao) : base(operacao) 
        {
            _parserExpression = new();
            _parserMap = new();
            _caseLinhas = new();
        }

        public override object Executar(DataFrame dataFrame)
        {
            var resultado = _parserMap.Parse(Operacao.lambdax);

            // Constrói todas as expressões sem compilar
            var corpoExpressions = new List<NodeExpression>();
            foreach (var node in resultado)
            {
                corpoExpressions.AddRange(BuilderExpression(node, dataFrame));
            }

            // Cria um parâmetro único para o índice da linha
            var indexParam = Expression.Parameter(typeof(int), "i");
            var variaveis = new Dictionary<string, ParameterExpression>();

            // Monta a expressão final de todas as linhas
            var bloco = Expression.Block(corpoExpressions.Select(e => e.ParaExpression(variaveis, Operacao.Contexto, indexParam)));

            // Compila a lambda apenas aqui
            var lambda = Expression.Lambda<Action<int>>(bloco, indexParam).Compile();

            // Itera sobre cada linha do DataFrame
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
                lambda(i);

            return dataFrame;
        }

        private IEnumerable<NodeExpression> BuilderExpression(NodeMap node, DataFrame dataFrame)
        {
            switch (node)
            {
                case LineMap line:
                    return new[] { _parserExpression.ParseLine(line.Linha, Operacao.Contexto, dataFrame) };

                case IfMap ifNode:
                    return new[] { CriarIfExpression(ifNode, dataFrame) };

                case ForMap forNode:
                    return new[] { CriarForExpression(forNode, dataFrame) };

                default:
                    return Enumerable.Empty<NodeExpression>();
            }
        }

        private IfExpression CriarIfExpression(IfMap ifNode, DataFrame dataFrame)
        {
            var conditionExpr = _parserExpression.ParseExpression(ifNode.Condicao, Operacao.Contexto, dataFrame.NomeContexto);

            var ifExpr = new IfExpression(conditionExpr);

            // Corpo do IF
            foreach (var item in ifNode.Corpo)
                ifExpr.Body.AddRange(BuilderExpression(item, dataFrame));

            // Corpo do ELSE, se houver
            if (ifNode.Else != null)
                foreach (var item in ifNode.Else)
                    ifExpr.ElseBody.AddRange(BuilderExpression(item, dataFrame));

            return ifExpr;
        }

        private ForExpression CriarForExpression(ForMap forNode, DataFrame dataFrame)
        {
            var partes = forNode.Condicao.Split(';');
            var eqIdx = partes[0].IndexOf('=');
            string nomeVariavel = partes[0].Substring(0, eqIdx).Trim();

            var inicializador = _parserExpression.ParseLine(partes[0], Operacao.Contexto, dataFrame);
            var condicao = _parserExpression.ParseExpression(partes[1], Operacao.Contexto, dataFrame.NomeContexto);
            var incremento = _parserExpression.ParseLine(partes[2], Operacao.Contexto, dataFrame);

            var corpoNodes = new List<NodeExpression>();
            foreach (var item in forNode.Corpo)
            {
                if (item is LineMap line)
                    corpoNodes.Add(_parserExpression.ParseLine(line.Linha, Operacao.Contexto, dataFrame));
                else if (item is ForMap nestedFor)
                    corpoNodes.Add(CriarForExpression(nestedFor, dataFrame));
            }

            return new ForExpression(nomeVariavel, inicializador, condicao, incremento)
            {
                Corpo = corpoNodes
            };
        }
    }
}