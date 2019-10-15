using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class GridViewScrollTest : Page
    {
        public GridViewScrollTest()
        {
            this.InitializeComponent();

            for (int k = 0; k < 4; k++)
            {
                var testInfo = new TestInfo();
                testInfo.Key = k;
                testInfo.GroupHeader = k + " 제목";
                for (int i = 0; i < 120; i++)
                {
                    testInfo.Add(i + k + " Test");
                }

                TestInfos.Add(testInfo);
            }
        }

        ObservableCollection<TestInfo> TestInfos { get; set; } = new ObservableCollection<TestInfo>();


        private void TestGridView_Loaded(object sender, RoutedEventArgs e)
        {
            var item = TestInfos[2];
            TestGridView.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
        }

        private void BackToMainButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }

    public class TestInfo : List<Object>
    {
        public object Key { get; set; }
        public string GroupHeader { get; set; }
    }
}
