using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.AgrupamentoDados
{
    public class GroupByExecutor : FeatureExecutorBase<GroupBy>
    {
        public GroupByExecutor(GroupBy operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var nomeColuna = Operacao.col;
            var agregacao = Operacao.agg?.ToLower();

            var colunaChave = dataFrame.Colunas[dataFrame.ColunaIndex[nomeColuna]];
            int n = dataFrame.QuantidadeLinhas;

            // 🔹 1. Agrupar índices
            var grupos = new Dictionary<object?, List<int>>();

            for (int i = 0; i < n; i++)
            {
                var chave = colunaChave.PegarValor(i);

                if (!grupos.TryGetValue(chave, out var lista))
                {
                    lista = new List<int>();
                    grupos[chave] = lista;
                }

                lista.Add(i);
            }

            // 🔹 2. Novo DataFrame
            var novoDataFrame = new DataFrame();

            var listaChaves = new List<object?>(grupos.Count);

            // prepara colunas (exceto chave)
            var colunasResultado = new Dictionary<string, List<object?>>();
            foreach (var col in dataFrame.Colunas)
            {
                if (col.Nome == nomeColuna) continue;
                colunasResultado[col.Nome] = new List<object?>(grupos.Count);
            }

            Func<ColunaBase, List<int>, object?> functionAgregacao;

            switch(agregacao)
            {
                case "count":
                    functionAgregacao = AgregacaoCount;
                    break;

                case "sum":
                    functionAgregacao = AgregacaoSoma;
                    break;

                case "mean":
                    functionAgregacao = AgregacaoMedia;
                    break;

                case "min":
                    functionAgregacao = AgregacaoMinimo;
                    break;

                case "max":
                    functionAgregacao = AgregacaoMaximo;
                    break;

                case "diff":
                    functionAgregacao = AgregacaoDiff;
                    break;

                default:
                    throw new Exception($"Operação {agregacao} não suportada");
            }

            // 🔹 3. Processar grupos
            foreach (var grupo in grupos)
            {
                listaChaves.Add(grupo.Key);
                var indices = grupo.Value;

                foreach (var coluna in dataFrame.Colunas)
                {
                    if (coluna.Nome == nomeColuna) continue;

                    object? resultado = functionAgregacao(coluna, indices);

                    colunasResultado[coluna.Nome].Add(resultado);
                }
            }

            // 🔹 4. Montar resultado
            var tipoChave = colunaChave.TipoDado;

            var metodoChave = typeof(GroupByExecutor)
                .GetMethod(nameof(AdicionarColunaTipada), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .MakeGenericMethod(tipoChave);

            metodoChave.Invoke(this, new object[]
            {
                novoDataFrame,
                nomeColuna,
                listaChaves
            });

            // colunas agregadas com tipagem correta
            foreach (var colunaOriginal in dataFrame.Colunas)
            {
                if (colunaOriginal.Nome == nomeColuna) continue;

                var tipo = colunaOriginal.TipoDado;

                var metodo = typeof(GroupByExecutor)
                    .GetMethod(nameof(AdicionarColunaTipada), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(tipo);

                metodo.Invoke(this, new object[]
                {
                    novoDataFrame,
                    colunaOriginal.Nome,
                    colunasResultado[colunaOriginal.Nome]
                });
            }

            return novoDataFrame;
        }

        private object? AgregacaoSoma(ColunaBase coluna, List<int> indices)
        {
            Single? soma = 0;
            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor != null)
                    soma += (Single)valor; // assume já tipado corretamente
            }

            return soma;
        }

        private object? AgregacaoCount(ColunaBase coluna, List<int> indices)
        {
            int? count = 0;
            foreach (var i in indices)
            {
                if (coluna.PegarValor(i) != null)
                    count++;
            }
            return count;
        }

        private object? AgregacaoMedia(ColunaBase coluna, List<int> indices)
        {
            Single? total = 0;
            int contar = 0;
            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor != null)
                {
                    total += (Single)valor;
                    contar++;
                }
            }

            return contar == 0 ? null : total / contar;
        }

        private object? AgregacaoMinimo(ColunaBase coluna, List<int> indices)
        {
            object? min = null;
            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor == null) continue;

                if (min == null || ((IComparable)valor).CompareTo(min) < 0)
                    min = valor;
            }
            return min;
        }

        private object? AgregacaoMaximo(ColunaBase coluna, List<int> indices)
        {
            object? max = null;
            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor == null) continue;

                if (max == null || ((IComparable)valor).CompareTo(max) > 0)
                    max = valor;
            }
            return max;
        }

        private object? AgregacaoDiff(ColunaBase coluna, List<int> indices)
        {
            if (indices.Count < 2)
                return null;

            object? primeiro = null;
            object? ultimo = null;

            foreach (var i in indices)
            {
                var valor = coluna.PegarValor(i);
                if (valor == null) continue;

                if (primeiro == null)
                    primeiro = valor;

                ultimo = valor;
            }

            if (primeiro == null || ultimo == null)
                return null;

            return (Single)ultimo - (Single)primeiro;
        }

        private void AdicionarColunaTipada<T>(DataFrame df, string nomeColuna, List<object?> valores)
        {
            var listaTipada = new List<T?>(valores.Count);

            foreach (var v in valores)
            {
                listaTipada.Add((T?)v);
            }

            df.AdicionarColuna<T?>(nomeColuna, listaTipada);
        }
    }
}
