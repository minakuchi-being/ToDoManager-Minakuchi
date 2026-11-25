using System;
using System.Windows.Forms;
using ToDoManager.Models;
using ToDoManager.Services;

namespace ToDoManager {
    /// <summary>
    /// メインのToDo管理フォーム
    /// </summary>
    public partial class MainForm : Form {
        #region フィールド・初期化
        private readonly TodoService FService = new TodoService();

        public MainForm() {
            InitializeComponent();
            UpdateList();
        }
        #endregion

        #region UI操作
        /// <summary>
        /// ToDoリストを更新
        /// </summary>
        private void UpdateList() {
            FLstItems.Items.Clear();
            foreach (var wItem in FService.GetItems()) FLstItems.Items.Add(wItem);
        }
        #endregion

        #region ToDo操作
        /// <summary>
        /// 追加アイテムを処理
        /// </summary>
        private void AddItem() {
            using (var wForm = new TodoEditForm(new TodoItem())) {
                if (wForm.ShowDialog() == DialogResult.OK) {
                    try {
                        FService.AddOrUpdate(wForm.Item);
                        UpdateList();
                    } catch (ArgumentException wEx) {
                        MessageBox.Show(this, wEx.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    } catch (Exception wEx) {
                        MessageBox.Show(this, $"保存に失敗しました：{wEx.Message}", "システムエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// アイテムを編集
        /// </summary>
        private void EditItem() {
            if (!(FLstItems.SelectedItem is TodoItem wSelected)) return;

            using (var wForm = new TodoEditForm(wSelected)) {
                if (wForm.ShowDialog() == DialogResult.OK) {
                    try {
                        FService.AddOrUpdate(wForm.Item);
                        UpdateList();
                    } catch (ArgumentException wEx) {
                        MessageBox.Show(this, wEx.Message, "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    } catch (InvalidOperationException wEx) {
                        MessageBox.Show(this, wEx.Message, "編集エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } catch (Exception wEx) {
                        MessageBox.Show(this, $"編集に失敗しました：{wEx.Message}", "システムエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// アイテムを削除
        /// </summary>
        private void DeleteItem() {
            if (!(FLstItems.SelectedItem is TodoItem wSelected)) return;

            var wConfirm = MessageBox.Show(this, $"「{wSelected.Title}」を削除しますか？", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (wConfirm != DialogResult.Yes) return;

            try {
                FService.Delete(wSelected.Id);
                UpdateList();
            } catch (InvalidOperationException wEx) {
                MessageBox.Show(this, wEx.Message, "削除エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception wEx) {
                MessageBox.Show(this, $"削除に失敗しました：{wEx.Message}", "システムエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region イベントハンドラ
        private void FBtnAdd_Click(object sender, EventArgs e) => AddItem();
        private void FBtnEdit_Click(object sender, EventArgs e) => EditItem();
        private void FBtnDelete_Click(object sender, EventArgs e) => DeleteItem();
        private void FBtnXml_Click(object sender, EventArgs e) => FService.Export();
        private void SortByDueDateToolStripMenuItem_Click(object sender, EventArgs e) {
            FService.SortByDueDate();
            UpdateList();
        }
        private void SortByAddedOrderToolStripMenuItem_Click(object sender, EventArgs e) {
            FService.SortByAddedOrder();
            UpdateList();
        }
        private void FBtnXmlLoad_Click(object sender, EventArgs e) {
            if (FService.Import()) {
                UpdateList();
                MessageBox.Show(this, "データを読み込みました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show(this, "指定ファイルが存在しません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}