using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Newtonsoft.Json;

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

        public List<ListViewItem> RenderedItemList = new List<ListViewItem>();
        public List<ListViewHeaderItem> AllListViewHeaderItems { get; set; } = new List<ListViewHeaderItem>();
        public List<ListViewHeaderItem> DisplayedHeaderItems { get; set; } = new List<ListViewHeaderItem>();

        public ListResearch()
        {
            this.InitializeComponent();

            DateTests = new DateTest().SetInitData();
            TestListViewCollection.Source = DateTests;
            SetGridViewTestData(DateTests);
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
            var ss3 = args.ItemContainer as ListViewItem;
            var ss4 = args.ItemIndex;
            var ss5 = args.Phase;

            CheckRecycleRenderedList(ss1, ss2, ss3);            // Now Rendered list

            OnlyDisplayedHeaderItem();
        }

        /// <summary>
        /// 실제 동작함
        /// </summary>
        /// <param name="isRecycle"></param>
        /// <param name="dateTest"></param>
        public void CheckRecycleRenderedList(bool isRecycle, DateTest dateTest, ListViewItem nowItem)
        {
            if (isRecycle)      // 다시 가상화 되는 상태
            {
                NowRenderedList.Remove(dateTest);
                RenderedItemList.Remove(nowItem);
            }
            else
            {
                NowRenderedList.Add(dateTest);
                RenderedItemList.Add(nowItem);
            }

            NowCount.Text = "Now rendered items count : " + NowRenderedList.Count.ToString();

            // PointerWheelChagnedEvent 가 발생하지 않으므로 Container에서 검색해서 Index가 ItemSource의 Count와 같으면 맨 끝으로 간주
            var renderedLastItem = NowRenderedList.OrderBy(x => x.Name).LastOrDefault();
            var endItem = DateTests.LastOrDefault().LastOrDefault() as DateTest;
            if (renderedLastItem == endItem)
            {
                //AddNewData();
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

        public void OnlyDisplayedHeaderItem()
        {
            AllListViewHeaderItems = new List<ListViewHeaderItem>();
            DisplayedHeaderItems = new List<ListViewHeaderItem>();
            ContentChangeTest.Text = "";

            var _border = VisualTreeHelper.GetChild(TestListView, 0);                   // Border
            var _scrollViewer = VisualTreeHelper.GetChild(_border, 0) as ScrollViewer;  // ScrollViewer

            EnumVisual(_scrollViewer);      // ScrollView의 자식 Component 찾기

            foreach (var item in AllListViewHeaderItems)
            {
                if (item.RenderTransform is MatrixTransform)
                {
                    DisplayedHeaderItems.Remove(item);
                }
                else if (item.RenderTransform is CompositeTransform)
                {
                    DisplayedHeaderItems.Add(item);
                }
            }

            foreach (var item in DisplayedHeaderItems)
            {
                var ss = EnumVisual<TextBlock>(item);
                ContentChangeTest.Text += ss?.Text + " / ";
            }            
        }

        private void NowTopItem_Click(object sender, RoutedEventArgs e)
        {
            OnlyDisplayedHeaderItem();
        }

        #region GridView 영역
        private void OtherControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var aa = sender as GridViewItem;
        }

        /// <summary>
        /// 그리드 뷰에 Group Header만 추가
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
        #endregion

        // Enumerate all the descendants of the visual object.
        public void EnumVisual(DependencyObject myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                DependencyObject childVisual = VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                if (childVisual is ListViewHeaderItem headerItem)
                {
                    if(!AllListViewHeaderItems.Contains(headerItem))
                        AllListViewHeaderItems.Add(headerItem);
                }

                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }

        public T EnumVisual<T>(DependencyObject myVisual) where T: class
        {
            T reValue = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                DependencyObject childVisual = VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                if (childVisual is T headerItem)
                {
                    return headerItem;
                }

                // Enumerate children of the child visual object.
                reValue = EnumVisual<T>(childVisual);
                if (reValue != null)
                    break;
            }

            return reValue;
        }
    }
}
