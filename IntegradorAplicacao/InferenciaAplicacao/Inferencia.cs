using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao;
using IntegradorDominio.DataFrameModel;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IntegradorAplicacao.InferenciaAplicacao
{
    public class Inferencia
    {
        private IConverteJson<Dictionary<int, FuncaoDTO>> _conversorPipeline;

        private readonly ExecutorFinal _executor;
        private readonly MLContext _mlContext;
        private readonly string _caminhoModelo;
        private readonly string _caminhoSchema;

        public Inferencia(IConverteJson<Dictionary<int, FuncaoDTO>> conversorPipeline, string caminhoModelo, string caminhoSchema)
        {
            _conversorPipeline = conversorPipeline;

            _executor = new ExecutorFinal(_conversorPipeline);
            _caminhoModelo = caminhoModelo;
            _caminhoSchema = caminhoSchema;
            _mlContext = new MLContext();
        }

        public void RealizaInferencia(DataFrame dataFrame)
        {
            var dataFrameNovo = RealizaFeatureEngineering(dataFrame);
            var resultado = ToIDataView(dataFrameNovo);

            //var listaDePrevisoes = _mlContext.Data.CreateEnumerable<ModelOutput>(resultado, reuseRowObject: false);

            //return resultado;
        }

        public DataFrame RealizaFeatureEngineering(DataFrame dataFrame)
        {
            _executor.ConstroiSequenciaMetodoPipeline(_caminhoModelo);
            var dataFrameNovo = _executor.ExecutarTudo(dataFrame);
            return dataFrameNovo;
        }

        public IDataView ToIDataView(DataFrame dataFrame)
        {
            var list = new List<ExpandoObject>();
            var colunas = dataFrame.Colunas;
            int qtdLinhas = dataFrame.QuantidadeLinhas;

            for (int i = 0; i < qtdLinhas; i++)
            {
                var row = new ExpandoObject() as IDictionary<string, object>;
                foreach (var col in colunas)
                {
                    // Adiciona cada coluna pelo nome com seu valor original (string, bool, etc)
                    row[col.Nome] = col.PegarValor(i);
                }
                list.Add((ExpandoObject)row);
            }

            // O ML.NET tentará mapear os tipos do ExpandoObject
            return _mlContext.Data.LoadFromEnumerable(list);
        }
    }
}
