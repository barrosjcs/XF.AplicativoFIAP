using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.AplicativoFIAP.ViewModel;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace XF.AplicativoFIAP
{
	public partial class App : Application
	{
        public static ProfessorViewModel ProfessorVM { get; set; }

        public App ()
		{
			InitializeComponent();
            if (ProfessorVM == null) ProfessorVM = new ProfessorViewModel();

            MainPage = new NavigationPage(new View.MainPage() { BindingContext = ProfessorVM });
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
