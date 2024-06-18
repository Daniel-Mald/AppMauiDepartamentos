
using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.ViewModels;

namespace AppMauiDepartamentos.Views;

public partial class AddView : ContentPage
{
    public AddView(ActividadesViewModel vs)
	{
		InitializeComponent();
        BindingContext = vs;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una imagen"
            });

            if (result != null)
            {
                
                var stream = await result.OpenReadAsync();
                SelectedImage.Source = ImageSource.FromStream(() => stream);

                
                string base64Image = await ConvertImageToBase64(result);
                Imagen64.Text = base64Image;
            }
        }
        catch (Exception ex)
        {
            
        }
    }
    private async Task<string> ConvertImageToBase64(FileResult file)
    {
        using (var stream = await file.OpenReadAsync())
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
    }

    private async Task Button_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Selecciona una imagen"
            });

            if (result != null)
            {

                var stream = await result.OpenReadAsync();
                SelectedImage.Source = ImageSource.FromStream(() => stream);


                string base64Image = await ConvertImageToBase64(result);
                Imagen64.Text = base64Image;
            }
        }
        catch (Exception ex)
        {

        }
    }
}