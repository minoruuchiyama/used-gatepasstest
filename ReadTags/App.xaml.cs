using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ReadTags.Views;
using ReadTags.Models;

namespace ReadTags
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {

        private Mutex Mutex = new Mutex(false, "ReadTags");

        /// <summary>
        /// タスクトレイに表示するアイコン
        /// </summary>
        private NotifyIconWrapper notifyIcon;


        protected override void OnStartup(StartupEventArgs e) {

            // UI スレッドで実行されているコードで処理されなかったら発生する（.NET 3.0 より）
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // バックグラウンドタスク内で処理されなかったら発生する（.NET 4.0 より）
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // 例外が処理されなかったら発生する（.NET 1.0 より）
            AppDomain.CurrentDomain.UnhandledException  += CurrentDomain_UnhandledException;

            // 既に起動している場合
            if (!Mutex.WaitOne(0, false)) {

                //新しく起動した方は終了させる
                Mutex.Close();
                Mutex = null;
                Shutdown();

            //起動していない場合
            } else {
                //フォント変更
                var font = new System.Windows.Media.FontFamily("Meiryo UI");
                var style = new Style(typeof(Window));
                style.Setters.Add(new Setter(Window.FontFamilyProperty, font));
                FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(style));

                //タスクアイコンの表示
                base.OnStartup(e);
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                notifyIcon = new NotifyIconWrapper();

                //Webサーバーを起動
                WebServerModel.ServerStartAsync();
            }
        }

        protected override void OnExit(ExitEventArgs e) {

            if (Mutex != null) {
                Mutex.ReleaseMutex();
                Mutex.Close();
            }

            base.OnExit(e);
        }

        /// <summary>
        /// UI スレッドで発生した未処理例外を処理します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            var exception = e.Exception as Exception;
            if (ConfirmUnhandledException(exception, "UI スレッド")) {
                e.Handled = true;
            } else {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// バックグラウンドタスクで発生した未処理例外を処理します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            var exception = e.Exception.InnerException as Exception;
            if (ConfirmUnhandledException(exception, "バックグラウンドタスク")) {
                e.SetObserved();
            } else {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 実行を継続するかどうかを選択できる場合の未処理例外を処理します。
        /// </summary>
        /// <param name="e">例外オブジェクト</param>
        /// <param name="sourceName">発生したスレッドの種別を示す文字列</param>
        /// <returns>継続することが選択された場合は true, それ以外は false</returns>
        bool ConfirmUnhandledException(Exception e, string sourceName) {
            var message = $"エラーが発生しました。続けて発生する場合は開発者に報告してください。\nプログラムの実行を継続しますか？";
            if (e != null) message += $"\n{e.GetType().ToString()}\n({e.Message} @ {e.TargetSite.Name})\n[スタックトレース]\n{e.StackTrace}\n[InnerException]\n{e.InnerException}";
            // Logger.Fatal($"未処理例外 ({sourceName})", e); // 適当なログ記録
            var result = MessageBox.Show(message, $"未処理例外 ({sourceName})", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// 最終的に処理されなかった未処理例外を処理します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            var exception = e.ExceptionObject as Exception;
            var message = $"エラーが発生しました。続けて発生する場合は開発者に報告してください。";
            if (exception != null) message += $"\n{e.GetType().ToString()}\n({exception.Message} @ {exception.TargetSite.Name})";
            // Logger.Fatal("未処理例外", exception); // 適当なログ記録
            MessageBox.Show(message, "未処理例外", MessageBoxButton.OK, MessageBoxImage.Stop);
            Environment.Exit(1);
        }
    }
}
