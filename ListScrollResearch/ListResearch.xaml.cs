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

            //OnlyDisplayedItem();
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

        public void OnlyDisplayedItem()
        {
            var _border = VisualTreeHelper.GetChild(TestListView, 0);                   // Border
            var _scrollViewer = VisualTreeHelper.GetChild(_border, 0) as ScrollViewer;  // ScrollViewer

            //foreach (var item in TestListView.Items)
            //{
            //    if (TestListView.ContainerFromItem(item) is ListViewItem viewItem)
            //    {
            //        //var viewItem = TestListView.ContainerFromItem(item) as ListViewItem;
            //        var viewProperties = viewItem.GetType().GetProperties();
            //        var classItem = item as DateTest;

            //        foreach (var property in viewProperties)
            //        {
            //            if (property.Name.Contains("RenderTransform"))
            //            {
            //                var propertyValue = property.GetValue(viewItem);
            //                Debug.WriteLine("RenderTransform : " + propertyValue + " / Item : " + classItem.Name);
            //            }
            //            else if (property.Name.Contains("matrixTransform"))
            //            {
            //                Debug.WriteLine("Maxtrix : " + classItem.Name);
            //            }
            //        }
            //    }
            //}

            //return;

            var verticalOffset = _scrollViewer.VerticalOffset;       // 현재 스크롤 위치
            Debug.WriteLine("Vertical Offset : " + verticalOffset);

            EnumVisual(_scrollViewer);

            var firstItemProperties = ddd[0].GetType().GetProperties();
            var secondItemProperties = ddd[1].GetType().GetProperties();

            List<object> fir = new List<object>();
            List<object> sec = new List<object>();
            string aa = "";
            string bb = "";
            for (int i = 0; i < firstItemProperties.Length; i++)
            {
                var firstValue = firstItemProperties[i].GetValue(ddd[0]);
                var secondValue = secondItemProperties[i].GetValue(ddd[1]);
                Debug.WriteLine("first : " + firstValue + " / " + "second : " + secondValue);

                fir.Add(firstValue);
                sec.Add(secondValue);

                aa += firstValue?.ToString() + Environment.NewLine;
                bb += secondValue?.ToString() + Environment.NewLine;
            }

            Debug.WriteLine(ddd);

            double tessss = 0;
            List<ListViewItem> testlist = new List<ListViewItem>();
            for (int i = 0; i < TestListView.Items.Count; i++)
            {
                if (TestListView.ContainerFromIndex(i) is ListViewItem container)
                {
                    testlist.Add(container);
                    tessss += container.ActualHeight;
                }
            }

            ContentChangeTest.Text = "Vertical Offset : " + verticalOffset + " / Rendered Height" + tessss + " / ViewPort : " + _scrollViewer.ViewportHeight;

            return;

            double topItemOffset = 0.0;                            // Top Item offset
            //TestListViewCollection.Source
            for (int i = 0; i < TestListView.Items.Count; i++)
            {
                var tempItem = TestListView.Items[i] as DateTest;
                var container = TestListView.ContainerFromIndex(i) as ListViewItem;

                if (NowRenderedList.Count > 1)
                {
                    if (NowRenderedList[1] == tempItem)      // Rendered List의 0번째 아이템은 항상 Index 0 아이템 고정
                        break;
                    else
                        topItemOffset += container.ActualHeight;
                }
                else
                    topItemOffset += container.ActualHeight;
            }
            Debug.WriteLine("Top Item Offset : " + topItemOffset);
            Debug.WriteLine("VO - TIO = " + (verticalOffset - topItemOffset));

            int visibleItemIndex = 0;
            double tempActualHeight = 0;
            foreach (var renderedItem in RenderedItemList)
            {
                var offsetCompare = (int)Math.Floor(verticalOffset - topItemOffset);
                if (tempActualHeight == (offsetCompare - offsetCompare % 10))
                {
                    ContentChangeTest.Text = "Now location : " + NowRenderedList[visibleItemIndex].Name;
                    break;      // 해당 아이템 다음 아이템이 Displayed Item                
                }
                tempActualHeight += renderedItem.ActualHeight;
                visibleItemIndex++;
            }
        }

        private void NowTopItem_Click(object sender, RoutedEventArgs e)
        {
            OnlyDisplayedItem();
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

        public static List<ListViewHeaderItem> ddd = new List<ListViewHeaderItem>();
        // Enumerate all the descendants of the visual object.
        static public void EnumVisual(DependencyObject myVisual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                DependencyObject childVisual = VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                if (childVisual is ListViewHeaderItem headerItem)
                {
                    ddd.Add(headerItem);
                }

                // Enumerate children of the child visual object.
                EnumVisual(childVisual);
            }
        }
    }
}
