using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit;
using Coding4Fun.Toolkit.Controls;

namespace NanjingBus.Method
{
    public class MessageHelper
    {
        public static void Show(string str)
        {
            var toast = new ToastPrompt
            {
                Message = str,
                FontSize = 20,
                //ImageSource = new BitmapImage(new Uri("/Resources/Images/messIcon.png", UriKind.Relative))
            };
            toast.Show();
        }

    }
}
