using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorViewModel.Context
{
    public interface IContext<T> where T : class
    {
        T Mensagem { get; set; }
    }
}
