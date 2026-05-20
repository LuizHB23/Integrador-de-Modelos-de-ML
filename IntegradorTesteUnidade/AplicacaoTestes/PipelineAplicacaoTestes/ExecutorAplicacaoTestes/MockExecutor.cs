using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.Attributes;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorAplicacaoTestes
{
    public class MockExecutor : IExecutorBase
    {
        private readonly MockOperacao _op;
        public MockExecutor(MockOperacao op) => _op = op;
        public object Executar(DataFrame df)
        {
            df.NomeContexto = "ProcessadoPeloMock";
            return df;
        }
    }

    [FeatureName("SomaOperacao")]
    public class MockOperacao
    {
        public string Texto { get; set; } = "";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
