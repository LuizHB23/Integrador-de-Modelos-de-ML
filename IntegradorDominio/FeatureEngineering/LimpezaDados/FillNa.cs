using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class FillNa : IFeature
    {
        public string NomeExibicao => "Fill NA";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
