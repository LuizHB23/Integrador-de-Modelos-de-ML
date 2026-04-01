using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.OperacoesEspeciais;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.OperacoesEspeciais
{
    public class AbsolutoExecutor : FeatureExecutorBase<Absoluto>
    {
        public AbsolutoExecutor(Absoluto operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var coluna = dataFrame.PegarColuna<Single?>(Operacao.col);
            int quantidadeLinhas = dataFrame.QuantidadeLinhas;
            var resultado = new List<Single?>();
            Single? valor;

            for (int i = 0; i < quantidadeLinhas; i++)
            {
                valor = (Single?)coluna.PegarValor(i);
                
                if(valor < 0 && valor is not null)
                {
                    resultado.Add(valor * (-1));
                }
                else
                {
                    resultado.Add(valor);
                }
            }

            dataFrame.AlterarColuna(Operacao.col, resultado);

            return dataFrame;
        }
    }
}
