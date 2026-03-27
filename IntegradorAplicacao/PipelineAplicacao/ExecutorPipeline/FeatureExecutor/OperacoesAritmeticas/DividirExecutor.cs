using IntegradorAplicacao.PipelineAplicacao.Interfaces;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesAritmeticas;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesAritmeticas
{
    internal class DividirExecutor : IFeatureExecutor<Dividir>
    {
        public Dividir Operacao { get; }

        public DividirExecutor(Dividir operacao)
        {
            Operacao = operacao;
        }

        public DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;

            var resultado = new float[n];

            for (int i = 0; i < n; i++)
            {
                resultado[i] = Operacao.ColunaEsquerda.Dados[i] / Operacao.ColunaDireita.Dados[i];
            }

            dataFrame.AddColumn(Operacao.NomeColunaSaida, resultado);

            return dataFrame;
        }
    }
}
