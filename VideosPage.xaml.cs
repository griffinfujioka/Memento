using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace Memento
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class VideosPage : Memento.Common.LayoutAwarePage
    {
        public VideosPage()
        {
            this.InitializeComponent();
            List<DummiePerson> list = new List<DummiePerson>()
            {
                new DummiePerson{Image_Name="Made some good progress", Image="Person1.jpg", Description="I finally figured out how to calculate congruency complexity!"},
                new DummiePerson{Image_Name="Signing off!", Image="Person2.jpg", Description="Thank you all for the awesome experience!"},
                new DummiePerson{Image_Name="What a fun night!", Image="Person3.jpg", Description="I had such a good time with my friends tonight."},
                new DummiePerson{Image_Name="Talk about a bad hair day...", Image="Person4.jpg", Description="You know what's awesome about summer? Long hair."},
                new DummiePerson{Image_Name="I've been thinking about this project", Image="Person5.jpg", Description="We're on track for greatness."},
                new DummiePerson{Image_Name="Ya know what I have to say about your haircuts... ", Image="Person6.jpg", Description="I don't like haircuts one bit. That's right, I said. it. "},
            };
            DataContext = list;
            groupedItemsViewSource.Source = list;  
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        //protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        //{
        //    // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
        //}
    }
}
