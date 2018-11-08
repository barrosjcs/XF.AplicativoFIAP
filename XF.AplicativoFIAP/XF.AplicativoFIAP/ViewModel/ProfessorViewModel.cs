using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using XF.AplicativoFIAP.Model;

namespace XF.AplicativoFIAP.ViewModel
{
    public class ProfessorViewModel : INotifyPropertyChanged
    {
        List<Professor> CopiarListaProfessores;

        public ObservableCollection<Professor> Professores { get; set; } = new ObservableCollection<Professor>();

        private Professor selecionado;
        public Professor Selecionado
        {
            get { return selecionado; }
            set
            {
                selecionado = value as Professor;
                EventPropertyChanged();
            }
        }

        // UI Events
        public OnAdicionarProfessorCMD OnAdicionarProfessorCMD { get; }
        public OnEditarProfessorCMD OnEditarProfessorCMD { get; }
        public OnDeleteProfessorCMD OnDeleteProfessorCMD { get; }
        public ICommand OnSairCMD { get; private set; }
        public ICommand OnNovoCMD { get; private set; }

        static object locker = new object();

        private string pesquisaPorNome;
        public string PesquisaPorNome
        {
            get { return pesquisaPorNome; }
            set
            {
                if (value == pesquisaPorNome) return;

                pesquisaPorNome = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PesquisaPorNome)));
                AplicarFiltro();
            }
        }

        public ProfessorViewModel()
        {
            OnAdicionarProfessorCMD = new OnAdicionarProfessorCMD(this);
            OnEditarProfessorCMD = new OnEditarProfessorCMD(this);
            OnDeleteProfessorCMD = new OnDeleteProfessorCMD(this);
            OnSairCMD = new Command(OnSair);
            OnNovoCMD = new Command(OnNovo);

            CopiarListaProfessores = new List<Professor>();
            Carregar();
        }

        public async void Carregar()
        {
            CopiarListaProfessores = await ProfessorRepository.GetProfessoresSqlAzureAsync();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            if (pesquisaPorNome == null)
                pesquisaPorNome = "";

            var resultado = CopiarListaProfessores.Where(n => n.Nome.ToLowerInvariant().
                            Contains(PesquisaPorNome.ToLowerInvariant().Trim())).ToList();

            Professores.Clear();
            if (resultado != null && resultado.Any())
            {
                foreach (var item in resultado)
                {
                    Professores.Add(item);
                }
            }
            else
            {
                foreach (var item in CopiarListaProfessores)
                {
                    Professores.Add(item);
                }
            }
        }

        public async void Adicionar(Professor paramProfessor)
        {
            if (paramProfessor == null || string.IsNullOrWhiteSpace(paramProfessor.Nome))
                await Application.Current.MainPage.DisplayAlert("Atenção", "O campo nome é obrigatório", "OK");
            else if (await ProfessorRepository.PostProfessorSqlAzureAsync(paramProfessor))
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
                await Application.Current.MainPage.DisplayAlert("Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
        }

        public async void Editar()
        {
            await Application.Current.MainPage.Navigation.PushAsync(
                new View.NovoProfessorView() { BindingContext = App.ProfessorVM });
        }

        public async void Remover()
        {
            if (await Application.Current.MainPage.DisplayAlert("Atenção?",
                string.Format("Tem certeza que deseja remover o {0}?", Selecionado.Nome), "Sim", "Não"))
            {
                if (await ProfessorRepository.DeleteProfessorSqlAzureAsync(Selecionado.Id.ToString()))
                {
                    CopiarListaProfessores.Remove(Selecionado);
                    Carregar();
                }
                else
                    await Application.Current.MainPage.DisplayAlert(
                            "Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
            }
        }

        private async void OnSair()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void OnNovo()
        {
            App.ProfessorVM.Selecionado = new Professor();
            Application.Current.MainPage.Navigation.PushAsync(
                new View.NovoProfessorView() { BindingContext = App.ProfessorVM });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void EventPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class OnAdicionarProfessorCMD : ICommand
    {
        private ProfessorViewModel professorVM;
        public OnAdicionarProfessorCMD(ProfessorViewModel paramVM)
        {
            professorVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void AdicionarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            professorVM.Adicionar(parameter as Professor);
        }
    }

    public class OnEditarProfessorCMD : ICommand
    {
        private ProfessorViewModel professorVM;
        public OnEditarProfessorCMD(ProfessorViewModel paramVM)
        {
            professorVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void EditarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.ProfessorVM.Selecionado = parameter as Professor;
            professorVM.Editar();
        }
    }

    public class OnDeleteProfessorCMD : ICommand
    {
        private ProfessorViewModel professorVM;
        public OnDeleteProfessorCMD(ProfessorViewModel paramVM)
        {
            professorVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void DeleteCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.ProfessorVM.Selecionado = parameter as Professor;
            professorVM.Remover();
        }
    }
}
