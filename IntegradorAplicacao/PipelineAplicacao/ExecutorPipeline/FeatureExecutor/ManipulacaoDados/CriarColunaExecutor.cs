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
            List<object?>? valoresExistentes = null;

            // Verifica se value é o nome de um DataFrame existente
            if (!string.IsNullOrWhiteSpace(Operacao.value) && Operacao.Contexto != null && Operacao.Contexto.ContainsKey(Operacao.value))
            {
                if (Operacao.Contexto[Operacao.value] is DataFrame dfExistente)
                {
                    // Pegamos a primeira coluna do DataFrame existente (ou você pode parametrizar qual coluna)
                    var colunaBase = dfExistente.Colunas.Count > 0 ? dfExistente.Colunas[0] : null;

                    if (colunaBase != null)
                    {
                        tipoColuna = colunaBase.TipoDado;
                        valoresExistentes = new List<object?>();
                        for (int i = 0; i < colunaBase.Quantidade; i++)
                        {
                            valoresExistentes.Add(colunaBase.PegarValor(i));
                        }
                    }
                }
            }

            // Se não for DataFrame existente, usamos o valor fixo
            if (valoresExistentes == null && Operacao.value != "")
            {
                switch (Operacao.type)
                {
                    case "single":
                        tipoColuna = typeof(Single?);
                        valor = Convert.ToSingle(Operacao.value);
                        break;

                    case "boolean":
                    case "bool":
                        tipoColuna = typeof(Boolean?);
                        valor = Convert.ToBoolean(Operacao.value);
                        break;

                    case "string":
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
            var listaNova = (System.Collections.IList)Activator.CreateInstance(listType)!;

            int quantidadeLinhas = dataFrame.Colunas.Count > 0 ? dataFrame.QuantidadeLinhas : 1;

            if (valoresExistentes != null)
            {
                // Preenche a nova coluna com os valores do DataFrame existente
                foreach (var v in valoresExistentes)
                    listaNova.Add(v);
            }
            else
            {
                // Preencher a lista com o valor (ou nulo) repetido para todas as linhas
                for (int i = 0; i < quantidadeLinhas; i++)
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
