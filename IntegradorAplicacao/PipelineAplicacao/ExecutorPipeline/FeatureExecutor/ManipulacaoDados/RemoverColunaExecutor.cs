using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class RemoverColunaExecutor : FeatureExecutorBase<RemoverColuna>
    {
        public RemoverColunaExecutor(RemoverColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasParaRemover = TransformaStringColunasEmListaColunas(Operacao.col);
            var novoDataFrame = new DataFrame();

            foreach (var coluna in dataFrame.Colunas)
            {
                if (!colunasParaRemover.Contains(coluna.Nome))
                {
                    // Tipo da coluna original
                    var tipoElemento = coluna.TipoDado;

                    // Cria List<tipoElemento> dinamicamente
                    var tipoLista = typeof(List<>).MakeGenericType(tipoElemento);
                    var lista = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;

                    // Preenche a lista com os valores existentes da coluna
                    for (int i = 0; i < coluna.Quantidade; i++)
                    {
                        var valor = coluna.PegarValor(i);
                        lista.Add(valor);
                    }

                    // Chama AdicionarColuna<T> dinamicamente
                    var metodoAdicionar = typeof(DataFrame)
                        .GetMethod("AdicionarColuna")!
                        .MakeGenericMethod(tipoElemento);

                    metodoAdicionar.Invoke(novoDataFrame, new object[] { coluna.Nome, lista });
                }
            }

            return novoDataFrame;
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            var texto = colunas.Trim('[', ']').Split(',');
            List<string> colunasParaRemover = new();

            foreach (var coluna in texto)
            {
                colunasParaRemover.Add(coluna.Trim().Trim('"'));
            }

            return colunasParaRemover;
        }
    }
}