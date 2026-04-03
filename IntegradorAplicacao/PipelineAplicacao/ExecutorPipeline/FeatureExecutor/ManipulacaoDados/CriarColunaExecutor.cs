using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System.Collections;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class CriarColunaExecutor : FeatureExecutorBase<CriarColuna>
    {
        public CriarColunaExecutor(CriarColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            Type tipoColuna = typeof(object);
            object? valor = null;

            if(Operacao.value != "")
            {
                switch (Operacao.type)
                {
                    case "single":
                        tipoColuna = typeof(Single?);
                        valor = Convert.ToSingle(Operacao.value);
                        break;

                    case "boolean":
                        tipoColuna = typeof(Boolean?);
                        valor = Convert.ToBoolean(Operacao.value);
                        break;

                    case "bool":
                        tipoColuna = typeof(Boolean?);
                        valor = Convert.ToBoolean(Operacao.value);
                        break;

                    case "string":
                        tipoColuna = typeof(String);
                        valor = Convert.ToString(Operacao.value);
                        break;

                    case "str":
                        tipoColuna = typeof(String);
                        valor = Convert.ToString(Operacao.value);
                        break;

                    case "datetime":
                        tipoColuna = typeof(DateTime?);
                        valor = Convert.ToDateTime(Operacao.value);
                        break;
                }
            }

            // Se o valor for string vazia, tratamos como nulo
            if (valor is string s && string.IsNullOrWhiteSpace(s))
                valor = null;

            // Criar a lista do tipo correto
            Type listType = typeof(List<>).MakeGenericType(tipoColuna);
            var listaNova = (IList)Activator.CreateInstance(listType)!;

            // Preencher a lista com o valor (ou nulo) repetido para todas as linhas
            int quantidadeLinhas = dataFrame.Colunas.Count > 0 ? dataFrame.QuantidadeLinhas : 1;
            for (int i = 0; i < quantidadeLinhas; i++)
            {
                listaNova.Add(valor);
            }

            // Criar a coluna dinamicamente
            Type colunaTipo = typeof(Coluna<>).MakeGenericType(tipoColuna);
            var construtorColuna = colunaTipo.GetConstructor(new Type[] { typeof(string), listType })!;
            var novaColuna = construtorColuna.Invoke(new object[] { Operacao.name, listaNova });

            // Adicionar ao DataFrame
            var metodoAdd = typeof(DataFrame).GetMethod("AdicionarColuna")!.MakeGenericMethod(tipoColuna);
            metodoAdd.Invoke(dataFrame, new object[] { Operacao.name, listaNova });

            return dataFrame;
        }
    }
}
