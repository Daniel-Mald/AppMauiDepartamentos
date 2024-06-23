using AppMauiDepartamentos.ViewModels;

namespace AppMauiDepartamentos.Views;

public partial class MisBorradoresView : ContentPage
{
	public MisBorradoresView(ActividadesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}