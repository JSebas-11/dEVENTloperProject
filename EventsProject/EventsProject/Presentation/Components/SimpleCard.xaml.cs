using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.Components;

public partial class SimpleCard : UserControl {
    public SimpleCard() {
        InitializeComponent();
    }

    //------------------------PROPERTIES-----------------------
    // Título de la tarjeta
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SimpleCard), new PropertyMetadata("Default Title"));

    public string Title {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // Valor (numero, porcentaje, etc.)
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(SimpleCard), new PropertyMetadata("0"));

    public string Value {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    // Icono FontAwesome
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(FontAwesomeIcon), typeof(SimpleCard), new PropertyMetadata(FontAwesomeIcon.InfoCircle));

    public FontAwesomeIcon Icon {
        get => (FontAwesomeIcon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}

