using IntegradorDominio.InterfacesSteps;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.FeatureEngineering.LimpezaDados
{
    public class RemoverColuna : IFeature
    {
        public string NomeExibicao => "Remover Coluna";
        public string NomeCodigo => throw new NotImplementedException();
    }
}
