using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class CopiarExecutor : FeatureExecutorBase<Copiar>
    {
        public CopiarExecutor(Copiar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var novoDataFrame = new DataFrame();

            foreach (var coluna in dataFrame.Colunas)
            {
                var nome = coluna.Nome;
                var tipo = coluna.TipoDado;

                if (coluna is Coluna<float?> cf)
                {
                    var origem = cf.PegarColunaSpan();
                    var lista = new List<float?>(origem.Length);

                    for (int i = 0; i < origem.Length; i++)
                        lista.Add(origem[i]);

                    novoDataFrame.AdicionarColuna<float?>(nome, lista);
                }
                else if (coluna is Coluna<string?> cs)
                {
                    var origem = cs.PegarColunaSpan();
                    var lista = new List<string?>(origem.Length);

                    for (int i = 0; i < origem.Length; i++)
                        lista.Add(origem[i]);

                    novoDataFrame.AdicionarColuna<string?>(nome, lista);
                }
                else if (coluna is Coluna<bool?> cb)
                {
                    var origem = cb.PegarColunaSpan();
                    var lista = new List<bool?>(origem.Length);

                    for (int i = 0; i < origem.Length; i++)
                        lista.Add(origem[i]);

                    novoDataFrame.AdicionarColuna<bool?>(nome, lista);
                }
                else if (coluna is Coluna<DateTime?> cd)
                {
                    var origem = cd.PegarColunaSpan();
                    var lista = new List<DateTime?>(origem.Length);

                    for (int i = 0; i < origem.Length; i++)
                        lista.Add(origem[i]);

                    novoDataFrame.AdicionarColuna<DateTime?>(nome, lista);
                }
                else
                {
                    // fallback genérico (evita reflection)
                    var listType = typeof(List<>).MakeGenericType(tipo);
                    var lista = (IList)Activator.CreateInstance(listType)!;

                    for (int i = 0; i < coluna.Quantidade; i++)
                        lista.Add(coluna.PegarValor(i));

                    // aqui ainda precisa reflection se DataFrame não tiver overload não genérico
                    var metodo = typeof(DataFrame)
                        .GetMethod("AdicionarColuna")!
                        .MakeGenericMethod(tipo);

                    metodo.Invoke(novoDataFrame, new object[] { nome, lista });
                }
            }

            return novoDataFrame;
        }
    }
}
