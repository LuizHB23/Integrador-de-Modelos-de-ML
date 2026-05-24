using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IntegradorAplicacao.Infraestrutura.Gerenciador
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
            string appFolder = _provider.GetCaminhoPastaModelo(transformador.NomeModelo);
            appFolder = Path.Combine(appFolder, "Transformadores");
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
