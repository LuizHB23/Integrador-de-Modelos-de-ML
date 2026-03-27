using IntegradorDominio.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.Interfaces
{
    public interface IFeatureExecutor<T> where T : class
    {
        T Operacao { get; }
        DataFrame Executar(DataFrame df);
    }
}
