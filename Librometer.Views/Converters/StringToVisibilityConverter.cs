﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections;

namespace Librometer.Views.Converters
{
    /*RGE [ValueConversion(typeof(string), typeof(Visibility))]*/
    public class StringToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            var list = (IList)value;
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Reverse value computing
            /*RGE inutile dans mon cas
            return ((value is Visibility)
                        && (((Visibility)value) == Visibility.Visible));*/
            return null;
        }
    }
}
