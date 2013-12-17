using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PyxisPrepAshp.Patterns;

namespace TDDSandbox.ViewModels
{
    public class AutoFillDisplayGroup
    {
        public const string Patient = "PATIENTS";
        public const string Med = "COMPONENTS";
    }

    public class AutoFillItem
    {
        public string DisplayGroup { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }
    }

    public interface IAutoFillDataService
    {
        IEnumerable<AutoFillItem> GetAutoFillList(string searchText);
    }

    /// <summary>
    /// The SearchAutoFillViewModel class performs following tasks:
    /// 1. call data service to retrieve a list of relevant data when the search text is populated
    /// 2. set DisplayAutoFillList property to true when items list is not empty.
    /// 3. set search text value when the SelectedItem is changed
    /// 4. If the SearchCommand is assigned, execute the command when the SelectedItem is changed
    /// 5. set ClearTextImage to visible when the search text is not empty
    /// 6. clear search text when the clear image is clicked (left mouse click) [UI unit test??]
    /// </summary>
    public class SearchAutoFillViewModel : DependencyObject, INotifyPropertyChanged
    {
        private readonly IAutoFillDataService _autoFillDataService;
        private bool _selectedItemChanging = false;
        private bool _searchTextChanging = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchAutoFillViewModel(IAutoFillDataService autoFillDataService)
        {
            _autoFillDataService = autoFillDataService;
            ClearTextImageVisibility = Visibility.Hidden;
            ClearTextCommand = new RelayCommand(() =>
                {
                    SearchText = null;
                });
            SearchTextEnteredCommand = new RelayCommand(OnSearchTextEntered);
        }

        public ICommand ClearTextCommand { get; private set; }
        public ICommand SearchTextEnteredCommand { get; private set; }

        public static DependencyProperty DisplayAutoFillListProperty = DependencyProperty.Register(
            "DisplayAutoFillList",
            typeof(bool),
            typeof(SearchAutoFillViewModel),
            new PropertyMetadata(null)
            );

        public bool DisplayAutoFillList
        {
            get { return (bool)GetValue(DisplayAutoFillListProperty); }
            set
            {
                SetValue(DisplayAutoFillListProperty, value);
                OnPropertyChanged("DisplayAutoFillList");
            }
        }

        #region ClearTextImageVisibility Property
        public static DependencyProperty ClearTextImageVisibilityProperty = DependencyProperty.Register(
            "ClearTextImageVisibility",
            typeof(Visibility),
            typeof(SearchAutoFillViewModel)
            );
 
        public Visibility ClearTextImageVisibility
        {
            get
            {
                return (Visibility) GetValue(ClearTextImageVisibilityProperty);
            }
            set
            {
                SetValue(ClearTextImageVisibilityProperty, value);
                OnPropertyChanged("ClearTextImageVisibility");
            }
        }
        #endregion

        #region Items Property
        public static DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(ObservableCollection<AutoFillItem>),
            typeof(SearchAutoFillViewModel)
            );
        public ObservableCollection<AutoFillItem> Items
        {
            get { return GetValue(ItemsProperty) as ObservableCollection<AutoFillItem>; }
            set
            {
                SetValue(ItemsProperty, value);
                OnPropertyChanged("Items");
            }
        }

        #endregion

        #region SearchText property

        private void OnSearchTextEntered()
        {
            _searchTextChanging = true;
            try
            {
                ClearTextImageVisibility = string.IsNullOrWhiteSpace(SearchText) ? Visibility.Hidden : Visibility.Visible;
                var data = _autoFillDataService.GetAutoFillList(SearchText);
                Items = new ObservableCollection<AutoFillItem>(data);
                SelectedItem = null;
            }
            finally
            {
                _searchTextChanging = false;
            }
        }

        public static DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText",
            typeof(string),
            typeof(SearchAutoFillViewModel),
            new PropertyMetadata(null)
            );

        public string SearchText
        {
            get { return GetValue(SearchTextProperty) as string; }
            set 
            {
                SetValue(SearchTextProperty, value);
                OnPropertyChanged("SearchText");
            }
        }

        #endregion

        #region SelectedItem

        public static DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(AutoFillItem),
            typeof(SearchAutoFillViewModel),
            new PropertyMetadata(SelectedItemPropertyChanged)
            );

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = e.NewValue as AutoFillItem;
            var model = d as SearchAutoFillViewModel;
            var txt = item == null ? null : item.Title;
            if (model != null && !model._searchTextChanging)
            {
                model._selectedItemChanging = true;
                try
                {
                    model.SearchText = txt;
                    model.DisplayAutoFillList = false;
                }
                finally
                {
                    model._selectedItemChanging = false;
                }
                if (model.SearchCommand != null)
                {
                    model.SearchCommand.Execute(txt);
                }
            }
        }

        public AutoFillItem SelectedItem
        {
            get { return GetValue(SelectedItemProperty) as AutoFillItem; }
            set
            {
                SetValue(SelectedItemProperty, value);
                OnPropertyChanged("SelectedItem");
            }
        }

        #endregion

        #region SearchCommand

        public static DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            "SearchCommand",
            typeof(ICommand),
            typeof(SearchAutoFillViewModel)
            );

        public ICommand SearchCommand
        {
            get { return GetValue(SearchCommandProperty) as ICommand; }
            set
            {
                SetValue(SearchCommandProperty, value);
                OnPropertyChanged("SearchCommand");
            }
        }

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            switch (propertyName)
            {
                case "Items":
                    DisplayAutoFillList = Items != null && Items.Any();
                    break;
            }
        }

    }

}

