using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.Context
{
    public class InserirModeloContext : IContext<string>
    {
        public string Mensagem { get; set; } = string.Empty;
    }
}
