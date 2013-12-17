using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NUnit.Framework;
using Moq;
using TDDSandbox.ViewModels;

namespace TDDSandbox.UnitTests
{
    [TestFixture]
    public class SearchAutoFillViewModelTests
    {
        private Mock<IAutoFillDataService> _autoFillService;
        private SearchAutoFillViewModel _autoFillViewModel;
        private Mock<ICommand> _command;
 
        private static class TestData
        {
            public const string SearchText = "cl";
            public const string SearchTextHasMatch = "e";
            public static readonly ObservableCollection<AutoFillItem> AutoFillItems =
                new ObservableCollection<AutoFillItem>(new AutoFillItem[]
                {
                    new AutoFillItem { Id = "123",
                        Title = "Med Item 1"}
                });
        }

        [SetUp]
        public void Init()
        {
            _autoFillService = new Mock<IAutoFillDataService>(MockBehavior.Loose);
            _autoFillViewModel = new SearchAutoFillViewModel(_autoFillService.Object);
            _command = new Mock<ICommand>(MockBehavior.Loose);
        }

        [TearDown]
        public void Dispose()
        {
            _autoFillService = null;
            _autoFillViewModel = null;
            _command = null;
        }

        [Test]
        public void ClearTextCommand_IsCalled_ClearSearchText()
        {
            _autoFillViewModel.SearchText = TestData.SearchText;
            _autoFillViewModel.ClearTextCommand.Execute(null);
            Assert.IsNullOrEmpty(_autoFillViewModel.SearchText);
        }

        [Test]
        public void SearchText_PopulatedAndDisplayItem_OneServiceCall()
        {
            SetViewModelSearchText(TestData.SearchText);
            _autoFillService.Verify(s => s.GetAutoFillList(_autoFillViewModel.SearchText), Times.Exactly(1));
        }

        [Test]
        public void SearchText_ByDefaultNotPopulated_NoServiceCall()
        {
            _autoFillService.Verify(s => s.GetAutoFillList(_autoFillViewModel.SearchText), Times.Never);
        }

        [Test]
        public void SearchText_ByDefaultIsEmpty_ClearTextImageVisibility_Hidden()
        {
            Assert.AreEqual(Visibility.Hidden, _autoFillViewModel.ClearTextImageVisibility);
        }

        [Test]
        public void SearchText_IsNotEmpty_ClearTextImageVisibility_Visible()
        {
            SetViewModelSearchText(TestData.SearchText);
            Assert.AreEqual(Visibility.Visible, _autoFillViewModel.ClearTextImageVisibility);
        }

        [Test]
        public void SearchText_ChangedToEmptyAndItemsVisible_ItemsIsEmpty()
        {
            SetupMockServiceData(_autoFillViewModel.SearchText);
            _autoFillViewModel.SearchText = TestData.SearchTextHasMatch;
            _autoFillViewModel.SearchText = string.Empty;
            Assert.IsTrue(_autoFillViewModel.Items == null || !_autoFillViewModel.Items.Any());
        }

        [Test]
        public void Items_ByDefaultIsNull_NotDisplayItems()
        {
            Assert.IsFalse(_autoFillViewModel.DisplayAutoFillList);
        }

        [Test]
        public void Items_IsEmpty_NotDisplayItems()
        {
            SetupMockServiceData(null);
            SetViewModelSearchText(string.Empty);
            Assert.IsFalse(_autoFillViewModel.DisplayAutoFillList);
        }

        [Test]
        public void Items_NotEmpty_DisplayItems()
        {
            SetupMockServiceData(TestData.SearchTextHasMatch);
            SetViewModelSearchText(TestData.SearchTextHasMatch);
            Assert.IsTrue(_autoFillViewModel.DisplayAutoFillList);
        }

        [Test]
        public void SelectedItem_Changed_SearchTextIsSelectedItemTitle()
        {
            SetupMockServiceData(TestData.SearchTextHasMatch);
            SetViewModelSearchText(TestData.SearchTextHasMatch);

            _autoFillViewModel.SelectedItem = _autoFillViewModel.Items.First();
            Assert.IsTrue(_autoFillViewModel.SearchText == _autoFillViewModel.SelectedItem.Title);
        }

        [Test]
        public void SearchCommand_Assigned_ExecutedOnSelectedItemChanged()
        {

            SetupMockServiceData(TestData.SearchTextHasMatch);
            SetViewModelSearchText(TestData.SearchTextHasMatch);
            _autoFillViewModel.SearchCommand = _command.Object;
            _autoFillViewModel.SelectedItem = _autoFillViewModel.Items.First();
            _command.Verify(cmd => cmd.Execute(_autoFillViewModel.SearchText), Times.Once);
        }

        [Test]
        public void SearchCommand_NotAssigned_NoExecutionOnSelectedItemChanged()
        {
            SetupMockServiceData(TestData.SearchTextHasMatch);
            SetViewModelSearchText(TestData.SearchTextHasMatch);
            _autoFillViewModel.SelectedItem = _autoFillViewModel.Items.First();
            _command.Verify(cmd => cmd.Execute(_autoFillViewModel.SearchText), Times.Never);
        }

        private void SetupMockServiceData(string searchText)
        {
            _autoFillService.Setup(s => s.GetAutoFillList(searchText)).Returns(TestData.AutoFillItems);
        }

        private void SetViewModelSearchText(string newValue)
        {
            _autoFillViewModel.SearchText = newValue;
            _autoFillViewModel.SearchTextEnteredCommand.Execute(newValue);
        }

    }
}
