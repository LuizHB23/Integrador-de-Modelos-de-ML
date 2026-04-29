using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class RemoverNuloExecutor : FeatureExecutorBase<RemoverNulo>
    {
        public RemoverNuloExecutor(RemoverNulo operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            if (dataFrame == null || dataFrame.QuantidadeLinhas == 0)
                return dataFrame;

            int rows = dataFrame.QuantidadeLinhas;
            int cols = dataFrame.Colunas.Count;

            var colunas = dataFrame.Colunas;

            var novo = new DataFrame();

            // pré-cria colunas (evita re-reflection no loop de linhas)
            for (int c = 0; c < cols; c++)
            {
                var col = colunas[c];

                var tipoLista = typeof(List<>).MakeGenericType(col.TipoDado);
                var lista = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;

                novo.AdicionarColuna(col.Nome, lista as dynamic);
            }

            // buffer de chave reutilizável (evita StringBuilder por linha)
            Span<char> buffer = stackalloc char[1024]; // suficiente para chaves comuns
            var seen = new HashSet<string>(rows);

            for (int i = 0; i < rows; i++)
            {
                int pos = 0;
                bool hasNull = false;

                for (int j = 0; j < cols; j++)
                {
                    var v = colunas[j].PegarValor(i);

                    if (v == null)
                    {
                        hasNull = true;
                        break;
                    }

                    if (v is string str && string.IsNullOrEmpty(str))
                    {
                        hasNull = true;
                        break;
                    }

                    var s = v.ToString();

                    // escreve no span (rápido, sem StringBuilder)
                    foreach (var ch in s)
                    {
                        if (pos < buffer.Length)
                            buffer[pos++] = ch;
                    }

                    if (pos < buffer.Length)
                        buffer[pos++] = '|';
                }

                if (hasNull)
                    continue;

                string key = new string(buffer.Slice(0, pos));

                if (!seen.Add(key))
                    continue;

                // adiciona linha
                for (int j = 0; j < cols; j++)
                {
                    var v = colunas[j].PegarValor(i);
                    novo.Colunas[j].AdicionaValor(v);
                }
            }

            return novo;
        }
    }
}
