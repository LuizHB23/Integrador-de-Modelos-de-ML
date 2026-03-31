using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System.Diagnostics;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class FillNaExecutor : FeatureExecutorBase<FillNa>
    {
        public FillNaExecutor(FillNa operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (!dataFrame.ColunaIndex.TryGetValue(Operacao.col, out int index))
            {
                throw new Exception($"Coluna '{Operacao.col}' não encontrada");
            }

            object? valorOperacao;

            if (Operacao.Contexto!.ContainsKey(Operacao.value))
            {
                valorOperacao = Operacao.Contexto[Operacao.value];
            }
            else
            {
                valorOperacao = Operacao.value;
            }

            var coluna = dataFrame.Colunas[index];

            if (coluna is Coluna<Single?> colunaFloat)
            {
                float valor = Convert.ToSingle(valorOperacao);

                for (int i = 0; i < colunaFloat.Dados.Count; i++)
                {
                    if (colunaFloat.Dados[i] is null)
                    {
                        colunaFloat.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<Boolean?> colunaBool)
            {
                bool valor = Convert.ToBoolean(valorOperacao);

                for (int i = 0; i < colunaBool.Dados.Count; i++)
                {
                    if (colunaBool.Dados[i] == null)
                    {
                        colunaBool.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<DateTime?> colunaDate)
            {
                DateTime valor = Convert.ToDateTime(valorOperacao);

                for (int i = 0; i < colunaDate.Dados.Count; i++)
                {
                    if (colunaDate.Dados[i] == null)
                    {
                        colunaDate.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<string?> colunaString)
            {
                string valor = (string)valorOperacao!;

                for (int i = 0; i < colunaString.Dados.Count; i++)
                {
                    if (colunaString.Dados[i] == null)
                    {
                        colunaString.Dados[i] = valor;
                    }
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
