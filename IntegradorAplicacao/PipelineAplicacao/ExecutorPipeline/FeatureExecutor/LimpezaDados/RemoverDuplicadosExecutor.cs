using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class RemoverDuplicadosExecutor : FeatureExecutorBase<RemoverDuplicados>
    {
        public RemoverDuplicadosExecutor(RemoverDuplicados operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (dataFrame == null || dataFrame.QuantidadeLinhas == 0)
                return dataFrame;

            var novoDataFrame = new DataFrame();
            int quantidadeColunas = dataFrame.Colunas.Count;
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;

            // Cria colunas no novo DataFrame com o mesmo tipo
            foreach (var colOriginal in dataFrame.Colunas)
            {
                Type tipo = colOriginal.TipoDado;

                // Cria lista vazia do tipo correto
                var tipoLista = typeof(List<>).MakeGenericType(tipo);
                var listaVazia = Activator.CreateInstance(tipoLista);

                // Adiciona a coluna
                var metodoAdicionar = typeof(DataFrame).GetMethod("AdicionarColuna")!.MakeGenericMethod(tipo);
                metodoAdicionar.Invoke(novoDataFrame, new object[] { colOriginal.Nome, listaVazia });
            }

            // HashSet para verificar duplicados linha a linha
            var linhasVistas = new HashSet<string>();

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                // Cria uma "chave" única para a linha inteira
                var chaveLinha = new StringBuilder();
                for (int j = 0; j < quantidadeColunas; j++)
                {
                    var valor = dataFrame.Colunas[j].PegarValor(i);
                    chaveLinha.Append(valor?.ToString() ?? "NULL").Append("|");
                }

                string chaveFinal = chaveLinha.ToString();

                if (!linhasVistas.Contains(chaveFinal))
                {
                    linhasVistas.Add(chaveFinal);

                    // Adiciona os valores no novo DataFrame
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