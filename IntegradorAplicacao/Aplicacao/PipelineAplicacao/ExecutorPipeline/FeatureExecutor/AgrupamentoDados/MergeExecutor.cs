using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using IntegradorDominio.Models.DataFrameModel;
using System.Data;
using System.Diagnostics;


namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class MergeExecutor : FeatureExecutorBase<Merge>
    {
        public MergeExecutor(Merge operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunas = TransformaStringColunasEmListaColunas(Operacao.on);
            var dfDir = (DataFrame)Operacao.Contexto[Operacao.right]!;

            if (colunas.Count == 0)
                throw new Exception("Colunas de merge inválidas");

            var colunasSet = new HashSet<string>(colunas);

            // 🔥 cache colunas chave (evita lookup por nome)
            var colsEsq = colunas.Select(c => dataFrame.PegarColunaBase(c)).ToArray();
            var colsDir = colunas.Select(c => dfDir.PegarColunaBase(c)).ToArray();

            var novo = new DataFrame();
            var mapaIndices = new Dictionary<string, int>();
            var mapaDireito = new Dictionary<string, string>();

            // =========================
            // COLUNAS ESQUERDO
            // =========================
            foreach (var col in dataFrame.Colunas)
            {
                var list = (System.Collections.IList)
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(col.TipoDado))!;

                novo.AdicionarColuna(col.Nome, (dynamic)list);
                mapaIndices[col.Nome] = novo.Colunas.Count - 1;
            }

            // =========================
            // COLUNAS DIREITO
            // =========================
            foreach (var col in dfDir.Colunas)
            {
                if (colunasSet.Contains(col.Nome)) continue;

                string nome = col.Nome;

                if (mapaIndices.ContainsKey(nome))
                    nome = $"{nome}_{Operacao.right}";

                mapaDireito[col.Nome] = nome;

                var list = (System.Collections.IList)
                    Activator.CreateInstance(typeof(List<>).MakeGenericType(col.TipoDado))!;

                novo.AdicionarColuna(nome, (dynamic)list);
                mapaIndices[nome] = novo.Colunas.Count - 1;
            }

            // =========================
            // LOOKUP DIREITO (OTIMIZADO)
            // =========================
            var lookup = new Dictionary<StructKey, int>();

            for (int i = 0; i < dfDir.QuantidadeLinhas; i++)
            {
                var key = BuildKey(colsDir, i);

                if (!lookup.TryGetValue(key, out int existente))
                {
                    lookup[key] = i;
                }
                else
                {
                    // prioriza menos null
                    if (TemMenosNull(colsDir, i, existente))
                        lookup[key] = i;
                }
            }

            // =========================
            // LOOP PRINCIPAL
            // =========================
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var key = BuildKey(colsEsq, i);

                lookup.TryGetValue(key, out int linhaDir);

                // ESQUERDO
                foreach (var col in dataFrame.Colunas)
                {
                    novo.Colunas[mapaIndices[col.Nome]]
                        .AdicionaValor(col.PegarValor(i));
                }

                // DIREITO
                foreach (var col in dfDir.Colunas)
                {
                    if (colunasSet.Contains(col.Nome)) continue;

                    var nome = mapaDireito[col.Nome];

                    object? v = linhaDir != 0 || lookup.ContainsKey(key)
                        ? col.PegarValor(linhaDir)
                        : null;

                    novo.Colunas[mapaIndices[nome]]
                        .AdicionaValor(v);
                }
            }

            return novo;
        }

        private StructKey BuildKey(ColunaBase[] cols, int row)
        {
            var values = new object?[cols.Length];

            for (int i = 0; i < cols.Length; i++)
                values[i] = cols[i].PegarValor(row);

            return new StructKey(values);
        }

        private bool TemMenosNull(ColunaBase[] cols, int atual, int existente)
        {
            int nullAtual = 0;
            int nullExistente = 0;

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].PegarValor(atual) == null) nullAtual++;
                if (cols[i].PegarValor(existente) == null) nullExistente++;
            }

            return nullAtual < nullExistente;
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

        private struct StructKey : IEquatable<StructKey>
        {
            private readonly object?[] values;

            public StructKey(object?[] values)
            {
                this.values = values;
            }

            public bool Equals(StructKey other)
            {
                if (values.Length != other.values.Length) return false;

                for (int i = 0; i < values.Length; i++)
                {
                    if (!Equals(values[i], other.values[i]))
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();

                foreach (var v in values)
                    hash.Add(v);

                return hash.ToHashCode();
            }
        }
    }
}
