using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.ItensViewModel
{
    public class TransformadorItemViewModel
    {
        public int Posicao { get; set; }
        public string NomeTransformador { get; set; } = string.Empty;
        public string CaminhoTransformador { get; set; } = string.Empty;

        public TransformadorItemViewModel(int posicao, string nomeTransformador, string caminhoTransformador)
        {
            Posicao = posicao;
            NomeTransformador = nomeTransformador;
            CaminhoTransformador = caminhoTransformador;
        }
    }
}
