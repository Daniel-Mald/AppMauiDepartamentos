using AppMauiDepartamentos.Services;
using AppMauiDepartamentos.ViewModels;
using System.IO;

namespace AppMauiDepartamentos.Views;

public partial class UpdateView : ContentPage
{
	public UpdateView(ActividadesViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        SetImage(vm.Img);
        
        
	}
    public void SetImage(string img)
    {
        //aqui quiero convertir la de base64 a imagen y ponerla en el selectedImage
        //Image64 tiene el texto en base64
        //byte[] imageBytes = Convert.FromBase64String(img);
        //string outputPath = "imagen.jpg";

        //File.WriteAllBytes(outputPath, imageBytes);


        //SelectedImage.Source = outputPath;
        try
        {

            //byte[] imageBytes = Convert.FromBase64String(img);

            //using (var ms = new MemoryStream(imageBytes))
            //{
            //    SelectedImage.Source = ImageSource.FromStream(() => ms);
            //}
            byte[] imageBytes = Convert.FromBase64String(img);

            // Crea un stream a partir del array de bytes
            MemoryStream ms = new MemoryStream(imageBytes);

            // Asigna la imagen al control
            SelectedImage.Source = ImageSource.FromStream(() =>
            {
                MemoryStream copy = new MemoryStream(imageBytes);
                return copy;
            });
            }
        catch (Exception ex)
        {

        }

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
}