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
using System.Windows.Media.Imaging;

namespace wpweeklyplanner
{
    /// <summary>
    /// Control that represents an event in the weekly grid
    /// </summary>
    [TemplatePart(Name="EventGfxTop", Type=typeof(Image))]
    [TemplatePart(Name="EventGfxMiddle", Type=typeof(Image))]
    [TemplatePart(Name="EventGfxBottom", Type=typeof(Image))]
    public class WeeklyEvent : Button
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WeeklyEvent()
        {
            DefaultStyleKey = typeof(WeeklyEvent);
        }

        /// <summary>
        /// Convinience property for accessing the model that feeds data to this control
        /// </summary>
        public EventViewModel ViewModel { get; set; }

        /// <summary>
        /// Property describing base name of the graphics to be used.
        /// </summary>
        public string GfxName { get; set; }

        /// <summary>
        /// Overridden OnApplyTemplate handler
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Image topImage = GetTemplateChild("EventGfxTop") as Image;
            if (topImage != null)
            {
                string image = "/Content/event_{0}_top.png";
                topImage.Source = new BitmapImage(new Uri(String.Format(image, GfxName), UriKind.Relative));
            }

            Image middleImage = GetTemplateChild("EventGfxMiddle") as Image;
            if (middleImage != null)
            {
                string image = "/Content/event_{0}_middle.png";
                middleImage.Source = new BitmapImage(new Uri(String.Format(image, GfxName), UriKind.Relative));
            }

            Image bottomImage = GetTemplateChild("EventGfxBottom") as Image;
            if (bottomImage != null)
            {
                string image = "/Content/event_{0}_bottom.png";
                bottomImage.Source = new BitmapImage(new Uri(String.Format(image, GfxName), UriKind.Relative));
            }

        }

    }
}