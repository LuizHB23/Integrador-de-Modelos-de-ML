using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            var hashesColunas = CriarHashSetsPorColuna(dataFrame);


            foreach (var colOriginal in dataFrame.Colunas)
            {
                var tipo = colOriginal.TipoDado;

                // Use reflection para chamar PegarColuna<T> de forma genérica
                var metodoPegar = typeof(DataFrame).GetMethod("PegarColuna")!
                    .MakeGenericMethod(tipo);

                var colunaExistente = metodoPegar.Invoke(novoDataFrame, new object[] { colOriginal.Nome });

                if (colunaExistente == null)
                {
                    // Cria uma lista vazia do tipo correto
                    var tipoLista = typeof(List<>).MakeGenericType(tipo);
                    var listaVazia = Activator.CreateInstance(tipoLista);

                    // Adiciona a coluna usando o método oficial AdicionarColuna<T>
                    var metodoAdicionar = typeof(DataFrame).GetMethod("AdicionarColuna")!
                        .MakeGenericMethod(tipo);

                    metodoAdicionar.Invoke(novoDataFrame, new object[] { colOriginal.Nome, listaVazia });
                }
            }

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                List<dynamic?> valores = new();
                int contar = 0;

                for (int j = 0; j < quantidadeColunas; j++)
                {
                    dynamic? valor = dataFrame.Colunas[j].PegarValor(i);

                    if (i == 0)
                    {
                        // Primeira linha: adiciona diretamente
                        hashesColunas[j].Add(valor);
                        novoDataFrame.Colunas[j].AdicionaValor(valor);
                    }
                    else
                    {
                        valores.Add(valor);

                        if (hashesColunas[j].Contains(valor))
                        {
                            contar++;
                        }
                    }
                }

                // Só adiciona a linha se não for duplicada e i > 0
                if (i != 0 && contar != quantidadeColunas)
                {
                    for (int k = 0; k < quantidadeColunas; k++)
                    {
                        dynamic? valor = valores[k];  // agora seguro, valores foi preenchido
                        hashesColunas[k].Add(valor);
                        novoDataFrame.Colunas[k].AdicionaValor(valor);
                    }
                }
            }

            return novoDataFrame;
        }

        public List<dynamic> CriarHashSetsPorColuna(DataFrame dataFrame)
        {
            var hashesColunas = new List<dynamic>();

            foreach (var coluna in dataFrame.Colunas)
            {
                // Pega o tipo da coluna
                var tipo = coluna.TipoDado;

                // Cria o HashSet<T?> dinamicamente usando reflection
                var tipoHashSet = typeof(HashSet<>).MakeGenericType(tipo);
                var hashSet = Activator.CreateInstance(tipoHashSet)!;

                hashesColunas.Add(hashSet);
            }

            return hashesColunas;
        }
    }
}
