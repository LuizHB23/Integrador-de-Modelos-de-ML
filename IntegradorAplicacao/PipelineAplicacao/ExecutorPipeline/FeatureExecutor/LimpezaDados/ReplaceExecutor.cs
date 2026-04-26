using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.LimpezaDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.LimpezaDados
{
    public class ReplaceExecutor : FeatureExecutorBase<Replace>
    {
        public ReplaceExecutor(Replace operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColunaBase(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            var tipo = Nullable.GetUnderlyingType(coluna.TipoDado) ?? coluna.TipoDado;

            // 🔥 pré-conversão (evita converter dentro do loop)
            object? oldValue = Converter(tipo, Operacao.old);
            object? newValue = Converter(tipo, Operacao.value);

            int n = coluna.Quantidade;

            // 🔥 local reference (micro-otimização importante)
            var pegar = coluna.PegarValor;
            var set = coluna.InjetarValor;

            for (int i = 0; i < n; i++)
            {
                var atual = pegar(i);

                // evita Equals virtual pesado quando possível
                if (ReferenceEquals(atual, oldValue) ||
                    (atual != null && atual.Equals(oldValue)))
                {
                    set(i, newValue);
                }
            }

            return dataFrame;
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
