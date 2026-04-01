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

            // Adiciona todas as colunas do DataFrame esquerdo
            foreach (var coluna in dataFrame.Colunas)
            {
                var tipoLista = typeof(List<>).MakeGenericType(coluna.TipoDado);
                var listaVazia = Activator.CreateInstance(tipoLista);
                novoDataFrame.AdicionarColuna(coluna.Nome, (dynamic)listaVazia);
            }

            //Adiciona colunas do DataFrame direito com cuidado com conflitos
            foreach (var coluna in dataFrameDireito.Colunas)
            {
                if (coluna.Nome == Operacao.on) 
                    continue;

                string nomeDestino = coluna.Nome;

                if (novoDataFrame.Colunas.Exists(c => c.Nome == nomeDestino))
                    nomeDestino = $"{nomeDestino}_{Operacao.right}";

                var tipoLista = typeof(List<>).MakeGenericType(coluna.TipoDado);
                var listaVazia = Activator.CreateInstance(tipoLista);
                novoDataFrame.AdicionarColuna(nomeDestino, (dynamic)listaVazia);
            }

            // Cria lookup do DataFrame direito baseado na Operacao.on
            int idxOnDireito = dataFrameDireito.Colunas.FindIndex(c => c.Nome == Operacao.on);
            if (idxOnDireito == -1)
                throw new InvalidOperationException($"Coluna '{Operacao.on}' não encontrada no DataFrame direito.");

            var lookupDireito = new Dictionary<object?, List<object?>>();
            for (int i = 0; i < dataFrameDireito.QuantidadeLinhas; i++)
            {
                var chave = dataFrameDireito.Colunas[idxOnDireito].PegarValor(i);
                var linha = new List<object?>();
                foreach (var col in dataFrameDireito.Colunas)
                    linha.Add(col.PegarValor(i));
                lookupDireito[chave] = linha;
            }

            //Preenche o novo DataFrame
            int idxOnEsquerdo = dataFrame.Colunas.FindIndex(c => c.Nome == Operacao.on);
            if (idxOnEsquerdo == -1)
                throw new InvalidOperationException($"Coluna '{Operacao.on}' não encontrada no DataFrame esquerdo.");

            for (int i = 0; i < dataFrame.QuantidadeLinhas; i++)
            {
                var chave = dataFrame.Colunas[idxOnEsquerdo].PegarValor(i);

                // Adiciona valores das colunas do DataFrame esquerdo
                for (int j = 0; j < dataFrame.Colunas.Count; j++)
                    novoDataFrame.Colunas[j].AdicionaValor(dataFrame.Colunas[j].PegarValor(i));

                // Busca a linha correspondente no lookup do DataFrame direito
                List<object?>? linhaDireita = lookupDireito.ContainsKey(chave) ? lookupDireito[chave] : null;

                // Adiciona valores das colunas do DataFrame direito
                for (int j = 0; j < dataFrameDireito.Colunas.Count; j++)
                {
                    var colDireita = dataFrameDireito.Colunas[j];
                    if (colDireita.Nome == Operacao.on) continue;

                    string nomeDestino = colDireita.Nome;
                    if (novoDataFrame.Colunas.Exists(c => c.Nome == nomeDestino))
                        nomeDestino = $"{nomeDestino}_{Operacao.right}";

                    int idxDestino = novoDataFrame.Colunas.FindIndex(c => c.Nome == nomeDestino);

                    object? valor = linhaDireita != null ? linhaDireita[j] : null;
                    novoDataFrame.Colunas[idxDestino].AdicionaValor(valor);
                }
            }

            return novoDataFrame;
        }
    }
}
