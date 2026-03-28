using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class TrocarTipo : IFeature
    {
        public string NomeExibicao => "Trocar Tipo";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
