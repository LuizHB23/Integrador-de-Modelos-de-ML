using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.AgrupamentoDados;
using System.ComponentModel.DataAnnotations;


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
            bool ehDiff = false;

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

                case "std":
                    functionAgregacao = AgregacaoDesvioPadrao;
                    break;

                case "min":
                    functionAgregacao = AgregacaoMinimo;
                    break;

                case "max":
                    functionAgregacao = AgregacaoMaximo;
                    break;

                case "diff":
                    ehDiff = true;
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

                Type tipo;

                if (ehDiff && colunaOriginal.TipoDado == typeof(DateTime))
                {
                    tipo = typeof(Single);
                }
                else
                {
                    tipo = colunaOriginal.TipoDado;
                }

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
            Single? valor;
            foreach (var i in indices)
            {
                valor = (Single?)coluna.PegarValor(i);
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
            Single? valor;

            foreach (var i in indices)
            {
                valor = (Single?)coluna.PegarValor(i);
                if (valor != null)
                {
                    total += (Single)valor;
                    contar++;
                }
            }

            return contar == 0 ? null : total / contar;
        }

        private object? AgregacaoDesvioPadrao(ColunaBase coluna, List<int> indices)
        {
            List<Single> valores = new List<Single>();

            foreach (var i in indices)
            {
                var valor = (Single?)coluna.PegarValor(i);
                if (valor != null)
                    valores.Add((Single)valor);
            }

            if (valores.Count == 0)
                return null;

            // calcula média
            Single media = valores.Sum() / valores.Count;

            // calcula variância
            Single variancia = valores.Sum(v => (v - media) * (v - media)) / valores.Count;

            // desvio padrão
            return (Single)Math.Sqrt(variancia);
        }

        private object? AgregacaoMinimo(ColunaBase coluna, List<int> indices)
        {
            object? min = null;
            Single? valor;

            foreach (var i in indices)
            {
                valor = (Single?)coluna.PegarValor(i);
                if (valor == null) continue;

                if (min == null || ((IComparable)valor).CompareTo(min) < 0)
                    min = valor;
            }
            return min;
        }

        private object? AgregacaoMaximo(ColunaBase coluna, List<int> indices)
        {
            object? max = null;
            Single? valor;

            foreach (var i in indices)
            {
                valor = (Single?)coluna.PegarValor(i);
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
            object? valor;

            foreach (var i in indices)
            {
                valor = coluna.PegarValor(i);
                if (valor == null) continue;

                if (primeiro == null)
                    primeiro = valor;

                ultimo = valor;
            }

            if (primeiro == null || ultimo == null)
                return null;

            Single? resultado;

            if (ultimo is DateTime && primeiro is DateTime)
            {
                TimeSpan tempo = (DateTime)ultimo - (DateTime)primeiro;
                resultado = Convert.ToSingle(tempo.TotalDays);
            }
            else
            {
                resultado = (Single)ultimo - (Single)primeiro;
            }

            return resultado;
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
