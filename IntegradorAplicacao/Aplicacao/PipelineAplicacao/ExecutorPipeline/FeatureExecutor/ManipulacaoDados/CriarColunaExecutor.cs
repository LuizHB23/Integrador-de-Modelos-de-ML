using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System.Collections;
using System.Drawing;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class CriarColunaExecutor : FeatureExecutorBase<CriarColuna>
    {
        public CriarColunaExecutor(CriarColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            string nome = Operacao.name;

            if (!string.IsNullOrWhiteSpace(Operacao.value) && Operacao.Contexto != null && Operacao.Contexto.TryGetValue(Operacao.value, out var ctx) && ctx is DataFrame dfExistente && dfExistente.Colunas.Count > 0)
            {
                var col = dfExistente.Colunas[0];

                int n = col.Quantidade;

                if (col is Coluna<float?> cf)
                {
                    var span = cf.PegarColunaSpan();
                    var arr = new float?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    dataFrame.AdicionarColuna<float?>(nome, arr.ToList());
                    return dataFrame;
                }

                if (col is Coluna<bool?> cb)
                {
                    var span = cb.PegarColunaSpan();
                    var arr = new bool?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    dataFrame.AdicionarColuna<bool?>(nome, arr.ToList());
                    return dataFrame;
                }

                if (col is Coluna<DateTime?> cd)
                {
                    var span = cd.PegarColunaSpan();
                    var arr = new DateTime?[n];

                    for (int i = 0; i < n; i++)
                        arr[i] = span[i];

                    dataFrame.AdicionarColuna<DateTime?>(nome, arr.ToList());
                    return dataFrame;
                }

                var objArr = new object?[n];
                for (int i = 0; i < n; i++)
                    objArr[i] = col.PegarValor(i);

                dataFrame.AdicionarColuna<object?>(nome, objArr.ToList());
                return dataFrame;
            }

            int rows = dataFrame.QuantidadeLinhas;

            switch (Operacao.type?.ToLower())
            {
                case "single":
                case "float":
                    {
                        float? value = VerificaNulidade(Operacao.value) ? null : Convert.ToSingle(Operacao.value);

                        var arr = new float?[rows];

                        for (int i = 0; i < rows; i++)
                            arr[i] = value;

                        dataFrame.AdicionarColuna<float?>(nome, arr.ToList());
                        break;
                    }

                case "boolean":
                case "bool":
                    {
                        bool? value = VerificaNulidade(Operacao.value) ? null : Convert.ToBoolean(Operacao.value);

                        var arr = new bool?[rows];

                        for (int i = 0; i < rows; i++)
                            arr[i] = value;

                        dataFrame.AdicionarColuna<bool?>(nome, arr.ToList());
                        break;
                    }

                case "datetime":
                    {
                        DateTime? value = VerificaNulidade(Operacao.value) ? null : Convert.ToDateTime(Operacao.value);

                        var arr = new DateTime?[rows];

                        for (int i = 0; i < rows; i++)
                            arr[i] = value;

                        dataFrame.AdicionarColuna<DateTime?>(nome, arr.ToList());
                        break;
                    }

                case "string":
                case "str":
                default:
                    {
                        string? value = Operacao.value;

                        var arr = new string?[rows];

                        for (int i = 0; i < rows; i++)
                            arr[i] = value;

                        dataFrame.AdicionarColuna<string?>(nome, arr.ToList());
                        break;
                    }
            }

            return dataFrame;
        }

        private bool VerificaNulidade(string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                || ValoresNulos.Contains(valor.Trim().ToLower());
        }

        private static readonly HashSet<string> ValoresNulos = new(StringComparer.OrdinalIgnoreCase)
        {
            "na",
            "n/a",
            "nan",
            "null",
            "none",
            ""
        };
    }
}
