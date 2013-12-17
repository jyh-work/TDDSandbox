using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TDDSandbox.Controls
{
    /// <summary>
    /// The SearchableTextBlock extend the standard TextBlock and performs following tasks:
    /// 1. When the source text contains one or more searched value, the control should split the source text to multiple Run components
    /// 2. Bold the searched value with assigned highlight property value
    /// 3. The highlighted text forground should match the HighlightedForground property value if the property is set
    /// </summary>
    public class SearchableTextBlock : TextBlock
    {
        static SearchableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableTextBlock), new FrameworkPropertyMetadata(typeof(SearchableTextBlock)));
        }

        private bool _resetingText;

        #region Properties
        new private string Text
        {
            set
            {
                ResetText(value);
            }
        }

        #endregion

        #region Dependency Properties

        #region Search Words

        public static readonly DependencyProperty SearchValueProperty =
            DependencyProperty.Register("SearchValue",
                typeof(string),
                typeof(SearchableTextBlock),
                new PropertyMetadata(new PropertyChangedCallback(SearchValuePropertyChanged)));

        public static void SearchValuePropertyChanged(DependencyObject inObj, DependencyPropertyChangedEventArgs e)
        {
            var stb = inObj as SearchableTextBlock;
            if (stb == null)
                return;
            stb.ResetText();
        }

        public string SearchValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(GetValue(SearchValueProperty) as string))
                    SetValue(SearchValueProperty, string.Empty);
                return GetValue(SearchValueProperty) as string;
            }
            set
            {
                SetValue(SearchValueProperty, value);
            }
        }

        #endregion

        #region SourceText
        public event EventHandler OnSourceTextChanged;

        public string SourceText
        {
            get { return (string)GetValue(SourceTextProperty); }
            set
            {
                SetValue(SourceTextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for SourceText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceTextProperty =
            DependencyProperty.Register("SourceText", typeof(string), typeof(SearchableTextBlock)
            , new PropertyMetadata(new PropertyChangedCallback(SourceTextChanged)));

        private static void SourceTextChanged(DependencyObject inObj, DependencyPropertyChangedEventArgs inArgs)
        {
            var stb = (SearchableTextBlock)inObj;

            stb.Text = stb.SourceText;

            if (stb.OnSourceTextChanged != null)
                stb.OnSourceTextChanged(stb, null);
        }
        #endregion

        #region HighlightForeground

        public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(
            "HighlightForeground",
            typeof(Brush),
            typeof(SearchableTextBlock),
            new PropertyMetadata(new PropertyChangedCallback(HighlightForgroundChanged))
            );

        public Brush HighlightForeground
        {
            get
            {
                return (Brush)GetValue(HighlightForegroundProperty);
            }
            set
            {
                SetValue(HighlightForegroundProperty, value);
            }
        }

        private static void HighlightForgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var stb = d as SearchableTextBlock;
            if (stb == null ||
                !stb.Inlines.Any() ||
                string.IsNullOrWhiteSpace(stb.SearchValue) ||
                stb._resetingText)
            {
                return;
            }
            stb.ResetText();
        }

        #endregion

        #endregion Dependency Properties

        private void ResetText()
        {
            ResetText(SourceText);
        }

        private void ResetText(string value)
        {
            Inlines.Clear();
            if (string.IsNullOrWhiteSpace(SearchValue) || string.IsNullOrWhiteSpace(value))
            {
                base.Text = value;
                return;
            }

            _resetingText = true;
            try
            {
                string[] split = Regex.Split(value, SearchValue, RegexOptions.IgnoreCase);
                int length = 0;
                foreach (var str in split)
                {
                    var searchLength = 0;
                    if (Inlines.Count > 0)
                    {
                        AddInline(value.Substring(length, SearchValue.Length));
                        searchLength = SearchValue.Length;
                    }
                    AddInline(str);
                    length = length + str.Length + searchLength;
                }
            }
            finally
            {
                _resetingText = false;
            }
        }

        private void AddInline(string value)
        {
            var run = new Run(value);
            if (Regex.IsMatch(value, SearchValue, RegexOptions.IgnoreCase))
            {
                run.FontWeight = FontWeights.Bold;
                run.Foreground = HighlightForeground;
            }
            Inlines.Add(run);
        }

    }
}
