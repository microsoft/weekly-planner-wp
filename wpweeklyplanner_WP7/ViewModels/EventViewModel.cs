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
using System.ComponentModel;
using System.Windows.Data;

namespace wpweeklyplanner
{
    /// <summary>
    /// Conversion class between starting time of an event and a string
    /// </summary>
    public class StartTimeConverter : IValueConverter
    {

        /// <summary>
        /// Converter from starting time to strings
        /// </summary>
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            EventViewModel model = value as EventViewModel;
            string str = wpweeklyplanner.AppResources.DetailsStarts;
            DateTime now = DateTime.Now;
            DateTime time = new DateTime( now.Year, now.Month, now.Day, model.StartHour, 0, 0 );
            return String.Format(str, time.ToShortTimeString());
        }

        /// <summary>
        /// Conversion back from string to starting time. Needed only for two-way binding
        /// which is not used, so no need to implement this
        /// </summary>
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Conversion class between ending time of an event and a string
    /// </summary>
    public class EndTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converter from ending time to strings
        /// </summary>
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {
            EventViewModel model = value as EventViewModel;
            string str = wpweeklyplanner.AppResources.DetailsEnds;
            DateTime now = DateTime.Now;
            DateTime time = new DateTime(now.Year, now.Month, now.Day, model.StartHour + model.Duration, 0, 0);
            return String.Format(str, time.ToShortTimeString());
        }

        /// <summary>
        /// Conversion back from string to ending time. Needed only for two-way binding
        /// which is not used, so no need to implement this
        /// </summary>
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Model class for an event
    /// </summary>
    public class EventViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Member variable for Text property
        /// </summary>
        private string _text;

        /// <summary>
        /// Property for summary text of the event
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Member variable for DayOfWeek property
        /// </summary>
        private int _dayOfWeek;

        /// <summary>
        /// Property for day of the week of the event (from 0=monday to 6=sunday)
        /// </summary>
        public int DayOfWeek
        {
            get
            {
                return _dayOfWeek;
            }
            set
            {
                if (value != _dayOfWeek)
                {
                    _dayOfWeek = value;
                    NotifyPropertyChanged("DayOfWeek");
                }
            }
        }
        
        /// <summary>
        /// Member variable for StartHour property
        /// </summary>
        private int _startHour;

        /// <summary>
        /// Property for starting hour of the event
        /// </summary>
        public int StartHour
        {
            get
            {
                return _startHour;
            }
            set
            {
                if (value != _startHour)
                {
                    _startHour = value;
                    NotifyPropertyChanged("StartHour");
                }
            }
        }

        /// <summary>
        /// Member variable for Duration property
        /// </summary>
        private int _duration;

        /// <summary>
        /// Property for duration of the event
        /// </summary>
        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    NotifyPropertyChanged("Duration");
                }
            }
        }

        /// <summary>
        /// Implementation of PropertyChanged event of INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Helper method for emitting PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}