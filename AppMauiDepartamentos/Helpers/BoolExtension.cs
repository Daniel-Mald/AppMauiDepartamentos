using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMauiDepartamentos.Helpers
{
    [ContentProperty(nameof(Value))]

    public class BoolExtension : IMarkupExtension
    {
        public string Value { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if(bool.TryParse(Value, out bool result))
            {
                return result;
            }
            throw new InvalidOperationException($"Cannot convert {Value} to boolean");
        }
    }
}
