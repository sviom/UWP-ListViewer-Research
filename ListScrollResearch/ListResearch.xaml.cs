using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace ListScrollResearch
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class ListResearch : Page
    {
        public ObservableCollection<DateTest> DateTests { get; set; }
        public ObservableCollection<DateTest> NowRenderedList { get; set; } = new ObservableCollection<DateTest>();

        public ListResearch()
        {
            this.InitializeComponent();
            //NowRenderedList = new List<DateTest>();
            TestListViewCollection.Source = new DateTest().SetInitData();
        }

        private void TestListView_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < TestListView.Items.Count; i++)
            {
                var nowDateTest = TestListView.Items[i] as DateTest;
                if (nowDateTest.Date.Date == DateTimeOffset.Now.Date)
                {
                    TestListView.SelectedIndex = i;
                    TestListView.ScrollIntoView(TestListView.Items[i]);
                    break;
                }
            }
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void TestListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var ss = args.Handled;
            var ss1 = args.InRecycleQueue;
            var ss2 = args.Item as DateTest;
            var ss3 = args.ItemContainer;
            var ss4 = args.ItemIndex;
            var ss5 = args.Phase;

            //Debug.WriteLine(ss);
            //Debug.WriteLine(ss1);
            //Debug.WriteLine(ss2.Name);
            //Debug.WriteLine(ss3);
            //Debug.WriteLine(ss4);
            //Debug.WriteLine(ss5);
            //Debug.WriteLine("------------------------------");

            CheckRecycleTest(ss1, ss2);
            //args.RegisterUpdateCallback(this.CheckRecycleTest);
            //args.Handled = true;
        }

        /// <summary>
        /// 콜백
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CheckRecycleTest(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            //Debug.WriteLine("IsRecycle : " + isRecycle.ToString() + "/ DateTest : " + dateTest.Name);
            var dateTest = args.Item as DateTest;
            if (args.InRecycleQueue)      // 다시 가상화 되는 상태
            {
                Debug.WriteLine("Removed");
                NowRenderedList.Remove(dateTest);
            }
            else
            {
                Debug.WriteLine("Added");
                NowRenderedList.Add(dateTest);
            }

            NowCount.Text = NowRenderedList.Count.ToString();            
        }

        /// <summary>
        /// 실제 동작함
        /// </summary>
        /// <param name="isRecycle"></param>
        /// <param name="dateTest"></param>
        public void CheckRecycleTest(bool isRecycle, DateTest dateTest)
        {
            Debug.WriteLine("IsRecycle : " + isRecycle.ToString() + "/ DateTest : " + dateTest.Name);
            if (isRecycle)      // 다시 가상화 되는 상태
            {
                Debug.WriteLine("Removed");
                NowRenderedList.Remove(dateTest);
            }
            else
            {
                Debug.WriteLine("Added");
                NowRenderedList.Add(dateTest);
            }

            NowCount.Text = "Now rendered items count : " + NowRenderedList.Count.ToString();
            ContentChangeTest.Text = "Top Item = " + NowRenderedList.OrderBy(x => x.Date).FirstOrDefault().Name;
            
        }
    }
}
