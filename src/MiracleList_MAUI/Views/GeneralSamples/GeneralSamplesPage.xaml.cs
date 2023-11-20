namespace MiracleList_MAUI.Views.GeneralSamples;

public partial class GeneralSamplesPage : ContentPage
{
	public GeneralSamplesPage()
	{
		InitializeComponent();
	}

 private void OnPointerClicked(object sender, EventArgs e)
 {
		Shell.Current.GoToAsync(nameof(PointerDemosPage));
 }

 private void OnDragDropClicked(object sender, EventArgs e)
 {
  Shell.Current.GoToAsync(nameof(DragDropDemoPage));
 }
}