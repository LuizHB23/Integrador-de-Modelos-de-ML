using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.Pipeline.InterfacesSteps
{
    public interface IStepTransform
    {
        public string NomeExibicao { get; }
        public void ExecutarTransform();
    }
}
