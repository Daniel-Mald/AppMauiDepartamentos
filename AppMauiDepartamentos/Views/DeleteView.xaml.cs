using AppMauiDepartamentos.ViewModels;

namespace AppMauiDepartamentos.Views;

public partial class DeleteView : ContentPage
{
	public DeleteView(ActividadesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}