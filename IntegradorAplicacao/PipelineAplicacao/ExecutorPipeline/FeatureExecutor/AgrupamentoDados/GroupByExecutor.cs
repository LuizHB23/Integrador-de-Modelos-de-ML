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
            var colunasChave = TransformaStringColunasEmListaColunas(Operacao.col); // múltiplas colunas
            var agregacao = Operacao.agg?.ToLower();

            if (colunasChave == null || colunasChave.Count == 0)
                throw new Exception("É necessário informar pelo menos uma coluna-chave para o groupby.");

            // 🔹 2. Criar grupos por combinação de chaves
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

            // 🔹 3. Preparar novo DataFrame
            var novoDataFrame = new DataFrame();
            var colunasResultado = new Dictionary<string, List<object?>>();

            foreach (var col in dataFrame.Colunas)
            {
                colunasResultado[col.Nome] = new List<object?>();
            }

            // 🔹 4. Função de agregação
            Func<ColunaBase, List<int>, object?> functionAgregacao = agregacao?.ToLower() switch
            {
                "sum" => AgregacaoSoma,
                "count" => AgregacaoCount,
                "mean" => AgregacaoMedia,
                "std" => AgregacaoDesvioPadrao,
                "min" => AgregacaoMinimo,
                "max" => AgregacaoMaximo,
                "diff" => AgregacaoDiff,
                _ => throw new Exception($"Operação {agregacao} não suportada")
            };

            bool ehDiff = agregacao == "diff";

            // 🔹 5. Processar cada grupo
            foreach (var grupo in grupos)
            {
                var indices = grupo.Value;

                foreach (var col in dataFrame.Colunas)
                {
                    if (colunasChave.Contains(col.Nome))
                    {
                        // pegar a primeira chave para cada grupo
                        colunasResultado[col.Nome].Add(col.PegarValor(indices[0]));
                    }
                    else
                    {
                        var resultado = functionAgregacao(col, indices);
                        colunasResultado[col.Nome].Add(resultado);
                    }
                }
            }

            // 🔹 6. Adicionar colunas ao novo DataFrame dinamicamente
            foreach (var col in dataFrame.Colunas)
            {
                Type tipo = col.TipoDado;

                AdicionarColunaTipadaDynamic(novoDataFrame, col.Nome, colunasResultado[col.Nome], tipo, ehDiff);
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

        private void AdicionarColunaTipadaDynamic(DataFrame df, string nomeColuna, List<object?> valores, Type tipoOriginal, bool ehDiff = false)
        {
            Type tipoFinal = (ehDiff && tipoOriginal == typeof(DateTime?)) ? typeof(Single?) : tipoOriginal;

            var listaTipada = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tipoFinal))!;

            foreach (var v in valores)
            {
                if (v == null)
                {
                    listaTipada.Add(null);
                }
                else
                {
                    object valorConvertido;

                    if (ehDiff && tipoOriginal == typeof(DateTime))
                    {
                        // Converte DateTime para Single (dias)
                        if (v is DateTime dt)
                        {
                            valorConvertido = Convert.ToSingle(dt.Subtract(DateTime.MinValue).TotalDays);
                        }
                        else
                        {
                            valorConvertido = Convert.ToSingle(v); // já deve ser Single
                        }
                    }
                    else
                    {
                        // Mantém o tipo original
                        valorConvertido = Convert.ChangeType(v, Nullable.GetUnderlyingType(tipoFinal) ?? tipoFinal);
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
            List<string> colunasParaRemover = new();

            foreach (var coluna in texto)
            {
                colunasParaRemover.Add(coluna.Trim().Trim('"'));
            }

            return colunasParaRemover;
        }
    }
}
