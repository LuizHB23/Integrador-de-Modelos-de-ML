using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class ArredondarExecutor : FeatureExecutorBase<Arredondar>
    {
        public ArredondarExecutor(Arredondar operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);

            if (coluna == null)
                throw new Exception($"Coluna '{Operacao.col}' não encontrada.");

            int casas = Convert.ToInt32(Operacao.value);

            var span = coluna.PegarColunaSpan();

            for (int i = 0; i < span.Length; i++)
            {
                var valor = span[i];
                if (valor.HasValue)
                    span[i] = MathF.Round(valor.Value, casas);
            }

            return dataFrame;
        }
    }
}
