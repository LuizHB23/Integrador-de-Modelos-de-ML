using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
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
                    float[] dadosFloat = ConverterParaSingle(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosFloat);
                    break;

                case "boolean":
                    bool[] dadosBoolean = ConverterParaBool(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosBoolean);
                    break;

                case "bool":
                    bool[] dadosBool = ConverterParaBool(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosBool);
                    break;

                case "string":
                    string[] dadosString = ConverterParaString(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosString);
                    break;

                case "str":
                    string[] dadosStr = ConverterParaString(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosStr);
                    break;

                case "datetime":
                    DateTime[] dadosdatetime = ConverterParaDateTime(colunaBase, n);
                    dataFrame.AlteraColuna(Operacao.col, dadosdatetime);
                    break;
            }

            return dataFrame;
        }
        private float[] ConverterParaSingle(ColunaBase coluna, int n)
        {
            float[] resultado = new float[n];
            for (int i = 0; i < n; i++)
            {
                resultado[i] = Convert.ToSingle(coluna.PegarValor(i));
            }
            return resultado;
        }

        private bool[] ConverterParaBool(ColunaBase coluna, int n)
        {
            bool[] resultado = new bool[n];

            for (int i = 0; i < n; i++)
            {
                var valor = coluna.PegarValor(i)?.ToString()?.Trim();

                if (valor == "1")
                {
                    resultado[i] = true;
                }
                else if (valor == "0")
                {
                    resultado[i] = false;
                }
                else
                {
                    resultado[i] = Convert.ToBoolean(valor);
                }
            }
            return resultado;
        }

        private string[] ConverterParaString(ColunaBase coluna, int n)
        {
            string[] resultado = new string[n];
            for (int i = 0; i < n; i++)
            {
                resultado[i] = Convert.ToString(coluna.PegarValor(i))!;
            }
            return resultado;
        }

        private DateTime[] ConverterParaDateTime(ColunaBase coluna, int n)
        {
            DateTime[] resultado = new DateTime[n];
            for (int i = 0; i < n; i++)
            {
                resultado[i] = Convert.ToDateTime(coluna.PegarValor(i));
            }
            return resultado;
        }
    }
}
