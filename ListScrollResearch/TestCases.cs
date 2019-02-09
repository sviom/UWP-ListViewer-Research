using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ListScrollResearch
{
    public class DateTest : INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private int _testCount = 3000;

        public static ObservableCollection<DateGroup> TestCasesGroup = new ObservableCollection<DateGroup>();

        public ObservableCollection<DateGroup> SetInitData()
        {
            

            DateGroup now = new DateGroup
            {
                Key = DateTime.Now.ToString(),
                GroupName = DateTime.Now.ToString()
            };

            DateGroup before = new DateGroup
            {
                Key = DateTime.Now.AddDays(-1).ToString(),
                GroupName = DateTime.Now.AddDays(-1).ToString()
            };

            DateGroup after = new DateGroup
            {
                Key = DateTime.Now.AddDays(1).ToString(),
                GroupName = DateTime.Now.AddDays(1).ToString()
            };

            for (int i = 0; i < _testCount; i++)
            {
                if (i < (_testCount / 3))
                {
                    before.Add(new DateTest() { Name = "test_" + i, Date = DateTime.Now.AddDays(-1) });
                }
                else if (i > (_testCount / 3 - 1) && i < (_testCount / 2))
                {
                    now.Add(new DateTest() { Name = "test_" + i, Date = DateTime.Now });
                }
                else
                {
                    after.Add(new DateTest() { Name = "test_" + i, Date = DateTime.Now.AddDays(1) });
                }
            }

            TestCasesGroup.Add(before);
            TestCasesGroup.Add(now);
            TestCasesGroup.Add(after);

            return TestCasesGroup;
        }


        public DateTimeOffset Date { get; set; }
        public string Name { get; set; }
    }

    /*
     * 
     * DateGroup List<object>
	    - Key(*DateTest-Date와 일치해야 Group화 가능)
		    ○ DateTest
			    § Name
			    § Date(*Key와 일치해야 함)
     */
    public class DateGroup : List<object>
    {
        public object Key { get; set; }
        public string GroupName { get; set; }
    }
}
