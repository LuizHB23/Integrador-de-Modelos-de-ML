using IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class MaximoExecutor : FeatureExecutorBase<Maximo>
    {
        public MaximoExecutor(Maximo operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            object? maximo = null;
            Single? valor;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                valor = (Single?)coluna.PegarValor(i);
                if (valor == null) continue;

                if (maximo == null || ((IComparable)valor).CompareTo(maximo) > 0)
                    maximo = valor;
            }

            return maximo!;
        }
    }
}
