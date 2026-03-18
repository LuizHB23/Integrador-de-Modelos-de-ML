using IntegradorAplicacao.DTO;
using IntegradorViewModel.ItensViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.Context
{
    public class ConfiguracaoCardSchemaContext : IContext<SchemaItemViewModel>
    {
        public SchemaItemViewModel? Mensagem { get; set; } = null;
    }
}
