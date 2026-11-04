using System.Globalization;

namespace ProcessAutomationMAUI.Converter
{
    public class PickerSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value[0] == null) return null;
            if (value[1] is not IEnumerable<object> collection) return null;
            if (parameter == null || parameter is not string param) return null;

            return collection.AsQueryable().FirstOrDefault(x => x.GetType().GetProperty(param).GetValue(x).ToString() == value[0].ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (parameter == null || parameter is not string param) return null;

            return new object[2] { value.GetType().GetProperty(param).GetValue(value), null };
        }
    }
}
