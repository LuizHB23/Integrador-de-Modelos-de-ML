using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.JanelasTemporais;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.JanelasTemporais
{
    public class GroupWindowExecutor : FeatureExecutorBase<GroupWindow>
    {
        public GroupWindowExecutor(GroupWindow operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col);
            var novaColunaPrefixo = Operacao.exit ?? "GroupWindow";

            if (colunasChave == null || colunasChave.Count == 0)
                throw new Exception("É necessário informar pelo menos uma coluna-chave para o groupwindow.");

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

            // 🔹 Identificar colunas que sofrerão diff (todas exceto chaves)
            var colunasNaoChave = dataFrame.ColunaIndex.Keys
                                        .Where(c => !colunasChave.Contains(c))
                                        .ToList();

            // 🔹 Preparar resultados por coluna
            var resultados = new Dictionary<string, List<object?>>();
            foreach (var col in colunasNaoChave)
                resultados[col] = new List<object?>(new object?[dataFrame.QuantidadeLinhas]);

            // 🔹 Processar cada grupo
            foreach (var grupo in grupos.Values)
            {
                for (int j = 0; j < grupo.Count; j++)
                {
                    if (j == 0)
                    {
                        // primeira linha do grupo -> null em todas as colunas não-chave
                        foreach (var col in colunasNaoChave)
                            resultados[col][grupo[j]] = null;
                        continue;
                    }

                    foreach (var col in colunasNaoChave)
                    {
                        var atual = dataFrame.PegarColunaBase(col).PegarValor(grupo[j]);
                        var anterior = dataFrame.PegarColunaBase(col).PegarValor(grupo[j - 1]);

                        if (atual is DateTime dtAtual && anterior is DateTime dtAnterior)
                            resultados[col][grupo[j]] = Convert.ToSingle((dtAtual - dtAnterior).TotalDays);
                        else if (atual is Single sAtual && anterior is Single sAnterior)
                            resultados[col][grupo[j]] = sAtual - sAnterior;
                        else
                            resultados[col][grupo[j]] = null;
                    }
                }
            }

            // 🔹 Adicionar cada coluna diff ao DataFrame como Single?
            var tipoFinal = typeof(Single?);
            foreach (var kvp in resultados)
            {
                var listaTipada = (System.Collections.IList)Activator.CreateInstance(
                    typeof(List<>).MakeGenericType(tipoFinal))!;

                foreach (var v in kvp.Value)
                    listaTipada.Add(v == null ? null : Convert.ToSingle(v));

                var metodoAdicionar = typeof(DataFrame)
                    .GetMethod("AdicionarColuna")!
                    .MakeGenericMethod(tipoFinal);

                // nome da coluna diff = original + "_diff"
                metodoAdicionar.Invoke(dataFrame, new object[] { kvp.Key + "_diff", listaTipada });
            }

            return dataFrame;
        }

        private List<string> TransformaStringColunasEmListaColunas(string colunas)
        {
            var texto = colunas.Trim('[', ']').Split(',');
            var colunasParaRemover = new List<string>();
            foreach (var coluna in texto)
                colunasParaRemover.Add(coluna.Trim().Trim('"'));
            return colunasParaRemover;
        }
    }
}