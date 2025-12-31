using SilksongSaveSync.ViewModels;

namespace SilksongSaveSync.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

