using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class RemoverColunaExecutor : FeatureExecutorBase<RemoverColuna>
    {
        public RemoverColunaExecutor(RemoverColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var remover = new HashSet<string>(
                TransformaStringColunasEmListaColunas(Operacao.col)
            );

            var novoDataFrame = new DataFrame();
            int n = dataFrame.QuantidadeLinhas;

            foreach (var coluna in dataFrame.Colunas)
            {
                if (remover.Contains(coluna.Nome))
                    continue;

                var nome = coluna.Nome;

                // 🔥 caminho otimizado por tipo
                if (coluna is Coluna<float?> cf)
                {
                    var span = cf.PegarColunaSpan();
                    var arr = new float?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    novoDataFrame.AdicionarColuna<float?>(nome, arr.ToList());
                    continue;
                }

                if (coluna is Coluna<bool?> cb)
                {
                    var span = cb.PegarColunaSpan();
                    var arr = new bool?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    novoDataFrame.AdicionarColuna<bool?>(nome, arr.ToList());
                    continue;
                }

                if (coluna is Coluna<DateTime?> cd)
                {
                    var span = cd.PegarColunaSpan();
                    var arr = new DateTime?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    novoDataFrame.AdicionarColuna<DateTime?>(nome, arr.ToList());
                    continue;
                }

                if (coluna is Coluna<string?> cs)
                {
                    var span = cs.PegarColunaSpan();
                    var arr = new string?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    novoDataFrame.AdicionarColuna<string?>(nome, arr.ToList());
                    continue;
                }

                // 🔥 fallback genérico (sem reflection)
                var objArr = new object?[n];
                for (int i = 0; i < n; i++)
                    objArr[i] = coluna.PegarValor(i);

                novoDataFrame.AdicionarColuna<object?>(nome, objArr.ToList());
            }

            return novoDataFrame;
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            var texto = colunas.Trim('[', ']').Split(',');
            var list = new List<string>(texto.Length);

            foreach (var c in texto)
                list.Add(c.Trim().Trim('"'));

            return list;
        }
    }
}