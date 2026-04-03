using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.FeatureEngineering.ManipulacaoDados;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.ManipulacaoDados
{
    public class RenomearColunaExecutor : FeatureExecutorBase<RenomearColuna>
    {
        public RenomearColunaExecutor(RenomearColuna operacao) : base(operacao) { }

        public override object Executar(DataFrame dataFrame)
        {
            var listaAntigoNome = TransformaStringEmLista(Operacao.col);
            var listaNovoNome = TransformaStringEmLista(Operacao.name);

            for(int i = 0; i < listaAntigoNome.Count; i++)
            {
                dataFrame.RenomearColunas(listaAntigoNome[i], listaNovoNome[i]);
            }

            return dataFrame;
        }

        private List<string> TransformaStringEmLista(string textoMudar)
        {
            var texto = textoMudar.Trim('[', ']').Split(',');
            List<string> lista = new();

            foreach (var coluna in texto)
            {
                lista.Add(coluna.Trim().Trim('"'));
            }

            return lista;
        }
    }
}
