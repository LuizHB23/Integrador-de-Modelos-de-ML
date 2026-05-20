using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
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
                    if (colunaBase is Coluna<float?>)
                        break;

                    float?[] dadosFloat = ConverterParaSingle(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosFloat);
                    break;

                case "boolean":
                case "bool":
                    if (colunaBase is Coluna<bool?>)
                        break;

                    bool?[] dadosBoolean = ConverterParaBool(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosBoolean);
                    break;

                case "string":
                case "str":
                    if (colunaBase is Coluna<string?>)
                        break;

                    string?[] dadosString = ConverterParaString(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosString);
                    break;

                case "datetime":
                    if (colunaBase is Coluna<DateTime?>)
                        break;

                    DateTime?[] dadosdatetime = ConverterParaDateTime(colunaBase, n);
                    dataFrame.AlterarColuna(Operacao.col, dadosdatetime);
                    break;
            }

            return dataFrame;
        }
        private float?[] ConverterParaSingle(ColunaBase colunaBase, int n)
        {
            var resultado = new float?[n];

            if (colunaBase is Coluna<string?> colunaString)
            {
                var span = colunaString.PegarColunaSpan();

                for (int i = 0; i < n; i++)
                {
                    var texto = span[i];

                    if (string.IsNullOrWhiteSpace(texto))
                        continue;

                    if (float.TryParse(texto.Replace(',', '.'),
                        CultureInfo.InvariantCulture, out float valor))
                        resultado[i] = valor;
                }

                return resultado;
            }

            for (int i = 0; i < n; i++)
            {
                var valorOriginal = colunaBase.PegarValor(i);

                if (valorOriginal == null)
                    continue;

                if (float.TryParse(valorOriginal.ToString(), out float valor))
                    resultado[i] = valor;
            }

            return resultado;
        }

        private bool?[] ConverterParaBool(ColunaBase coluna, int n)
        {
            var resultado = new bool?[n];

            if (coluna is Coluna<string?> colStr)
            {
                var span = colStr.PegarColunaSpan();

                for (int i = 0; i < n; i++)
                {
                    var valor = span[i]?.Trim();

                    if (string.IsNullOrEmpty(valor))
                        continue;
                    else if (valor == "1")
                        resultado[i] = true;
                    else if (valor == "0")
                        resultado[i] = false;
                    else if (bool.TryParse(valor, out bool convertido))
                        resultado[i] = convertido;
                }

                return resultado;
            }

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(valor))
                    continue;
                else if (valor == "1")
                    resultado[i] = true;
                else if (valor == "0")
                    resultado[i] = false;
                else if (bool.TryParse(valor, out bool convertido))
                    resultado[i] = convertido;
            }

            return resultado;
        }

        private string?[] ConverterParaString(ColunaBase coluna, int n)
        {
            var resultado = new string?[n];

            if (coluna is Coluna<string?> colStr)
            {
                var span = colStr.PegarColunaSpan();

                for (int i = 0; i < n; i++)
                    resultado[i] = span[i];

                return resultado;
            }

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i);
                resultado[i] = valor?.ToString();
            }

            return resultado;
        }

        private DateTime?[] ConverterParaDateTime(ColunaBase coluna, int n)
        {
            var resultado = new DateTime?[n];

            if (coluna is Coluna<string?> colStr)
            {
                var span = colStr.PegarColunaSpan();

                for (int i = 0; i < n; i++)
                {
                    var valor = span[i];

                    if (string.IsNullOrWhiteSpace(valor))
                        continue;

                    if (DateTime.TryParse(valor,
                        new CultureInfo("en-US"),
                        DateTimeStyles.AllowWhiteSpaces,
                        out DateTime convertido))
                    {
                        resultado[i] = convertido;
                    }
                }

                return resultado;
            }

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i);

                if (valor == null)
                    continue;

                if (DateTime.TryParse(valor.ToString(),
                    new CultureInfo("en-US"),
                    DateTimeStyles.AllowWhiteSpaces,
                    out DateTime convertido))
                {
                    resultado[i] = convertido;
                }
            }

            return resultado;
        }
    }
}
