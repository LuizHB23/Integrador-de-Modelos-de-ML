using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;

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

            var coluna = dataFrame.Colunas[index];

            if (coluna is Coluna<Single?> colunaFloat)
            {
                float valor = Convert.ToSingle(Operacao.value);

                for (int i = 0; i < colunaFloat.Dados.Length; i++)
                {
                    if (colunaFloat.Dados[i] is null)
                    {
                        colunaFloat.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<Boolean?> colunaBool)
            {
                bool valor = Convert.ToBoolean(Operacao.value);

                for (int i = 0; i < colunaBool.Dados.Length; i++)
                {
                    if (colunaBool.Dados[i] == null)
                    {
                        colunaBool.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<DateTime?> colunaDate)
            {
                DateTime valor = Convert.ToDateTime(Operacao.value);

                for (int i = 0; i < colunaDate.Dados.Length; i++)
                {
                    if (colunaDate.Dados[i] == null)
                    {
                        colunaDate.Dados[i] = valor;
                    }
                }
            }
            else if (coluna is Coluna<string?> colunaString)
            {
                string valor = Operacao.value;

                for (int i = 0; i < colunaString.Dados.Length; i++)
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
