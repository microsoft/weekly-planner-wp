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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;

namespace wpweeklyplanner
{
    /// <summary>
    /// Page for main view of the application
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Set up data context here so all children elements will be able to access it
            DataContext = App.ViewModel;

#if WP8 // Different background image based on aspect ratio
            if (App.Current.Host.Content.ScaleFactor == 150)
            {
                BgBrush.ImageSource = new BitmapImage(
                    new Uri(@"Content/background01_720p.png", UriKind.Relative)
                );
            }
#endif

            // We might need to restore the state PivotRoot after receiving its Loaded event. Since we
            // need to do this in tombstoning only the best place where to register the event handler
            // would be in OnNavigatedTo(). Unfortunately OnNavigatedTo() gets called after Loaded
            // event is emitted. Therefore we need to do the registration here.
            PivotRoot.Loaded += new RoutedEventHandler(PivotRoot_Loaded);
        }

        /// <summary>
        /// True when this object instance has been just created, otherwise false
        /// </summary>
        private bool isNewInstance = true;

        /// <summary>
        /// Member variable storing selected pivot index (in case of tombstoning)
        /// </summary>
        private int pivotIndex = -1;

        /// <summary>
        /// Member variable storing scrolling state in the selected pivot item (in case of tombstoning)
        /// </summary>
        private double verticalOffset = double.NaN;

        /// <summary>
        /// Overridden OnNavigatedTo handler
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (isNewInstance)
            {

                // Restoration PivotRoot is done in PivotRoot_Loaded instead of here, because OnNavigatedTo()
                // is called before pivot is loaded. Restoration of panorama states cannot be done here as
                // panorama items might not be ready before Loaded event. Also scroller state cannot be restored
                // before Loaded event. Take the values out of States and save them to member variable because State
                // is guaranteed to be accessible only during OnNavigatedTo() and OnNavigatedFrom().

                if (State.ContainsKey("PivotIndex"))
                {
                    pivotIndex = (int)State["PivotIndex"];
                }
                else
                {
                    pivotIndex = -1;
                }

                if (State.ContainsKey("VerticalOffset"))
                {
                    verticalOffset = (double)State["VerticalOffset"];
                }
                else
                {
                    verticalOffset = double.NaN;
                }

                isNewInstance = false;

            }
        }

        /// <summary>
        /// Overridden OnNavigatedFrom handler
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode != NavigationMode.Back)
            {

                // Save transient page state when moving away
                State["PivotIndex"] = PivotRoot.SelectedIndex;

                ScrollViewer scroller = (PivotRoot.SelectedItem as PivotItem).Content as ScrollViewer;
                State["VerticalOffset"] = scroller.VerticalOffset;
            }
        }

        /// <summary>
        /// Handler invoked when pivot control has finished loading
        /// </summary>
        void PivotRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // If we have state information related to tombstoning, restore the state of Pivot
            if (pivotIndex != -1)
            {
                PivotRoot.SelectedIndex = pivotIndex;
                pivotIndex = -1;

                // Since we have restored the state now, unregister the event handler as we don't need it anymore
                PivotRoot.Loaded -= new RoutedEventHandler(PivotRoot_Loaded);

                // Scroller state also needs to be restored now that we know in which pivot item we're in. Register
                // event handler for its Loaded event
                ScrollViewer scroller = (PivotRoot.SelectedItem as PivotItem).Content as ScrollViewer;
                scroller.Loaded += new RoutedEventHandler(Scroller_Loaded);

            }
        }

        /// <summary>
        /// Handler invoked when the ScrollViewer of the selected pivot item has finished loading
        /// </summary>
        void Scroller_Loaded(object sender, RoutedEventArgs e)
        {
            // If we have state information related to tombstoning, restore the state of ScrollViewer of the 
            // current PivotItem
            if ( !double.IsNaN(verticalOffset))
            {
                ScrollViewer scroller = (PivotRoot.SelectedItem as PivotItem).Content as ScrollViewer;
                scroller.ScrollToVerticalOffset(verticalOffset);
                verticalOffset = double.NaN;

                // Since we have restored the state now, unregister the event handler as we don't need it anymore
                scroller.Loaded -= new RoutedEventHandler(Scroller_Loaded);
            }
        }

        /// <summary>
        /// Handler invoked when user taps an empty time slot (to create a new event)
        /// </summary>
        private void AddEvent(object sender, RoutedEventArgs e)
        {
            AddEventArgs args = e as AddEventArgs;
            NavigationService.Navigate(new Uri("/EditPage.xaml?dayOfWeek=" + args.DayOfWeek + "&startHour=" + args.StartHour, UriKind.Relative));
        }

        /// <summary>
        /// Handler invoked when user taps an existing event (to view its details)
        /// </summary>
        private void SelectEvent(object sender, RoutedEventArgs e)
        {
            SelectEventArgs args = e as SelectEventArgs;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?eventIndex=" + App.ViewModel.Items.IndexOf(args.ViewModel), UriKind.Relative));
        }

        /// <summary>
        /// Handler invoked when user wishes to edit an existing event (via context menu)
        /// </summary>
        private void EditEvent(object sender, RoutedEventArgs e)
        {
            SelectEventArgs args = e as SelectEventArgs;
            NavigationService.Navigate(new Uri("/EditPage.xaml?eventIndex=" + App.ViewModel.Items.IndexOf(args.ViewModel), UriKind.Relative));
        }

        /// <summary>
        /// Handler invoked when user wishes to remove an event (via context menu)
        /// </summary>
        private void RemoveEvent(object sender, RoutedEventArgs e)
        {
            SelectEventArgs args = e as SelectEventArgs;
            int index = App.ViewModel.Items.IndexOf(args.ViewModel);

            MessageBoxResult result = MessageBox.Show(wpweeklyplanner.AppResources.RemoveMessage,
                                           wpweeklyplanner.AppResources.RemoveTitle,
                                           MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                App.ViewModel.Items.RemoveAt(index);
            }
        }

    }

}