using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.AplicativoFIAP.ViewModel;

namespace XF.AplicativoFIAP.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NovoProfessorView : ContentPage
	{
        public static ProfessorViewModel ProfessorVM { get; set; }

        public NovoProfessorView ()
		{
			InitializeComponent ();
		}
	}
}