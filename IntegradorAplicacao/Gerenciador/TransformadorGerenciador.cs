using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IntegradorAplicacao.Gerenciador
{
    public class TransformadorGerenciador : IGerenciador<TransformadorDTO>
    {
        private readonly IPathProvider _provider;

        public TransformadorGerenciador(IPathProvider provider)
        {
            _provider = provider;
        }

        public string Salvar(TransformadorDTO transformador)
        {
            string appFolder = _provider.GetCaminhoModelo();
            appFolder = Path.Combine(appFolder, transformador.NomeModelo, "Transformadores");
            Directory.CreateDirectory(appFolder);

            string nomeArquivo = Path.GetFileName(transformador.CaminhoTransformador);
            string caminhoDestino = Path.Combine(appFolder, nomeArquivo);

            File.Copy(transformador.CaminhoTransformador, caminhoDestino, true);

            return caminhoDestino;
        }

        public void Carregar(TransformadorDTO objeto)
        {
            throw new NotImplementedException();
        }
    }
}
