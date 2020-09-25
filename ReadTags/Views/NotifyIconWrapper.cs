using System;
using System.ComponentModel;
using System.Windows;

namespace ReadTags.Views {
    /// <summary>
    /// タスクトレイ通知アイコン
    /// </summary>
    public partial class NotifyIconWrapper : Component {
        /// <summary>
        /// NotifyIconWrapper クラス　を生成、初期化します
        /// デジタル署名パスワード bm7sVqjM
        /// </summary>
        public NotifyIconWrapper() {
            // コンポーネントの初期化
            InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.Version.Click += this.versionClick;
            this.Exit.Click += this.exitClick;
        }

        /// <summary>
        /// コンテナ　をしていして　NotifyIconWrapper　クラス　を生成、初期化します。
        /// </summary>
        /// <param name="container">コンテナ</param>
        public NotifyIconWrapper(IContainer container) {
            container.Add(this);

            InitializeComponent();
        }

        private void versionClick(object sender, EventArgs e) {
            AboutBoxWindow aboutBoxWindow = new AboutBoxWindow();
            aboutBoxWindow.Show();
        }
        private void exitClick(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e) {

        }
    }
}
