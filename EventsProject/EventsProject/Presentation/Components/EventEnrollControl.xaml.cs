using EventsProject.Presentation.DTOs;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.Components;

public partial class EventEnrollControl : UserControl {
    public EventEnrollControl() {
        InitializeComponent();
    }

    //------------------------DEPENDENCY PROPERTY-----------------------
    public static readonly DependencyProperty EventProperty =
        DependencyProperty.Register(nameof(Event), typeof(EventDTO), typeof(EventEnrollControl), new PropertyMetadata(null));

    public EventDTO Event {
        get => (EventDTO)GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }

    //------------------------BUTTONS-----------------------
    public event EventHandler<EventDTO>? EventClicked;
    
    private void btnEnrroll_Click(object sender, RoutedEventArgs e) {
        EventClicked?.Invoke(this, Event);
    }
}
