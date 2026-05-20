using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class FillNaExecutor : FeatureExecutorBase<FillNa>
    {
        public FillNaExecutor(FillNa operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (!dataFrame.ColunaIndex.TryGetValue(Operacao.col, out int index))
                throw new Exception($"Coluna '{Operacao.col}' não encontrada");

            object? valorOperacao = Operacao.Contexto!.ContainsKey(Operacao.value)
                ? Operacao.Contexto[Operacao.value]
                : Operacao.value;

            var coluna = dataFrame.Colunas[index];

            if (coluna is Coluna<Single?> colunaFloat)
            {
                float valor = Convert.ToSingle(valorOperacao);
                var span = colunaFloat.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].HasValue)
                        span[i] = valor;
                }
            }
            else if (coluna is Coluna<Boolean?> colunaBool)
            {
                bool valor = Convert.ToBoolean(valorOperacao);
                var span = colunaBool.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].HasValue)
                        span[i] = valor;
                }
            }
            else if (coluna is Coluna<DateTime?> colunaDate)
            {
                DateTime valor = Convert.ToDateTime(valorOperacao);
                var span = colunaDate.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].HasValue)
                        span[i] = valor;
                }
            }
            else if (coluna is Coluna<string?> colunaString)
            {
                string valor = (string)valorOperacao!;
                var span = colunaString.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == null)
                        span[i] = valor;
                }
            }
            else
            {
                throw new Exception($"Tipo não suportado: {coluna.GetType()}");
            }

            return dataFrame;
        }
    }
}