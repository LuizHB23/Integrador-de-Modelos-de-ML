using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.ExpressionsNo;
using IntegradorDominio.FeatureEngineering.MapearLinhas.Variavel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas.ExpressionTrees.For
{
    public class ForExpression : NodeExpression
    {
        public string NomeVariavel { get; set; }
        public NodeExpression Inicializador { get; set; }
        public NodeExpression Condicao { get; set; }
        public NodeExpression Incremento { get; set; }
        public List<NodeExpression> Corpo { get; set; } = new List<NodeExpression>();

        public ForExpression(string nomeVariavel, NodeExpression inicializador, NodeExpression condicao, NodeExpression incremento)
        {
            NomeVariavel = nomeVariavel;
            Inicializador = inicializador;
            Condicao = condicao;
            Incremento = incremento;
        }

        public override Expression ParaExpression(
            Dictionary<string, ParameterExpression> variaveis,
            Dictionary<string, object> contexto,
            ParameterExpression indexVar)
        {
            // 1️⃣ Cria variável do loop se não existir
            if (!contexto.TryGetValue(NomeVariavel, out var varObj))
            {
                var novaVariavel = new Variavel<object>(NomeVariavel, 1_000_000);
                contexto[NomeVariavel] = novaVariavel;
                varObj = novaVariavel;
            }

            var varConst = Expression.Constant(varObj);

            // 2️⃣ Inicializador, condição e incremento
            var initExpr = Inicializador.ParaExpression(variaveis, contexto, indexVar);
            var condExpr = Condicao.ParaExpression(variaveis, contexto, indexVar);
            var incExpr = Incremento.ParaExpression(variaveis, contexto, indexVar);

            // 3️⃣ Corpo do loop
            var corpoExpressions = Corpo.Select(c => c.ParaExpression(variaveis, contexto, indexVar)).ToList();
            corpoExpressions.Add(incExpr); // incrementa no final

            // 4️⃣ Loop While simulado
            var breakLabel = Expression.Label();
            var loop = Expression.Loop(
                Expression.IfThenElse(
                    Expression.Convert(condExpr, typeof(bool)),
                    Expression.Block(corpoExpressions),
                    Expression.Break(breakLabel)
                ),
                breakLabel
            );

            // 5️⃣ Bloco final
            return Expression.Block(
                initExpr,
                loop
            );
        }
    }
}
