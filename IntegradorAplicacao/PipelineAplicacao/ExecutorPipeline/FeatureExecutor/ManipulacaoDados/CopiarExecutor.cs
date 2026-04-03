using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class CopiarExecutor : FeatureExecutorBase<Copiar>
    {
        public CopiarExecutor(Copiar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var novoDataFrame = new DataFrame();

            foreach (var coluna in dataFrame.Colunas)
            {
                Type tipo = coluna.TipoDado;

                Type listTipo = typeof(List<>).MakeGenericType(tipo);
                var listaNova = (IList)Activator.CreateInstance(listTipo)!;

                for (int i = 0; i < coluna.Quantidade; i++)
                {
                    listaNova.Add(coluna.PegarValor(i));
                }

                Type colunaTipo = typeof(Coluna<>).MakeGenericType(tipo);
                var construtorColuna = colunaTipo.GetConstructor(new Type[] { typeof(string), listTipo })!;
                var novaColuna = construtorColuna.Invoke(new object[] { coluna.Nome, listaNova });

                var metodoAdd = typeof(DataFrame).GetMethod("AdicionarColuna")!.MakeGenericMethod(tipo);
                metodoAdd.Invoke(novoDataFrame, new object[] { coluna.Nome, listaNova });
            }

            return novoDataFrame;
        }
    }
}
