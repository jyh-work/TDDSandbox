using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using NUnit.Framework;
using TDDSandbox.Controls;

namespace TDDSandbox.UnitTests
{
    [TestFixture]
    public class SearchableTextBlockTests
    {
        private static class TestData
        {
            public const string SearchValue = "to";
            public const string SourceValueZeroMatch = "there is no match";
            public const string SourceValueOneMatch = "there is only one match at the end. to";
            public const string SourceValueTwoMatch = "there are two of 'to' that match 'to' keyword";

            public static readonly Brush SearchValueForground = Brushes.Gray;
            public static readonly Brush SourceValueForground = Brushes.Red;
        }

        [SetUp]
        public void Init()
        {
            //control must be initialized within the test thread
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void SearchValue_ByDefaultBlank_OneInlines()
        {
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueZeroMatch);
                Assert.IsTrue(ctrl.Inlines.Count == 1);
            });
        }

        [Test]
        public void SearchValue_ByDefaultBlank_OneInlineEqualToSourceText()
        {
            var result = string.Empty;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueZeroMatch);
                var inline = ctrl.Inlines.FirstInline as Run;
                result = inline == null ? null : inline.Text;
            });
            Assert.AreEqual(result, TestData.SourceValueZeroMatch);
        }

        [Test]
        public void SearchValue_OneMatch_OneMatchedInlines()
        {
            var result = 0;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueOneMatch, TestData.SearchValue);
                var matched = (from r in ctrl.Inlines
                               select r as Run)
                    .ToList<Run>()
                    .Where(r => string.Compare(r.Text, ctrl.SearchValue, StringComparison.CurrentCultureIgnoreCase) == 0);
                result = matched.Count();
            });
            Assert.AreEqual(1, result);
        }

        [Test]
        public void SearchValue_TwoMatches_TwoMatchedInlines()
        {
            var result = 0;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueTwoMatch, TestData.SearchValue);
                var runs = (from r in ctrl.Inlines
                               select r as Run)
                    .ToList<Run>();
                var matched = runs
                    .Where(r => string.Compare(r.Text, ctrl.SearchValue, StringComparison.CurrentCultureIgnoreCase) == 0);
                result = matched.Count();
            });
            Assert.AreEqual(2, result);
        }

        [Test]
        public void SearchValue_HasMatch_SearchValueIsBold()
        {
            var result = FontWeights.Normal;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueTwoMatch, TestData.SearchValue);
                var matched = (from r in ctrl.Inlines
                               select r as Run)
                    .ToList<Run>()
                    .FirstOrDefault(r => string.Compare(r.Text, ctrl.SearchValue, StringComparison.CurrentCultureIgnoreCase) == 0);
                result = matched == null ? FontWeights.Normal : matched.FontWeight;
            });
            Assert.AreEqual(FontWeights.Bold, result);
        }

        [Test]
        public void SearchValue_HasMatch_SearchValueForegroundIsHighlightedForeground()
        {
            var result = TestData.SourceValueForground;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(TestData.SourceValueTwoMatch, TestData.SearchValue);
                var matched = (from r in ctrl.Inlines
                               select r as Run)
                    .ToList<Run>()
                    .FirstOrDefault(r => string.Compare(r.Text, ctrl.SearchValue, StringComparison.CurrentCultureIgnoreCase) == 0);
                result = matched == null ? TestData.SourceValueForground : matched.Foreground;
            });
            Assert.AreEqual(TestData.SearchValueForground, result);
        }

        [Test]
        public void SourceValue_HasOneMatchWithExistingSearchValue_OneMatchedInlines()
        {
            var result = 0;
            RunSTAThread(() =>
            {
                var ctrl = TestObject(null, TestData.SearchValue);
                ctrl.SourceText = TestData.SourceValueOneMatch;
                var matched = (from r in ctrl.Inlines
                               select r as Run)
                    .ToList<Run>()
                    .Where(r => string.Compare(r.Text, ctrl.SearchValue, StringComparison.CurrentCultureIgnoreCase) == 0);
                result = matched.Count();
            });
            Assert.AreEqual(1, result);
        }

        private SearchableTextBlock TestObject(string source = null, string searchText = null)
        {
            return new SearchableTextBlock
            {
                SourceText = source,
                HighlightForeground = TestData.SearchValueForground,
                SearchValue = searchText,
                Foreground = TestData.SourceValueForground
            };
        }

        private void RunSTAThread(Action action)
        {
            var thread = new Thread(new ThreadStart(action));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(); //must join for unit testing
        }

    }
}
