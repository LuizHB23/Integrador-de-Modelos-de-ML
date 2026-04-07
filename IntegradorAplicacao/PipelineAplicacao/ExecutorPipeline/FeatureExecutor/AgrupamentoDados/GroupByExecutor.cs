using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor : FeatureExecutorBase<GroupBy>
    {
        public GroupByExecutor(GroupBy operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col);
            var agregacao = Operacao.agg?.ToLower();

            if (colunasChave == null || colunasChave.Count == 0)
                throw new Exception("É necessário informar pelo menos uma coluna-chave para o groupby.");

            bool ehDiff = agregacao == "diff";

            // 🔥 CASO ESPECIAL: diff como janela (igual GroupWindow)
            if (ehDiff)
            {
                return ExecutarDiffComoJanela(dataFrame, colunasChave);
            }

            // 🔹 Criar grupos
            var grupos = new Dictionary<string, List<int>>();

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var chave = string.Join("|", colunasChave.Select(c =>
                {
                    var valor = dataFrame.PegarColunaBase(c)?.PegarValor(i);
                    return valor?.ToString() ?? "NULL";
                }));

                if (!grupos.TryGetValue(chave, out var lista))
                {
                    lista = new List<int>();
                    grupos[chave] = lista;
                }

                lista.Add(i);
            }

            // 🔹 Preparar novo DataFrame
            var novoDataFrame = new DataFrame();
            var colunasResultado = new Dictionary<string, List<object?>>();

            foreach (var col in dataFrame.Colunas)
            {
                colunasResultado[col.Nome] = new List<object?>();
            }

            // 🔹 Função de agregação
            Func<ColunaBase, List<int>, object?> functionAgregacao = agregacao switch
            {
                "sum" => AgregacaoSoma,
                "count" => AgregacaoCount,
                "mean" => AgregacaoMedia,
                "std" => AgregacaoDesvioPadrao,
                "min" => AgregacaoMinimo,
                "max" => AgregacaoMaximo,
                _ => throw new Exception($"Operação {agregacao} não suportada")
            };

            // 🔹 Processar cada grupo
            foreach (var grupo in grupos)
            {
                var indices = grupo.Value;

                foreach (var col in dataFrame.Colunas)
                {
                    if (colunasChave.Contains(col.Nome))
                    {
                        colunasResultado[col.Nome].Add(col.PegarValor(indices[0]));
                    }
                    else
                    {
                        var resultado = functionAgregacao(col, indices);
                        colunasResultado[col.Nome].Add(resultado);
                    }
                }
            }

            var ordemFinal = new List<string>();
            ordemFinal.AddRange(colunasChave);
            ordemFinal.AddRange(
                dataFrame.Colunas
                    .Select(c => c.Nome)
                    .Where(nome => !colunasChave.Contains(nome))
            );

            // 🔹 Adicionar colunas
            foreach (var nomeColuna in ordemFinal)
            {
                var colOriginal = dataFrame.Colunas.First(c => c.Nome == nomeColuna);

                AdicionarColunaTipadaDynamic(
                    novoDataFrame,
                    nomeColuna,
                    colunasResultado[nomeColuna],
                    colOriginal.TipoDado,
                    false
                );
            }

            return novoDataFrame;
        }

        // 🔥 NOVO: diff estilo janela
        private DataFrame ExecutarDiffComoJanela(DataFrame dataFrame, List<string> colunasChave)
        {
            var grupos = new Dictionary<string, List<int>>();

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var chave = string.Join("|", colunasChave.Select(c =>
                {
                    var valor = dataFrame.PegarColunaBase(c)?.PegarValor(i);
                    return valor?.ToString() ?? "NULL";
                }));

                if (!grupos.TryGetValue(chave, out var lista))
                {
                    lista = new List<int>();
                    grupos[chave] = lista;
                }

                lista.Add(i);
            }

            var novoDataFrame = new DataFrame();

            foreach (var col in dataFrame.Colunas)
            {
                var valores = new List<object?>();

                if (colunasChave.Contains(col.Nome))
                {
                    for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
                        valores.Add(col.PegarValor(i));
                }
                else
                {
                    var resultados = new object?[dataFrame.QuantidadeLinhas];

                    foreach (var grupo in grupos.Values)
                    {
                        for (int j = 0; j < grupo.Count; j++)
                        {
                            if (j == 0)
                            {
                                resultados[grupo[j]] = null;
                                continue;
                            }

                            var atual = col.PegarValor(grupo[j]);
                            var anterior = col.PegarValor(grupo[j - 1]);

                            if (atual is Single sAtual && anterior is Single sAnterior)
                            {
                                resultados[grupo[j]] = sAtual - sAnterior;
                            }
                            else
                            {
                                resultados[grupo[j]] = null;
                            }
                        }
                    }

                    valores.AddRange(resultados);
                }

                AdicionarColunaTipadaDynamic(
                    novoDataFrame,
                    col.Nome,
                    valores,
                    typeof(Single),
                    true
                );
            }

            return novoDataFrame;
        }

        private object? AgregacaoSoma(ColunaBase coluna, List<int> indices)
        {
            Single? soma = 0;

            foreach (var i in indices)
            {
                var valor = (Single?)coluna.PegarValor(i);
                if (valor != null)
                    soma += valor;
            }

            return soma;
        }

        private object? AgregacaoCount(ColunaBase coluna, List<int> indices)
        {
            int count = 0;

            foreach (var i in indices)
            {
                if (coluna.PegarValor(i) != null)
                    count++;
            }

            return count;
        }

        private object? AgregacaoMedia(ColunaBase coluna, List<int> indices)
        {
            Single total = 0;
            int contar = 0;

            foreach (var i in indices)
            {
                var valor = (Single?)coluna.PegarValor(i);
                if (valor != null)
                {
                    total += valor.Value;
                    contar++;
                }
            }

            return contar == 0 ? null : total / contar;
        }

        private object? AgregacaoDesvioPadrao(ColunaBase coluna, List<int> indices)
        {
            var valores = new List<double>();

            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor is Single s)
                    valores.Add(s);
                else if (valor is double d)
                    valores.Add(d);
                else if (valor != null)
                    valores.Add(Convert.ToDouble(valor));
            }

            if (valores.Count <= 1)
                return null; // Pandas retorna NaN se só tiver 1 valor

            var media = valores.Sum() / valores.Count;

            // Variância amostral (divide por N-1)
            var variancia = valores.Sum(v => (v - media) * (v - media)) / (valores.Count - 1);

            return (Single)Math.Sqrt(variancia);
        }

        private object? AgregacaoMinimo(ColunaBase coluna, List<int> indices)
        {
            Single? min = null;

            foreach (var i in indices)
            {
                var valor = (Single?)coluna.PegarValor(i);
                if (valor == null) continue;

                if (min == null || valor < min)
                    min = valor;
            }

            return min;
        }

        private object? AgregacaoMaximo(ColunaBase coluna, List<int> indices)
        {
            Single? max = null;

            foreach (var i in indices)
            {
                var valor = (Single?)coluna.PegarValor(i);
                if (valor == null) continue;

                if (max == null || valor > max)
                    max = valor;
            }

            return max;
        }

        private void AdicionarColunaTipadaDynamic(
            DataFrame df,
            string nomeColuna,
            List<object?> valores,
            Type tipoOriginal,
            bool ehDiff = false)
        {
            Type tipoFinal = ehDiff ? typeof(Single?) : tipoOriginal;

            var listaTipada = (System.Collections.IList)
                Activator.CreateInstance(typeof(List<>).MakeGenericType(tipoFinal))!;

            foreach (var v in valores)
            {
                if (v == null)
                {
                    listaTipada.Add(null);
                }
                else
                {
                    object valorConvertido;

                    if (ehDiff)
                    {
                        valorConvertido = Convert.ToSingle(v);
                    }
                    else
                    {
                        valorConvertido = Convert.ChangeType(
                            v,
                            Nullable.GetUnderlyingType(tipoFinal) ?? tipoFinal
                        );
                    }

                    listaTipada.Add(valorConvertido);
                }
            }

            var metodoAdicionar = typeof(DataFrame)
                .GetMethod("AdicionarColuna")!
                .MakeGenericMethod(tipoFinal);

            metodoAdicionar.Invoke(df, new object[] { nomeColuna, listaTipada });
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            var texto = colunas.Trim('[', ']').Split(',');
            var lista = new List<string>();

            foreach (var coluna in texto)
            {
                lista.Add(coluna.Trim().Trim('"'));
            }

            return lista;
        }
    }
}