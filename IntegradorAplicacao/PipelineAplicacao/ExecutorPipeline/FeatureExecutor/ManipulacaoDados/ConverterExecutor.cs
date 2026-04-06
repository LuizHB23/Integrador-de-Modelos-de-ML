using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class ConverterExecutor : FeatureExecutorBase<Converter>
    {
        public ConverterExecutor(Converter operacao) : base(operacao) { }

        public override DataFrame Executar(DataFrame dataFrame)
        {
            var n = dataFrame.QuantidadeLinhas;
            var nomeOrigem = Operacao.col;
            var tipoDestino = Operacao.type;

            var colunaBase = dataFrame.Colunas[dataFrame.ColunaIndex[nomeOrigem]];

            switch (tipoDestino.ToLower())
            {
                case "single":
                    List<float?> dadosFloat = ConverterParaSingle(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosFloat);
                    break;

                case "boolean":
                case "bool":
                    List<bool?> dadosBoolean = ConverterParaBool(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosBoolean);
                    break;

                case "string":
                case "str":
                    List<string?> dadosString = ConverterParaString(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosString);
                    break;

                case "datetime":
                    List<DateTime?> dadosdatetime = ConverterParaDateTime(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosdatetime);
                    break;
            }

            return dataFrame;
        }
        private List<float?> ConverterParaSingle(ColunaBase coluna, int n)
        {
            List<float?> resultado = new List<float?>();

            for (int i = 0; i < n; i++)
            {
                var valorOriginal = coluna.PegarValor(i);

                if (valorOriginal == null)
                {
                    resultado.Add(null);
                    continue;
                }
                else if (valorOriginal is string texto && string.IsNullOrWhiteSpace(texto.Trim()))
                {
                    resultado.Add(null);
                    continue;
                }
                else if (float.TryParse(valorOriginal.ToString().Replace(',', '.'), CultureInfo.InvariantCulture, out float valor))
                {
                    resultado.Add(valor);
                }
                else
                {
                    resultado.Add(null);
                }
            }

            return resultado;
        }

        private List<bool?> ConverterParaBool(ColunaBase coluna, int n)
        {
            List<bool?> resultado = new List<bool?>();

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(valor))
                {
                    resultado.Add(null);
                }
                else if (valor == "1")
                {
                    resultado.Add(true);
                }
                else if (valor == "0")
                {
                    resultado.Add(false);
                }
                else if (bool.TryParse(valor, out bool convertido))
                {
                    resultado.Add(convertido);
                }
                else
                {
                    resultado.Add(null);
                }
            }

            return resultado;
        }

        private List<string?> ConverterParaString(ColunaBase coluna, int n)
        {
            List<string?> resultado = new List<string?>();

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i);
                resultado.Add(valor?.ToString());
            }

            return resultado;
        }

        private List<DateTime?> ConverterParaDateTime(ColunaBase coluna, int n)
        {
            List<DateTime?> resultado = new List<DateTime?>();

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i);

                if (valor == null)
                {
                    resultado.Add(null);
                    continue;
                }

                if (DateTime.TryParse(valor.ToString(), new CultureInfo("en-US"),DateTimeStyles.AllowWhiteSpaces, out DateTime convertido))
                    resultado.Add(convertido);
                else
                    resultado.Add(null);
            }

            return resultado;
        }
    }
}
