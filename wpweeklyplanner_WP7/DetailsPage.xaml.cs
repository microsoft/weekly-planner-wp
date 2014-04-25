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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wpweeklyplanner
{
    /// <summary>
    /// Page for showing details about an event
    /// </summary>
    public partial class DetailsPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DetailsPage()
        {
            InitializeComponent();

            // Application bar is not a silverlight component -> it doesn't support data binding, which means
            // that it cannot be localized with XAML. Therefore, create it using C#
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // Use opacity slight < 1.0 so that application bar is overlaid on the background instead of background being resized
            ApplicationBar.Opacity = 0.9;

            ApplicationBarIconButton editButton = new ApplicationBarIconButton(new Uri("/Content/appbar.edit.rest.png", UriKind.Relative));
            editButton.Text = wpweeklyplanner.AppResources.ButtonEdit;
            editButton.IsEnabled = true;
            editButton.Click += new EventHandler(Button1_click);

            ApplicationBarIconButton removeButton = new ApplicationBarIconButton(new Uri("/Content/appbar.delete.rest.png", UriKind.Relative));
            removeButton.Text = wpweeklyplanner.AppResources.ButtonRemove;
            removeButton.IsEnabled = true;
            removeButton.Click += new EventHandler(Button2_click);

            ApplicationBar.Buttons.Add(editButton);
            ApplicationBar.Buttons.Add(removeButton);

        }

        /// <summary>
        /// Overridden OnNavigatedTo handler
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (isNewInstance)
            {
                // Make sure that data context is loaded
                if (!App.ViewModel.IsDataLoaded)
                {
                    App.ViewModel.LoadData();
                }

                eventIndex = int.Parse(NavigationContext.QueryString["eventIndex"]);

                isNewInstance = false;
            }

            // Refresh data context properly by setting it to NULL first. Otherwise changes made to the event
            // in Edit page will not get immediately reflected to Details page
            DataContext = null;
            DataContext = App.ViewModel.Items[eventIndex];

        }

        /// <summary>
        /// Handler for Edit button
        /// </summary>
        private void Button1_click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditPage.xaml?eventIndex=" + eventIndex, UriKind.Relative));
        }

        /// <summary>
        /// Handler for Remove button
        /// </summary>
        private void Button2_click(object sender, EventArgs e)
        {
            
            MessageBoxResult result = MessageBox.Show(wpweeklyplanner.AppResources.RemoveMessage,
                                                       wpweeklyplanner.AppResources.RemoveTitle,
                                                       MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                App.ViewModel.Items.RemoveAt(eventIndex);
                NavigationService.GoBack();
            }

        }

        /// <summary>
        /// True when this object instance has been just created, otherwise false
        /// </summary>
        private bool isNewInstance = true;

        /// <summary>
        /// Member variable for storing the index of the event
        /// </summary>
        private int eventIndex;
    }
}