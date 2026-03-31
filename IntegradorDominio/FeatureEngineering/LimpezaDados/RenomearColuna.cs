using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class RenomearColuna : IFeature
    {
        public string NomeExibicao => "Renomear Coluna";
        public Dictionary<string, object?>? Contexto { get; set; }
    }
}
