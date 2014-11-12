Weekly Planner
==============

This example application demonstrates how to build custom user interface controls for Windows Phone using Microsoft Silverlight.

![App design](/doc/WP7_weekly_planner.png?raw=true)

This example application is hosted in GitHub:
https://github.com/Microsoft/weekly-planner-wp

Developed with:
 * Microsoft Visual Studio 2010 Express for Windows Phone
 * Microsoft Visual Studio Express for Windows Phone 2012.

Compatible with:

 * Windows Phone 7
 * Windows Phone 8

Tested to work on:

 * Nokia Lumia 820 
 * Nokia Lumia 920


Instructions
------------

Make sure you have the following installed:

 * Windows 8
 * Windows Phone SDK 8.0
 * Latest NuGet Package Manager (>2.7.1) from https://nuget.org/ to enable NuGet Package Restore

To build and run the sample:

 * Open the SLN file
   * File > Open Project, select the file wpweeklyplanner.sln
 * Depending on whether you want to run the WP7 or WP8 version of the application, select either wprestaurantapp_WP7 or wprestaurantapp_WP8 as a StartUp Project.    
 * Select the target, for example 'Emulator WVGA'.
 * Press F5 to build the project and run it on the Windows Phone Emulator.

To deploy the sample on Windows Phone device:
 * See the official documentation for deploying and testing applications on Windows Phone devices at http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff402565(v=vs.105).aspx
 

About the implementation
------------------------

Important folders:

| Folder | Description |
| ------ | ----------- |
| / | Contains the project file, the license information and this file (README.md) |
| wpweeklyplanner_WP7 | Root folder for the WP7 implementation files. |
| wpweeklyplanner_WP8 | Root folder for the WP8 implementation files. |
| wpweeklyplanner_WP7/Content | Graphic files. |
| wpweeklyplanner_WP7/Properties | WP7 Application property files. |
| wpweeklyplanner_WP7/SampleData | Application sample data. |
| wpweeklyplanner_WP7/Themes | Application specific theming. |
| wpweeklyplanner_WP7/ViewModels | ViewModels of the application. |
| wpweeklyplanner_WP8/Content | Graphic files. |
| wpweeklyplanner_WP8/Properties | WP8 Application property files. |

Important files:

| File | Description |
| ---- | ----------- |
| MainPage.xaml.cs | Class responsible for displaying the pivot view of the application. |
| WeeklyGrid.cs | Custom control based on System.Windows.Controls.Control. Describes the view of a single day. |
| EventViewModel.cs | View model class describing a single event saved by the user. |
| MainViewModel.cs | View model class responsible for loading event data from disk and maintaining clipboard. |
| WeeklyEvent.cs | Custom control based on System.Windows.Controls.Button that visualises a single event saved by the user in the grid of a single day. |


Known issues
------------

No known issues.


License
-------

See the license file delivered with this project. The license is also available online at https://github.com/Microsoft/weekly-planner-wp/blob/master/License.txt

Version history
---------------

 * 1.3.0 Add support for 720p resolution and NuGet package restore.
 * 1.2.0 Bug fixes and changes based on reviews
 * 1.1.0 Bug fixes and changes based on reviews; published on the Nokia Developer website.
 * 1.0.1 Support for Windows Phone OS version 7.1
 * 1.0.0 First version.
