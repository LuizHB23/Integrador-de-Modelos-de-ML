namespace IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class EtapaExecucao
    {
        public string DataFrameDestino { get; set; }
        public string DataFrameOrigem { get; set; }
        public FeatureExecutor Executor { get; set; }

        public EtapaExecucao(string dataFrameDestino, string dataFrameOrigem, FeatureExecutor executor)
        {
            DataFrameDestino = dataFrameDestino;
            DataFrameOrigem = dataFrameOrigem;
            Executor = executor;
        }
    }
}
