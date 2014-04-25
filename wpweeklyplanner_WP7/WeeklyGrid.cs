/**
 * Copyright (c) 2011-2014 Microsoft Mobile. All rights reserved.
 *
 * Nokia, Nokia Connecting People, Nokia Developer, and HERE are trademarks
 * and/or registered trademarks of Nokia Corporation. Other product and company
 * names mentioned herein may be trademarks or trade names of their respective
 * owners.
 *
 * See the license text file delivered with this project for more information.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace wpweeklyplanner
{
    // WeeklyGrid is a custom control for showing the view of a single day, including
    // "hour slots" and events. Custom control is used here because especially with events
    // the rendering can get quite complex since events can overlap each other. With a custom
    // control also the handling of operations possible for events is easier since we can
    // declare separate events for each operation which can then be executed elsewhere.
    public class WeeklyGrid : Control
    {
        public event RoutedEventHandler AddEvent;
        public event RoutedEventHandler SelectEvent;
        public event RoutedEventHandler EditEvent;
        public event RoutedEventHandler RemoveEvent;

        public WeeklyGrid()
        {
            DefaultStyleKey = typeof(WeeklyGrid);

            Loaded += new RoutedEventHandler(WeeklyGrid_Loaded);
            Unloaded += new RoutedEventHandler(WeeklyGrid_Unloaded);
        }

        private void WeeklyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if we have a proper data context set
            MainViewModel model = DataContext as MainViewModel;
            if (model != null)
            {
                ViewModel = model;
            }

            // Call Update() to set up view based on events
            Update();
        }

        private void WeeklyGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clear the data context so we don't receive view model updates while we are not visible
            ViewModel = null;
        }

        private MainViewModel viewModel;
        public MainViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                if (value != viewModel)
                {
                    if( viewModel != null )
                    {
                        viewModel.Items.CollectionChanged -= new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
                    }
                    viewModel = value;
                    if (viewModel != null)
                    {
                        viewModel.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
                    }

                }
            }
        }

        public static readonly DependencyProperty DayOfWeekProperty =
            DependencyProperty.Register("DayOfWeek", typeof(int), typeof(WeeklyGrid), null);

        public int DayOfWeek
        {
            get
            {
                return (int)GetValue(DayOfWeekProperty);
            }
            set
            {
                SetValue(DayOfWeekProperty, value);
            }
        }

        public static readonly DependencyProperty StartHourProperty =
            DependencyProperty.Register("StartHour", typeof(int), typeof(WeeklyGrid), null);

        public int StartHour
        {
            get
            {
                return (int)GetValue(StartHourProperty);
            }
            set
            {
                SetValue(StartHourProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            grid = (Grid)GetTemplateChild("LayoutRoot");

            // Subscribe event handlers to all row items
            foreach (UIElement child in grid.Children)
            {
                Button item = child as Button;
                if (item != null)
                {
                    item.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(Item_Click);
                    item.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(Item_Click);
                }
            }

        }

        public void Update()
        {
            if (grid == null)
            {
                return;
            }
            
            // Remove all events from the grid
            IList<UIElement> removables = grid.Children.OfType<WeeklyEvent>().Cast<UIElement>().ToList();
            foreach (UIElement removable in removables)
            {
                grid.Children.Remove(removable);
            }

            // If view model is not set we don't have any events to show
            if (ViewModel == null || ViewModel.Items == null)
            {
                return;
            }

            // Discover all events for this day
            List<EventViewModel> events = new List<EventViewModel>();
            foreach (EventViewModel model in ViewModel.Items)
            {
                if (model.DayOfWeek == DayOfWeek)
                {
                    events.Add(model);
                }
            }

            // Sort the events (required to draw overlapping events more easily)
            events.Sort(EventsCompare);

            // Generate matching UI controls for event (with recursion for overlapping events)
            gfxNameCounter = 0;
            while (events.Count > 0)
            {
                List<EventViewModel> temp = new List<EventViewModel>(events);
                int maximumLevel = DiscoverMaxLevel(temp, 0);
                GenerateEventControl(events, 0, maximumLevel);
            }

        }

        private static int EventsCompare(EventViewModel x, EventViewModel y)
        {
            if (x.StartHour > y.StartHour)
            {
                return 1;
            }
            else if (x.StartHour < y.StartHour)
            {
                return -1;
            }
            else
            {
                if (x.Duration > y.Duration)
                {
                    return -1;
                }
                else if (x.Duration < y.Duration)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void GenerateEventControl(List<EventViewModel> events, int level, int maxLevel)
        {
            EventViewModel firstEvent = events[0];
            events.RemoveAt(0);

            bool hasOverlaps = false;
            while (events.Count > 0 && EventsOverlap(firstEvent, events[0]))
            {
                hasOverlaps = true;
                GenerateEventControl(events, level + 1, maxLevel);
            }

            WeeklyEvent control = new WeeklyEvent();
            control.Style = Application.Current.Resources["custom:WeeklyEvent"] as Style;
            control.Content = firstEvent.Text;
            Grid.SetRow(control, firstEvent.StartHour - StartHour);
            Grid.SetColumn(control, 1);
            Grid.SetRowSpan(control, firstEvent.Duration);
            double availableWidth = grid.ColumnDefinitions[1].ActualWidth;
            double widthPerEvent = availableWidth / (maxLevel + 1);
            control.Margin = new Thickness(level * widthPerEvent, 0, 0, 0);
            control.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            control.ViewModel = firstEvent;
            if (gfxNameCounter % 2 == 0)
            {
                control.GfxName = "green";
            }
            else
            {
                control.GfxName = "turquoise";
            }
            ++gfxNameCounter;

            ContextMenu menu = new ContextMenu();
            menu.IsZoomEnabled = false;
            MenuItem edit = new MenuItem();
            edit.Header = wpweeklyplanner.AppResources.ButtonEdit;
            edit.Click += new RoutedEventHandler(Event_Edit);
            edit.DataContext = control;
            MenuItem remove = new MenuItem();
            remove.Header = wpweeklyplanner.AppResources.ButtonRemove;
            remove.Click += new RoutedEventHandler(Event_Remove);
            remove.DataContext = control;

            menu.Items.Add(edit);
            menu.Items.Add(remove);
            ContextMenuService.SetContextMenu(control, menu);

            if (hasOverlaps)
            {
                control.Width = widthPerEvent;
            }
            else
            {
                control.Width = availableWidth - level * widthPerEvent;
            }

            control.Click += new RoutedEventHandler(Event_Click);
            grid.Children.Add(control);

        }

        private int DiscoverMaxLevel(List<EventViewModel> events, int level)
        {
            EventViewModel firstEvent = events[0];
            events.RemoveAt(0);

            int maximumLevel = level;
            while (events.Count > 0 && EventsOverlap(firstEvent, events[0]))
            {
                maximumLevel = Math.Max(maximumLevel, DiscoverMaxLevel(events, level + 1));
            }

            return maximumLevel;
            
        }

        private bool EventsOverlap(EventViewModel firstEvent, EventViewModel secondEvent)
        {
            // Events are sorted, so this function presumes that starting hour of firstEvent <= secondEvent
            return (firstEvent.StartHour + firstEvent.Duration > secondEvent.StartHour);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        private void Event_Click(object sender, RoutedEventArgs e)
        {
            WeeklyEvent ev = sender as WeeklyEvent;

            if (SelectEvent != null)
            {
                SelectEvent(this, new SelectEventArgs(ev.ViewModel));
            }
        }

        private void Event_Edit(object sender, RoutedEventArgs e)
        {
            WeeklyEvent ev = (sender as MenuItem).DataContext as WeeklyEvent;

            if (EditEvent != null)
            {
                EditEvent(this, new SelectEventArgs(ev.ViewModel));
            }
        }

        private void Event_Remove(object sender, RoutedEventArgs e)
        {
            WeeklyEvent ev = (sender as MenuItem).DataContext as WeeklyEvent;

            if (RemoveEvent != null)
            {
                RemoveEvent(this, new SelectEventArgs(ev.ViewModel));
            }
        }

        private void Item_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button control = sender as Button;
            int row = Grid.GetRow(control);
            int startHour = StartHour + row;

            if (AddEvent != null)
            {
                AddEvent( this, new AddEventArgs( DayOfWeek, startHour ) );
            }

        }

        private Grid grid;
        private int gfxNameCounter;
    }

    public class AddEventArgs : RoutedEventArgs
    {
        public AddEventArgs(int dayOfWeek, int startHour)
        {
            this.dayOfWeek = dayOfWeek;
            this.startHour = startHour;
        }

        private int dayOfWeek;
        public int DayOfWeek { get { return dayOfWeek; } }

        private int startHour;
        public int StartHour { get { return startHour; } }
    }

    public class SelectEventArgs : RoutedEventArgs
    {
        public SelectEventArgs(EventViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private EventViewModel viewModel;
        public EventViewModel ViewModel { get { return viewModel; } }
    }
}
