using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    internal class DividirExecutor : FeatureExecutorBase<Dividir>
    {
        public DividirExecutor(Dividir operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                resultado[i] = dataFrame.PegarColuna<float>(Operacao.left).Dados[i] / dataFrame.PegarColuna<float>(Operacao.left).Dados[i];
            }

            dataFrame.AlteraColuna(Operacao.exit, resultado);

            return dataFrame;
        }
    }
}
