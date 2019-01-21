using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OidcClientDemo.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for TokenViewer.xaml
    /// </summary>
    public partial class TokenViewer : UserControl
    {
        public TokenViewer()
        {
            InitializeComponent();
        }

        public string IdToken
        {
            get { return (string)GetValue(IdTokenProperty); }
            set { SetValue(IdTokenProperty, value); }
        }
        public static readonly DependencyProperty IdTokenProperty =
            DependencyProperty.Register(nameof(IdToken), typeof(string), typeof(TokenViewer), new PropertyMetadata(null));

        public string AccessToken
        {
            get { return (string)GetValue(AccessTokenProperty); }
            set { SetValue(AccessTokenProperty, value); }
        }
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register(nameof(AccessToken), typeof(string), typeof(TokenViewer), new PropertyMetadata(null));

        public int ExpiresIn
        {
            get { return (int)GetValue(ExpiresInProperty); }
            set { SetValue(ExpiresInProperty, value); }
        }
        public static readonly DependencyProperty ExpiresInProperty =
            DependencyProperty.Register(nameof(ExpiresIn), typeof(int), typeof(TokenViewer), new PropertyMetadata(0));

        public string TokenType
        {
            get { return (string)GetValue(TokenTypeProperty); }
            set { SetValue(TokenTypeProperty, value); }
        }
        public static readonly DependencyProperty TokenTypeProperty =
            DependencyProperty.Register(nameof(TokenType), typeof(string), typeof(TokenViewer), new PropertyMetadata(null));

        public Thickness GridThickness
        {
            get { return (Thickness)GetValue(GridThicknessProperty); }
            set { SetValue(GridThicknessProperty, value); }
        }
        public static readonly DependencyProperty GridThicknessProperty =
            DependencyProperty.Register(nameof(GridThickness), typeof(Thickness), typeof(TokenViewer), new PropertyMetadata(new Thickness(1)));

        public Brush GridBrush
        {
            get { return (Brush)GetValue(GridBrushProperty); }
            set { SetValue(GridBrushProperty, value); }
        }
        public static readonly DependencyProperty GridBrushProperty =
            DependencyProperty.Register(nameof(GridBrush), typeof(Brush), typeof(TokenViewer), new PropertyMetadata(default(SolidColorBrush)));
    }
}
