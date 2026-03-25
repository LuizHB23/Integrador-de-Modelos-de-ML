
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsPipelineModeloManager : CardsManager<ConfiguracaoCardFuncaoViewModel>
    {
        public CardsPipelineModeloManager(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicinarColuna(FuncaoItemViewModel funcaoItem, Action<ConfiguracaoCardFuncaoViewModel> actionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao)
        {
            _cardsLista.Add(new ConfiguracaoCardFuncaoViewModel(funcaoItem, actionExcluir, actionTrocarPosicao));
            AtualizaPosicoes();
        }

        public void CarregarSchema()
        {
            throw new NotImplementedException();
        }

        public void PreparaParaJson()
        {
            throw new NotImplementedException();
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);

        public override void RemoverColuna(ConfiguracaoCardFuncaoViewModel card) => base.RemoverColuna(card);
    }
}
