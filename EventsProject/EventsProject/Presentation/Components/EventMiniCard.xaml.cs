using EventsProject.Presentation.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.Components;

public partial class EventMiniCard : UserControl {
    public EventMiniCard() {
        InitializeComponent();
    }

    //------------------------DEPENDENCY PROPERTY-----------------------
    public static readonly DependencyProperty EventProperty =
        DependencyProperty.Register(nameof(Event), typeof(EventDTO), typeof(EventMiniCard), new PropertyMetadata(null));

    public EventDTO Event {
        get => (EventDTO)GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }
}
