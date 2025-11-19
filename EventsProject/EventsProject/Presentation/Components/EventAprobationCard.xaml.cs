using EventsProject.Domain.Models;
using System.Windows;
using System.Windows.Controls;

namespace EventsProject.Presentation.Components;

public partial class EventAprobation : UserControl {
    public EventAprobation() {
        InitializeComponent();
    }

    //------------------------DEPENDENCY PROPERTY-----------------------
    public static readonly DependencyProperty EventProperty =
        DependencyProperty.Register(nameof(Event), typeof(EventInfo), typeof(EventAprobation), new PropertyMetadata(null));

    public EventInfo Event {
        get => (EventInfo)GetValue(EventProperty);
        set => SetValue(EventProperty, value);
    }

    //------------------------BUTTONS-----------------------
    public event EventHandler<EventInfo>? ApproveClicked;
    public event EventHandler<EventInfo>? DenyClicked;

    private void btnApprove_Click(object sender, RoutedEventArgs e) {
        ApproveClicked?.Invoke(this, Event);
    }
    private void btnDeny_Click(object sender, RoutedEventArgs e) {
        DenyClicked?.Invoke(this, Event);
    }
}
