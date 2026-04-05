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
            DataFrame dataFrameDireito = (DataFrame)Operacao.Contexto[Operacao.right]!;

            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));
            if (dataFrameDireito == null) throw new ArgumentNullException(nameof(dataFrameDireito));
            if (string.IsNullOrEmpty(Operacao.on)) throw new ArgumentNullException(nameof(Operacao.on));

            var novoDataFrame = new DataFrame();

            // 🔥 Mapa: nome coluna -> índice no novo DF
            var mapaIndices = new Dictionary<string, int>();

            // 🔹 Colunas do esquerdo
            foreach (var coluna in dataFrame.Colunas)
            {
                var tipoLista = typeof(List<>).MakeGenericType(coluna.TipoDado);
                var listaVazia = Activator.CreateInstance(tipoLista);

                novoDataFrame.AdicionarColuna(coluna.Nome, (dynamic)listaVazia);
                mapaIndices[coluna.Nome] = novoDataFrame.Colunas.Count - 1;
            }

            // 🔥 Mapa fixo de nomes do direito
            var mapaNomesDireito = new Dictionary<string, string>();

            // 🔹 Colunas do direito
            foreach (var coluna in dataFrameDireito.Colunas)
            {
                if (coluna.Nome == Operacao.on)
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

            // 🔥 Lookup (índice da linha)
            int idxOnDireito = dataFrameDireito.Colunas.FindIndex(c => c.Nome == Operacao.on);
            if (idxOnDireito == -1)
                throw new InvalidOperationException($"Coluna '{Operacao.on}' não encontrada no DataFrame direito.");

            var lookupDireito = new Dictionary<object?, int>();

            for (int i = 0; i < dataFrameDireito.QuantidadeLinhas; i++)
            {
                var chave = dataFrameDireito.Colunas[idxOnDireito].PegarValor(i);
                lookupDireito[chave] = i;
            }

            // 🔥 Índice chave esquerdo
            int idxOnEsquerdo = dataFrame.Colunas.FindIndex(c => c.Nome == Operacao.on);
            if (idxOnEsquerdo == -1)
                throw new InvalidOperationException($"Coluna '{Operacao.on}' não encontrada no DataFrame esquerdo.");

            // 🔥 Loop principal
            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var chave = dataFrame.Colunas[idxOnEsquerdo].PegarValor(i);

                int? linhaDireita = lookupDireito.ContainsKey(chave)
                    ? lookupDireito[chave]
                    : (int?)null;

                // 🔹 esquerdo
                foreach (var col in dataFrame.Colunas)
                {
                    int idxDestino = mapaIndices[col.Nome];
                    novoDataFrame.Colunas[idxDestino].AdicionaValor(col.PegarValor(i));
                }

                // 🔹 direito
                foreach (var colDireita in dataFrameDireito.Colunas)
                {
                    if (colDireita.Nome == Operacao.on)
                        continue;

                    string nomeDestino = mapaNomesDireito[colDireita.Nome];
                    int idxDestino = mapaIndices[nomeDestino];

                    object? valor = linhaDireita != null
                        ? colDireita.PegarValor(linhaDireita.Value)
                        : null;

                    novoDataFrame.Colunas[idxDestino].AdicionaValor(valor);
                }
            }

            return novoDataFrame;
        }
    }
}
