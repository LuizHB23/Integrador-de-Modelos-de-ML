using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.For;
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
                        ExecutarForMap(forNode, dataFrame);
                        break;
                }
            }
        }

        private void ExecutarLineMap(LineMap line, DataFrame dataFrame)
        {
            // Garante que o contexto contém o DataFrame
            if (!Operacao.Contexto.ContainsKey("df"))
                Operacao.Contexto["df"] = dataFrame;

            var exprTree = _parserExpression.ParseLine(line.Linha, Operacao.Contexto, dataFrame);

            var indexParam = Expression.Parameter(typeof(int), "i");
            var variaveis = new Dictionary<string, ParameterExpression>();

            var assignExpr = exprTree.ParaExpression(variaveis, Operacao.Contexto, indexParam);

            var lambda = Expression.Lambda<Action<int>>(assignExpr, indexParam).Compile();

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
                lambda(i);
        }

        private void ExecutarForMap(ForMap forNode, DataFrame dataFrame)
        {
            var partes = forNode.Condicao.Split(';');
            if (partes.Length != 3) throw new Exception("Loop for inválido. Use: init; cond; inc");

            string initStr = partes[0].Trim();
            string condStr = partes[1].Trim();
            string incStr = partes[2].Trim();

            var eqIdx = initStr.IndexOf('=');
            string nomeVariavel = initStr.Substring(0, eqIdx).Trim();

            var indexParam = Expression.Parameter(typeof(int), "row");
            var variaveis = new Dictionary<string, ParameterExpression>();

            var inicializador = _parserExpression.ParseLine(initStr, Operacao.Contexto, dataFrame);
            var condicao = _parserExpression.ParseExpression(condStr, Operacao.Contexto, dataFrame.NomeContexto);
            var incremento = _parserExpression.ParseLine(incStr, Operacao.Contexto, dataFrame);

            var corpoNodes = new List<NodeExpression>();
            foreach (var item in forNode.Corpo)
            {
                if (item is LineMap line)
                    corpoNodes.Add(_parserExpression.ParseLine(line.Linha, Operacao.Contexto, dataFrame));
                else if (item is ForMap nestedFor)
                    corpoNodes.Add(CriarForExpression(nestedFor, dataFrame));
            }

            var forExpr = new ForExpression(nomeVariavel, inicializador, condicao, incremento)
            {
                Corpo = corpoNodes
            };

            var lambda = Expression.Lambda<Action<int>>(forExpr.ParaExpression(variaveis, Operacao.Contexto, indexParam), indexParam).Compile();

            // ✅ Executa o for por linha do DataFrame
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
                lambda(i);
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
