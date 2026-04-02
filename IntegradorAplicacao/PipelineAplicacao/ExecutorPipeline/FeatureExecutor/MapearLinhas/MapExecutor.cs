using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.Parser;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.MapearLinhas;
using IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha;
using IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos;
using System.Linq.Expressions;
using System.Xml;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas
{
    public class MapExecutor : FeatureExecutorBase<Map>
    {
        private readonly ParserMap _parserMap;
        private Dictionary<string, object> _caseLinhas;

        public MapExecutor(Map operacao) : base(operacao) 
        {
            _parserMap = new();
            _caseLinhas = new();
        }

        public override object Executar(DataFrame dataFrame)
        {
            var resultado = _parserMap.Parse(Operacao.lambdax);

            BuilderExpression(resultado, 0, dataFrame);

            return dataFrame;
        }

        private void BuilderExpression(List<NodeMap> nodes, int nivel, DataFrame dataFrame)
        {
            foreach (var node in nodes)
            {
                switch (node)
                {
                    case LineMap line:
                        ExecutarLineMap(line, dataFrame);
                        break;

                    case IfMap ifNode:
                        BuilderExpression(ifNode.Corpo, nivel + 1, dataFrame);
                        break;

                    case ForMap forNode:
                        BuilderExpression(forNode.Corpo, nivel + 1, dataFrame);
                        break;
                }
            }
        }

        private void ExecutarLineMap(LineMap line, DataFrame dataFrame)
        {
            // Garante que o contexto contém o DataFrame
            if (!Operacao.Contexto.ContainsKey("df"))
                Operacao.Contexto["df"] = dataFrame;

            var parserExpr = new ParserExpression();
            var exprTree = parserExpr.ParseLine(line.Linha, Operacao.Contexto, "df");

            var indexParam = Expression.Parameter(typeof(int), "i");
            var variaveis = new Dictionary<string, ParameterExpression>();

            var assignExpr = exprTree.ParaExpression(variaveis, Operacao.Contexto, indexParam);

            var lambda = Expression.Lambda<Action<int>>(assignExpr, indexParam).Compile();

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
                lambda(i);
        }
    }
}
