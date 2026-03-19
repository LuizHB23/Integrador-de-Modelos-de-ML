using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.Context
{
    public class NomeModeloContext : IContext<string>
    {
        public string Mensagem { get; set; } = string.Empty;
    }
}
