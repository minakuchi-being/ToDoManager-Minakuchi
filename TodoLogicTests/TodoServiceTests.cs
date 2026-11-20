using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ToDoManager.Models;
using ToDoManager.Services;

namespace ToDoManagerTests {
    /// <summary>
    /// TodoServiceの単体テストクラス
    /// </summary>
    [TestClass]
    public class TodoServiceTests {
        /// <summary>
        /// AddOrUpdateで新規アイテムが追加されること
        /// </summary>
        [TestMethod]
        public void AddOrUpdate_ToDoの追加() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = "Test", Content = "TestContent", DueDate = DateTime.Today, IsCompleted = false };
            wService.AddOrUpdate(wItem);
            var wItems = wService.GetItems().ToList();
            Assert.AreEqual(1, wItems.Count);
            Assert.AreEqual("Test", wItems[0].Title);
        }

        /// <summary>
        /// AddOrUpdateで既存アイテムが更新されること
        /// </summary>
        [TestMethod]
        public void AddOrUpdate_ToDoの更新() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = "Test", Content = "TestContent", DueDate = DateTime.Today, IsCompleted = false };
            wService.AddOrUpdate(wItem);
            wItem.Title = "Updated";
            wService.AddOrUpdate(wItem);
            var wItems = wService.GetItems().ToList();
            Assert.AreEqual(1, wItems.Count);
            Assert.AreEqual("Updated", wItems[0].Title);
        }

        #region Validate
        /// <summary>
        /// Validateで正常な入力が通過すること
        /// </summary>
        [TestMethod]
        public void Validate_プロパティが正常な入力のとき_通過する() {
            var wService = new TodoService();
            wService.Validate(new TodoItem { Title = "a" });
            wService.Validate(new TodoItem { Title = new string('a', 20) });
            wService.Validate(new TodoItem { Title = " Test " });
            wService.Validate(new TodoItem { Title = "Test", Content = "" });
            wService.Validate(new TodoItem { Title = "Test", Content = new string('a', 150) });
        }

        /// <summary>
        /// Validateでタイトルが未入力のとき例外がスローされること
        /// </summary>
        [TestMethod]
        public void Validate_Titleが0文字のとき_例外がスローされる() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = "" };
            Assert.ThrowsException<ArgumentException>(() => wService.Validate(wItem));
        }

        /// <summary>
        /// Validateでタイトルが空白だけのとき例外がスローされること
        /// </summary>
        [TestMethod]
        public void Validate_Titleが空白だけのとき_例外がスローされる() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = " " };
            Assert.ThrowsException<ArgumentException>(() => wService.Validate(wItem));
        }

        /// <summary>
        /// Validateでタイトルが上限を超えるとき例外がスローされること
        /// </summary>
        [TestMethod]
        public void Validate_Titleが21文字のとき_例外がスローされる() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = new string('a', 21) };
            Assert.ThrowsException<ArgumentException>(() => wService.Validate(wItem));
        }

        /// <summary>
        /// Validateで内容が上限を超えるとき例外がスローされること
        /// </summary>
        [TestMethod]
        public void Validate_Contentが151文字のとき_例外がスローされる() {
            var wService = new TodoService();
            var wItem = new TodoItem { Title = "Test", Content = new string('a', 151) };
            Assert.ThrowsException<ArgumentException>(() => wService.Validate(wItem));
        }
        #endregion

        /// <summary>
        /// SortByDueDateで期限順にソートされること
        /// </summary>
        [TestMethod]
        public void SortByDueDate_期限順ソート() {
            var wService = new TodoService();
            var wItem1 = new TodoItem { Title = "Test1", DueDate = DateTime.Today.AddDays(1) };
            var wItem2 = new TodoItem { Title = "Test2", DueDate = DateTime.Today };
            wService.AddOrUpdate(wItem1);
            wService.AddOrUpdate(wItem2);

            wService.SortByDueDate();
            var wItems = wService.GetItems().ToList();

            Assert.AreEqual("Test2", wItems[0].Title);
            Assert.AreEqual("Test1", wItems[1].Title);
        }

        /// <summary>
        /// SortByAddedOrderで追加順にソートされること
        /// </summary>
        [TestMethod]
        public void SortByAddedOrder_追加順ソート() {
            var wService = new TodoService();
            var wItem1 = new TodoItem { Title = "Test1", DueDate = DateTime.Today.AddDays(1) };
            var wItem2 = new TodoItem { Title = "Test2", DueDate = DateTime.Today };
            wService.AddOrUpdate(wItem1);
            wService.AddOrUpdate(wItem2);

            wService.SortByAddedOrder();
            var wItems = wService.GetItems().ToList();

            Assert.AreEqual("Test1", wItems[0].Title);
            Assert.AreEqual("Test2", wItems[1].Title);
        }
    }
}
