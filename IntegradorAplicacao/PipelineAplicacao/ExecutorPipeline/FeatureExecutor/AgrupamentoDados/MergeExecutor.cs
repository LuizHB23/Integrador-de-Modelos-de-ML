using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System.Data;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class MergeExecutor : FeatureExecutorBase<Merge>
    {
        public MergeExecutor(Merge operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunas = TransformaStringColunasEmListaColunas(Operacao.on); // múltiplas colunas
            DataFrame dataFrameDireito = (DataFrame)Operacao.Contexto[Operacao.right]!;

            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));
            if (dataFrameDireito == null) throw new ArgumentNullException(nameof(dataFrameDireito));
            if (colunas == null || colunas.Count == 0) throw new ArgumentNullException(nameof(Operacao.on));

            var novoDataFrame = new DataFrame();
            var mapaIndices = new Dictionary<string, int>();
            var mapaNomesDireito = new Dictionary<string, string>();

            // 🔹 Colunas do esquerdo
            foreach (var coluna in dataFrame.Colunas)
            {
                var tipoLista = typeof(List<>).MakeGenericType(coluna.TipoDado);
                var listaVazia = Activator.CreateInstance(tipoLista);
                novoDataFrame.AdicionarColuna(coluna.Nome, (dynamic)listaVazia);
                mapaIndices[coluna.Nome] = novoDataFrame.Colunas.Count - 1;
            }

            // 🔹 Colunas do direito
            foreach (var coluna in dataFrameDireito.Colunas)
            {
                if (colunas.Contains(coluna.Nome))
                    continue;

                string nomeDestino = coluna.Nome;
                if (mapaIndices.ContainsKey(nomeDestino))
                    nomeDestino = $"{nomeDestino}_{Operacao.right}";

                mapaNomesDireito[coluna.Nome] = nomeDestino;

                var tipoLista = typeof(List<>).MakeGenericType(coluna.TipoDado);
                var listaVazia = Activator.CreateInstance(tipoLista);
                novoDataFrame.AdicionarColuna(nomeDestino, (dynamic)listaVazia);
                mapaIndices[nomeDestino] = novoDataFrame.Colunas.Count - 1;
            }

            // 🔹 Lookup múltiplas colunas (direito)
            var lookupDireito = new Dictionary<string, int>();
            for (int i = 0; i < dataFrameDireito.QuantidadeLinhas; i++)
            {
                var chave = string.Join("|", colunas.Select(c => dataFrameDireito.PegarColunaBase(c).PegarValor(i)?.ToString() ?? "NULL"));
                lookupDireito[chave] = i;
            }

            // 🔹 Loop principal
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var chave = string.Join("|", colunas.Select(c => dataFrame.PegarColunaBase(c).PegarValor(i)?.ToString() ?? "NULL"));

                int? linhaDireita = lookupDireito.ContainsKey(chave) ? lookupDireito[chave] : (int?)null;

                // esquerdo
                foreach (var col in dataFrame.Colunas)
                {
                    int idxDestino = mapaIndices[col.Nome];
                    novoDataFrame.Colunas[idxDestino].AdicionaValor(col.PegarValor(i));
                }

                // direito
                foreach (var colDireita in dataFrameDireito.Colunas)
                {
                    if (colunas.Contains(colDireita.Nome)) continue;

                    string nomeDestino = mapaNomesDireito[colDireita.Nome];
                    int idxDestino = mapaIndices[nomeDestino];

                    object? valor = linhaDireita != null ? colDireita.PegarValor(linhaDireita.Value) : null;
                    novoDataFrame.Colunas[idxDestino].AdicionaValor(valor);
                }
            }

            return novoDataFrame;

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
