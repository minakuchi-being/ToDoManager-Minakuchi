using System;
using System.Linq;
using NUnit.Framework;
using ToDoManager.Models;
using ToDoManager.Services;

namespace ToDoManagerTests {
    /// <summary>
    /// TodoServiceの単体テストクラス
    /// </summary>
    [TestFixture]
    public class TodoServiceTests {
        /// <summary>
        /// AddOrUpdateで新規アイテムが追加されること
        /// </summary>
        [Test]
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
        [Test]
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

        #region Validate 正常系
        /// <summary>
        /// Validateで、タイトルの文字数が正しいとき、例外が発生しないこと
        /// </summary>
        /// <param name="vLength">タイトルの文字数</param>
        [TestCase(1, Description = "タイトルが1文字（最小）")]
        [TestCase(20, Description = "タイトルが20文字（最大）")]
        public void Validate_Titleが1文字以上20文字以下のとき_例外が発生しない(int vLength) {
            var wItem = new TodoItem { Title = new string('a', vLength) };
            Assert.That(() => TodoService.Validate(wItem), Throws.Nothing);
        }

        /// <summary>
        /// Validateで、タイトルの前後に空白を含むとき、例外が発生しないこと
        /// </summary>
        /// <param name="vTitle">タイトル</param>
        [TestCase(" Test ", Description = "タイトルの前後にスペース（半角）")]
        [TestCase("　Test　", Description = "タイトルの前後にスペース（全角）")]
        public void Validate_Titleの先頭と末尾にスペースを含むとき_例外が発生しない(string vTitle) {
            var wItem = new TodoItem { Title = vTitle };
            Assert.That(() => TodoService.Validate(wItem), Throws.Nothing);
        }

        /// <summary>
        /// Validateで、内容が未設定のとき、例外が発生しないこと
        /// </summary>
        [Test]
        public void Validate_Contentがnullのとき_例外が発生しない() {
            var wItem = new TodoItem { Title = "Test", Content = null };
            Assert.That(() => TodoService.Validate(wItem), Throws.Nothing);
        }

        /// <summary>
        /// Validateで、内容の文字数が正しいとき、例外が発生しないこと
        /// </summary>
        /// <param name="vLength">内容の文字数</param>
        [TestCase(0, Description = "内容が0文字（最小・未入力）")]
        [TestCase(150, Description = "内容が150文字（最大）")]
        public void Validate_Contentが0文字以上150文字以下のとき_例外が発生しない(int vLength) {
            var wItem = new TodoItem { Title = "Test", Content = new string('a', vLength) };
            Assert.That(() => TodoService.Validate(wItem), Throws.Nothing);
        }
        #endregion

        #region Validate 異常系
        /// <summary>
        /// Validateで、タイトルが未設定のとき、例外が発生すること
        /// </summary>
        [Test]
        public void Validate_Titleがnullのとき_例外が発生する() {
            var wItem = new TodoItem { Title = null };
            Assert.That(() => TodoService.Validate(wItem), Throws.ArgumentException);
        }

        /// <summary>
        /// Validateで、タイトルが空欄のとき、例外が発生すること
        /// </summary>
        /// <param name="vTitle">タイトル</param>
        [TestCase("", Description = "タイトルが未入力")]
        [TestCase(" ", Description = "タイトルがスペースのみ（半角）")]
        [TestCase("　", Description = "タイトルがスペースのみ（全角）")]
        public void Validate_Titleが空欄のとき_例外が発生する(string vTitle) {
            var wItem = new TodoItem { Title = vTitle };
            Assert.That(() => TodoService.Validate(wItem), Throws.ArgumentException);
        }

        /// <summary>
        /// Validateで、タイトルの文字数が上限を超えたとき、例外が発生すること
        /// </summary>
        [Test]
        public void Validate_Titleが21文字以上のとき_例外が発生する() {
            var wItem = new TodoItem { Title = new string('a', 21) };
            Assert.That(() => TodoService.Validate(wItem), Throws.ArgumentException);
        }

        /// <summary>
        /// Validateで、内容の文字数が上限を超えたとき、例外が発生すること
        /// </summary>
        [Test]
        public void Validate_Contentが151文字以上のとき_例外が発生する() {
            var wItem = new TodoItem { Title = "Test", Content = new string('a', 151) };
            Assert.That(() => TodoService.Validate(wItem), Throws.ArgumentException);
        }
        #endregion

        /// <summary>
        /// SortByDueDateで期限順にソートされること
        /// </summary>
        [Test]
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
        [Test]
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