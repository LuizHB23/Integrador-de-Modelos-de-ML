using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class RemoverNuloExecutor : FeatureExecutorBase<RemoverNulo>
    {
        public RemoverNuloExecutor(RemoverNulo operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (dataFrame == null || dataFrame.QuantidadeLinhas == 0)
                return dataFrame;

            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            int quantidadeColunas = dataFrame.Colunas.Count;

            var novoDataFrame = new DataFrame();

            // Cria colunas vazias do mesmo tipo que o DataFrame original
            foreach (var col in dataFrame.Colunas)
            {
                var tipoLista = typeof(List<>).MakeGenericType(col.TipoDado);
                var listaVazia = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;
                novoDataFrame.AdicionarColuna(col.Nome, listaVazia as dynamic);
            }

            // Percorre as linhas
            for (int i = 0; i < quantidadeLinhas; i++)
            {
                bool linhaValida = true;

                for (int j = 0; j < quantidadeColunas; j++)
                {
                    var valor = dataFrame.Colunas[j].PegarValor(i);
                    if (valor is string)
                    {
                        var texto = (string)valor;
                        if (texto.Trim() == "")
                        {
                            linhaValida = false;
                            break;
                        }
                    }
                    else
                    {
                        if (valor == null)
                        {
                            linhaValida = false;
                            break;
                        }
                    }
                }

                if (linhaValida)
                {
                    for (int j = 0; j < quantidadeColunas; j++)
                    {
                        var valor = dataFrame.Colunas[j].PegarValor(i);
                        novoDataFrame.Colunas[j].AdicionaValor(valor);
                    }
                }
            }

            return novoDataFrame;
        }
    }
}
