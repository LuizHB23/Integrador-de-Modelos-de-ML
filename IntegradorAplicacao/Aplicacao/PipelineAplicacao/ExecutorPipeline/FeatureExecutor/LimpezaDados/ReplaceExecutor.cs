using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class ReplaceExecutor : FeatureExecutorBase<Replace>
    {
        public ReplaceExecutor(Replace operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var colunaBase = dataFrame.PegarColunaBase(Operacao.col);

            if (colunaBase == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            var tipo = Nullable.GetUnderlyingType(colunaBase.TipoDado) ?? colunaBase.TipoDado;

            object? oldValue = Converter(tipo, Operacao.old);
            object? newValue = Converter(tipo, Operacao.value);

            if (colunaBase is Coluna<Single?> colFloat)
            {
                var span = colFloat.PegarColunaSpan();

                float? oldV = (float?)oldValue;
                float? newV = (float?)newValue;

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == oldV)
                        span[i] = newV;
                }

                return dataFrame;
            }

            if (colunaBase is Coluna<string> colString)
            {
                var span = colString.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == (string?)oldValue)
                        span[i] = (string?)newValue;
                }

                return dataFrame;
            }

            if (colunaBase is Coluna<DateTime?> colDate)
            {
                var span = colDate.PegarColunaSpan();

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == (DateTime?)oldValue)
                        span[i] = (DateTime?)newValue;
                }

                return dataFrame;
            }

            if (colunaBase is Coluna<bool?> colBool)
            {
                var span = colBool.PegarColunaSpan();

                bool? oldV = (bool?)oldValue;
                bool? newV = (bool?)newValue;

                for (int i = 0; i < span.Length; i++)
                {
                    if (span[i] == oldV)
                        span[i] = newV;
                }

                return dataFrame;
            }

            throw new Exception("Tipo não suportado para Replace");
        }

        private object? Converter(Type tipo, string? valor)
        {
            if (valor == null)
                return null;

            if (tipo == typeof(string))
                return valor;

            if (tipo == typeof(Single?) || tipo == typeof(Single))
                return Single.Parse(valor);

            if (tipo == typeof(Boolean?) || tipo == typeof(Boolean))
                return bool.Parse(valor);

            if (tipo == typeof(DateTime?) || tipo == typeof(DateTime))
                return DateTime.Parse(valor);

            // fallback genérico
            return Convert.ChangeType(valor, typeof(object));
        }
    }
}
