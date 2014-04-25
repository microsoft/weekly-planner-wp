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
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace wpweeklyplanner
{
    /// <summary>
    /// Page for editing an existing event
    /// </summary>
    public partial class EditPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EditPage()
        {
            InitializeComponent();

#if WP8 // Different background image based on aspect ratio
            if (App.Current.Host.Content.ScaleFactor == 150)
            {
                BgBrush.ImageSource = new BitmapImage(
                    new Uri(@"Content/background02_720p.png", UriKind.Relative)
                );
            }
#endif

            // Application bar is not a silverlight component -> it doesn't support data binding, which means
            // that it cannot be localized with XAML. Therefore, create it using C#
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // Use opacity slight < 1.0 so that application bar is overlaid on the background instead of background being resized
            ApplicationBar.Opacity = 0.9;

            doneButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative));
            doneButton.Text = wpweeklyplanner.AppResources.ButtonDone;
            doneButton.Click += new EventHandler(DoneButton_click);

            ApplicationBarIconButton cancelButton = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative));
            cancelButton.Text = wpweeklyplanner.AppResources.ButtonCancel;
            cancelButton.Click += new EventHandler(CancelButton_click);

            ApplicationBar.Buttons.Add(doneButton);
            ApplicationBar.Buttons.Add(cancelButton);
        }

        /// <summary>
        /// True when this object instance has been just created, otherwise false
        /// </summary>
        private bool isNewInstance = true;

        /// <summary>
        /// Member variable for Done button
        /// </summary>
        private ApplicationBarIconButton doneButton;


        /// <summary>
        /// Member variable storing the index of the event being modified
        /// </summary>
        private int eventIndex = -1;

        /// <summary>
        /// Member variable storing the day of the event to be created
        /// </summary>
        private int dayOfWeek = -1;

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

                // Restore page state. Some of the state of the page will be static in type, like
                // event index or day of week. These can be restored from navigation context. Rest of
                // the state are dynamic, and need to be restored from state object. Notice especially
                // starting hour and ending hour: original values for these come from navigation context
                // but user can edit them further in the view. Therefore we first check if they can be
                // restored from state object; if not, we restore them using original values from
                // navigation context

                if (NavigationContext.QueryString.ContainsKey("eventIndex"))
                {
                    eventIndex = int.Parse(NavigationContext.QueryString["eventIndex"]);
                }
                else
                {
                    dayOfWeek = int.Parse(NavigationContext.QueryString["dayOfWeek"]);
                }

                DateTime time = DateTime.Now;
                int startHour = -1;
                int endHour = -1;

                if (State.ContainsKey("StartHour"))
                {
                    StartsPicker.Value = (DateTime?)State["StartHour"];
                }
                else
                {

                    if (eventIndex != -1)
                    {
                        startHour = App.ViewModel.Items[eventIndex].StartHour;
                    }
                    else
                    {
                        startHour = int.Parse(NavigationContext.QueryString["startHour"]);
                    }

                    StartsPicker.Value = new DateTime(time.Year, time.Month, time.Day, startHour, 0, 0);
                }

                if (State.ContainsKey("EndHour"))
                {
                    EndsPicker.Value = (DateTime?)State["EndHour"];
                }
                else
                {
                    if (eventIndex != -1)
                    {
                        endHour = startHour + App.ViewModel.Items[eventIndex].Duration;
                    }
                    else
                    {
                        endHour = startHour + 1;
                    }

                    EndsPicker.Value = new DateTime(time.Year, time.Month, time.Day, endHour, 0, 0);
                }

                if (State.ContainsKey("Description"))
                {
                    DescriptionInput.Text = (string)State["Description"];
                }
                else
                {
                    if (eventIndex != -1)
                    {
                        DescriptionInput.Text = App.ViewModel.Items[eventIndex].Text;
                    }
                    else
                    {
                        DescriptionInput.Text = "";
                    }
                }

                isNewInstance= false;
            }

            // Register validators for validators. Don't do this in XAML because then times would get validated upon
            // setting their default values (which would complicate things).

            // DateTimeValueChangedEventArgs is part of Silverlight Toolkit which does not ship as part of this
            // example. Please see release notes for instructions how to install and use Silverlight Toolkit.
            StartsPicker.ValueChanged -= new EventHandler<DateTimeValueChangedEventArgs>(ValidateTime);
            StartsPicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(ValidateTime);
            EndsPicker.ValueChanged -= new EventHandler<DateTimeValueChangedEventArgs>(ValidateTime);
            EndsPicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(ValidateTime);

            UpdateStates();
        }

        /// <summary>
        /// Overridden OnNavigatedFrom handler
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode != NavigationMode.Back)
            {
                State["Description"] = DescriptionInput.Text;

                // When navigating to TimePicker OnNavigatedFrom() gets called. We should _not_ be saving the states
                // of TimePicker in that case, because we would end up overwriting the value(s) selected by the user
                // in OnNavigatedTo().

                // TimePicker is part of Silverlight Toolkit which does not ship as part of this
                // example. Please see release notes for instructions how to install and use Silverlight Toolkit.
                string uri = e.Uri.ToString();
                if (uri != StartsPicker.PickerPageUri.ToString() )
                {
                    State["StartHour"] = StartsPicker.Value;
                    State["EndHour"] = EndsPicker.Value;
                }
                else
                {
                    State.Remove("StartHour");
                    State.Remove("EndHour");
                }
            }

        }

        /// <summary>
        /// Helper method for updating appbar button states
        /// </summary>
        private void UpdateStates()
        {
            bool description = (DescriptionInput.Text != "");

            doneButton.IsEnabled = description;
        }

        /// <summary>
        /// Method to validate starting and ending time entered by the user
        /// </summary>
        private void ValidateTime(object sender, DateTimeValueChangedEventArgs e)
        {
            const int StartTimeCap = 6;
            const int EndTimeCap = 19;

            TimePicker picker = sender as TimePicker;
            DateTime time = e.NewDateTime ?? DateTime.Now;
            int newTime = time.Hour;

            if (picker == EndsPicker)
            {
                DateTime startTime = StartsPicker.Value ?? DateTime.Now;
                if (newTime <= startTime.Hour)
                {
                    newTime = startTime.Hour + 1;
                }
                if (time.Hour < StartTimeCap)
                {
                    newTime = StartTimeCap;
                }
                if (time.Hour > EndTimeCap + 1)
                {
                    newTime = EndTimeCap + 1;
                }
            }
            else
            {
                DateTime endTime = EndsPicker.Value ?? DateTime.Now;
                if (newTime >= endTime.Hour)
                {
                    newTime = endTime.Hour - 1;
                }
                if (time.Hour < StartTimeCap)
                {
                    newTime = StartTimeCap;
                }
                if (time.Hour > EndTimeCap)
                {
                    newTime = EndTimeCap;
                }

            }

            picker.Value = new DateTime(time.Year, time.Month, time.Day, newTime, 0, 0);

            UpdateStates();
        }

        /// <summary>
        /// Method to validate event summary entered by the user
        /// </summary>
        private void ValidateText(object sender, TextChangedEventArgs e)
        {
            UpdateStates();
        }

        /// <summary>
        /// Handler for Done button
        /// </summary>
        private void DoneButton_click(object sender, EventArgs e)
        {
            EventViewModel model = null;

            if (eventIndex != -1)
            {
                model = App.ViewModel.Items[eventIndex];
            }
            else
            {
                model = new EventViewModel();
                model.DayOfWeek = dayOfWeek;
            }

            DateTime startHour = StartsPicker.Value ?? DateTime.Now;
            DateTime endHour = EndsPicker.Value ?? DateTime.Now;
            model.StartHour = startHour.Hour;
            model.Duration = endHour.Hour - startHour.Hour;
            model.Text = DescriptionInput.Text;

            if (eventIndex == -1)
            {
                App.ViewModel.Items.Add(model);
            }

            NavigationService.GoBack();
        }

        /// <summary>
        /// Handler for Cancel button
        /// </summary>
        private void CancelButton_click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }

}