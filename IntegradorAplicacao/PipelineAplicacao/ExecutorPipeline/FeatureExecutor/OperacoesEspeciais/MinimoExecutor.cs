using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class MinimoExecutor : FeatureExecutorBase<Minimo>
    {
        public MinimoExecutor(Minimo operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            object? minimo = null;
            Single? valor;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                valor = (Single?)coluna.PegarValor(i);
                if (valor == null) continue;

                if (minimo == null || ((IComparable)valor).CompareTo(minimo) < 0)
                    minimo = valor;
            }

            return minimo!;
        }
    }
}
