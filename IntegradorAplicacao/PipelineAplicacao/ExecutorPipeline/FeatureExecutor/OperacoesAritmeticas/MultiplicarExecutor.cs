using IntegradorAplicacao.PipelineAplicacao.Interfaces;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    public class MultiplicarExecutor : IFeatureExecutor<Multiplicar>
    {
        public Multiplicar Operacao { get; }

        public MultiplicarExecutor(Multiplicar operacao)
        {
            Operacao = operacao;
        }

        public DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                resultado[i] = Operacao.ColunaEsquerda.Dados[i] * Operacao.ColunaDireita.Dados[i];
            }

            dataFrame.AddColumn(Operacao.NomeColunaSaida, resultado);

            return dataFrame;
        }
    }
}
