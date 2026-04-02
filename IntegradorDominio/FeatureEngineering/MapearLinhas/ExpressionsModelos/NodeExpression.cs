using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.MapearLinhas.ExpressionsModelos
{
    public abstract class NodeExpression
    {
        public abstract Expression ParaExpression(Dictionary<string, ParameterExpression> variaveis, Dictionary<string, object> contexto, ParameterExpression indexVar);
    }
}
