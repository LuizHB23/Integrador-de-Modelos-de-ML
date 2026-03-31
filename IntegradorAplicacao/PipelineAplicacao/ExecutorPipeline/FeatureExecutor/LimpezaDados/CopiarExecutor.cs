using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class CopiarExecutor : FeatureExecutorBase<Copiar>
    {
        public CopiarExecutor(Copiar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var novoDataFrame = new DataFrame();

            foreach (var coluna in dataFrame.Colunas)
            {
                novoDataFrame.Colunas.Add(coluna);
            }

            return novoDataFrame;
        }
    }
}
