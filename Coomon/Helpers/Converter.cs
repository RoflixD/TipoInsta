using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coomon.Helpers
{
    public static class Converter
    {
        public static T? ConvertFromString<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null && converter.ConvertFromString(input) is T res)
                {
                    return res;
                }
                return default;
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }
    }
}
