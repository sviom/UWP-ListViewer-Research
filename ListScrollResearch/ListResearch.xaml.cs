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
        public ObservableCollection<DateGroup> DateTests { get; set; }
        public ObservableCollection<DateTest> NowRenderedList { get; set; } = new ObservableCollection<DateTest>();

        public ListResearch()
        {
            this.InitializeComponent();
            TestListView.PointerWheelChanged += TestListView_PointerWheelChanged;
            //NowRenderedList = new List<DateTest>();
            DateTests = new DateTest().SetInitData();
            TestListViewCollection.Source = DateTests;
            SetGridViewTestData(DateTests);
        }

        private void TestListView_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TestListView_Loaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < TestListView.Items.Count; i++)
            //{
            //    var nowDateTest = TestListView.Items[i] as DateTest;
            //    if (nowDateTest.Date.Date == DateTimeOffset.Now.Date)
            //    {
            //        TestListView.SelectedIndex = i;
            //        TestListView.ScrollIntoView(TestListView.Items[i]);
            //        break;
            //    }
            //}
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Content가 추가로 로딩될 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TestListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var ss = args.Handled;
            var ss1 = args.InRecycleQueue;
            var ss2 = args.Item as DateTest;
            var ss3 = args.ItemContainer;
            var ss4 = args.ItemIndex;
            var ss5 = args.Phase;

            Debug.WriteLine(ss);
            Debug.WriteLine(ss1);
            Debug.WriteLine(ss2.Name);
            Debug.WriteLine(ss3);
            Debug.WriteLine(ss4);
            Debug.WriteLine(ss5);
            Debug.WriteLine("------------------------------");

            try
            {
                CheckRecycleTest(ss1, ss2);
            }
            catch(Exception ex)
            {

            }
            
        }

        /// <summary>
        /// 실제 동작함
        /// </summary>
        /// <param name="isRecycle"></param>
        /// <param name="dateTest"></param>
        public void CheckRecycleTest(bool isRecycle, DateTest dateTest)
        {
            //Debug.WriteLine("IsRecycle : " + isRecycle.ToString() + "/ DateTest : " + dateTest.Name);
            if (isRecycle)      // 다시 가상화 되는 상태
            {
                //Debug.WriteLine("Removed");
                NowRenderedList.Remove(dateTest);
            }
            else
            {
                //Debug.WriteLine("Added");
                NowRenderedList.Add(dateTest);
            }

            NowCount.Text = "Now rendered items count : " + NowRenderedList.Count.ToString();
            ContentChangeTest.Text = "Top Item = " + NowRenderedList.OrderBy(x => x.Date).FirstOrDefault().Name;

            // PointerWheelChagnedEvent 가 발생하지 않으므로 Container에서 검색해서 Index가 ItemSource의 Count와 같으면 맨 끝으로 간주
            var renderedLastItem = NowRenderedList.OrderBy(x => x.Name).LastOrDefault();
            var endItem = DateTests.LastOrDefault().LastOrDefault() as DateTest;
            if(renderedLastItem == endItem)
            {
                AddNewData();
            }
        }

        /// <summary>
        /// 나중에 데이터 더 추가하기 테스트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddData_Click(object sender, RoutedEventArgs e)
        {
            AddNewData();

            //DateTest.TestCasesGroup = DateTest.TestCasesGroup.OrderBy(x=>);
        }

        private void AddNewData()
        {
            DateGroup addedBeforeData = new DateGroup
            {
                Key = DateTime.Now.AddDays(-2).ToString()
            };
            DateGroup addedData = new DateGroup
            {
                Key = DateTime.Now.AddDays(2).ToString()
            };

            for (int i = 0; i < 100; i++)
            {
                addedData.Add(new DateTest() { Name = "new_test_" + i, Date = DateTime.Now.AddDays(1) });
                addedBeforeData.Add(new DateTest() { Name = "new_before_test_" + i, Date = DateTime.Now.AddDays(1) });
            }

            DateTest.TestCasesGroup.Add(addedData);
            DateTest.TestCasesGroup.Insert(0, addedBeforeData);
        }

        /// <summary>
        /// 선택 시 다른 이벤트로 이벤트 전달
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ListView).SelectedItem as DateTest;
            var selectedItemDate = selectedItem.Date;
            //OtherControl.SelectionChanged += OtherControl_SelectionChanged;
        }

        private void OtherControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();

        }

        /// <summary>
        /// 다른 컨트롤
        /// </summary>
        /// <param name="dateGroups"></param>
        public void SetGridViewTestData(ObservableCollection<DateGroup> dateGroups)
        {
            foreach (var item in dateGroups)
            {
                TextBlock _block = new TextBlock
                {
                    Text = item.GroupName
                };
                OtherControl.Items.Add(_block);
            }
        }
    }
}
