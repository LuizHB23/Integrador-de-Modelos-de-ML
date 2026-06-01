using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public abstract class CardsManager<T> where T : IConfiguracaoCard
    {
        protected ObservableCollection<T> _cardsLista;
        protected ObservableCollection<int> _posicoesLista;
        protected ModeloDTO _modelo;

        public CardsManager(ObservableCollection<T> cardsLista, ObservableCollection<int> posicoesLista, ModeloDTO modelo)
        {
            _cardsLista = cardsLista;
            _posicoesLista = posicoesLista;
            _modelo = modelo;
        }

        public virtual void AtualizaPosicoes()
        {
            for (int i = 0; i < _cardsLista.Count; i++)
            {
                _cardsLista[i].EstouReposicionando = true;

                _cardsLista[i].OpcoesPosicao = _posicoesLista;

                _cardsLista[i].Posicao = i + 1;

                _cardsLista[i].EstouReposicionando = false;
            }
        }

        public virtual async Task OrganizaPosicao(T card, int posicaoNova)
        {
            int posicaoOriginal = _cardsLista.IndexOf(card);

            _cardsLista.Move(posicaoOriginal, posicaoNova);

            AtualizaPosicoes();
        }

        public virtual async Task RemoverCard(T card)
        {
            _cardsLista.Remove(card);
            _posicoesLista.Remove(_cardsLista.Count + 1);
            AtualizaPosicoes();
        }
    }
}
