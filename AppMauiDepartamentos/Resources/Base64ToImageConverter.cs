using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Resources
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (((byte[])value).Count() == 0)
            {
                var imageDefault = ImageSource.FromResource("AppMauiDepartamentos.Resources.Images.default_image.png");
                return imageDefault;
            }
            var streamSource = ImageSource.FromStream(() => new MemoryStream((byte[])value));
            return streamSource;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
