using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.ComponentModel;
using PyxisPrepAshp.Patterns;
using TDDSandbox.ViewModels;

namespace TDDSandbox.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IAutoFillDataService _dataService;
        private readonly SearchAutoFillViewModel _searchAutoFillView;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            _dataService = new FakeAutoFillDataService();
            _searchAutoFillView = new SearchAutoFillViewModel(_dataService);
            InitializeContent();
        }

        public SearchAutoFillViewModel SearchAutoFillViewModel
        {
            get { return _searchAutoFillView; }
        }

        private void InitializeContent()
        {
            this.SearchAutoFill.Content = SearchAutoFillViewModel;
        }

    }


    internal class FakeAutoFillDataService : IAutoFillDataService
    {
        private static readonly AutoFillItem[] _testData = new AutoFillItem[]
                    {
                        new AutoFillItem
                            {
                                Id = "P11",
                                DisplayGroup = AutoFillDisplayGroup.Patient,
                                Title = "Willow, Don"
                            },
                        new AutoFillItem
                            {
                                Id = "P132",
                                DisplayGroup = AutoFillDisplayGroup.Patient,
                                Title = "Pine, Susan"
                            },
                        new AutoFillItem
                            {
                                Id = "P1345",
                                DisplayGroup = AutoFillDisplayGroup.Patient,
                                Title = "Beech, Mark"
                            },
                        new AutoFillItem
                            {
                                Id = "M1345",
                                DisplayGroup = AutoFillDisplayGroup.Med,
                                Title = "methylPREDNISolone 100 mg"
                            },
                        new AutoFillItem
                            {
                                Id = "M345",
                                DisplayGroup = AutoFillDisplayGroup.Med,
                                Title = "cefTRIAXone 2 gm"
                            },
                        new AutoFillItem
                            {
                                Id = "M3345",
                                DisplayGroup = AutoFillDisplayGroup.Med,
                                Title = "sodium chloride 0.9% 100 mL"
                            },
                        new AutoFillItem
                            {
                                Id = "test",
                                DisplayGroup = AutoFillDisplayGroup.Med,
                                Title = "there are two of 'to' that match 'to' keyword"
                            },
                    };

        public IEnumerable<AutoFillItem> GetAutoFillList(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new AutoFillItem[0];
            }

            var result = _testData.ToList();

            return result.Where(d => d.Id.Includes(searchText) ||
                                     d.Title.Includes(searchText));
        }

    }

    internal static class StringExtention
    {
        public static bool Includes(this string source, string val, bool caseInsensitive = true)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(val))
            {
                return false;
            }
            if (caseInsensitive)
            {
                source = source.ToLower();
                val = val.ToLower();
            }
            return source.Contains(val);
        }
    }

}
