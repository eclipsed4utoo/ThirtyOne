using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for MyImageButton.xaml
    /// </summary>
    public partial class MyImageButton : Button
    {
        public static readonly DependencyProperty ImageProperty =
                DependencyProperty.Register("ImageSource", typeof(Image), typeof(MyImageButton));

        public Image ImageSource
        {
            get { return (Image)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public MyImageButton()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                this.ImageSource = ImageSource;
            }
        }
    }
}
