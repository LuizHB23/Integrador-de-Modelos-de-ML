using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEstatisticas;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEstatisticas
{
    public class ModaExecutor : FeatureExecutorBase<Moda>
    {
        public ModaExecutor(Moda operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            var resultado = new List<Single?>();
            var frequencias = new Dictionary<Single, int>();

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                var valor = coluna.PegarValor(i);
                if (valor is not null)
                {
                    Single v = (Single)valor;
                    if (frequencias.ContainsKey(v))
                        frequencias[v]++;
                    else
                        frequencias[v] = 1;
                }
            }

            Single? moda = null;
            int frequenciaMaxima = 0;

            foreach (var valor in frequencias)
            {
                if (valor.Value > frequenciaMaxima)
                {
                    frequenciaMaxima = valor.Value;
                    moda = valor.Key;
                }
            }

            return moda;
        }
    }
}
