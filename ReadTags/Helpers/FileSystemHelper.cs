using System.IO;
using System;

namespace ReadTags.Helpers
{
    /// <summary>
    /// ファイル・ディレクトリ操作系Helper
    /// </summary>
    public static class FileSystemHelper{

        /// <summary>
        /// ファイルパス文字列を生成します
        /// </summary>
        /// <param name="directoryPath">ディレクトリパス</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="extension">拡張子</param>
        /// <param name="prefix">ファイル名に付く接頭辞</param>
        /// <param name="suffix">ファイル名に付く接尾辞</param>
        /// <returns></returns>
        public static string CreateFilePath(string directoryPath, string fileName ,string extension, string prefix="", string suffix="") {
            return AppendDirectorySeparator(directoryPath)+prefix+fileName+suffix+extension.ToLower();
        }

        /// <summary>
        /// ディレクトリ末尾の区切り記号が無い場合、補完する
        /// </summary>
        /// <param name="directoryPath">ディレクトリパス</param>
        /// <returns></returns>
        public static string AppendDirectorySeparator(string directoryPath) {
            return directoryPath.TrimEnd('\\')+"\\";
        }

        /// <summary>
        /// 指定されたディレクトリに書き込み権限が無い場合例外
        /// </summary>
        /// <param name="folderPath"></param>
        public static void HasWriteAccessToDirectory(string folderPath) {
            try {
                var ds = Directory.GetAccessControl(folderPath);

            } catch (UnauthorizedAccessException) {
                throw new Exception($"ディレクトリに権限がありません：{folderPath}");
            }
        }

        /// <summary>
        /// 指定されたファイルがロック(排他ロック・権限ロック)されている場合例外
        /// </summary>
        /// <param name="filePath">検証したいファイルへのフルパス</param>
        public static void HasFileLocked(string filePath) {
            FileStream stream = null;

            try {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            } catch {
                throw new Exception($"ファイルがロックされています：{filePath}");

            } finally {
                if (stream != null) {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// パスがディレクトリか判定する
        /// </summary>
        /// <param name="path"></param>
        public static bool IsDirectory(string path) {
            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory) {
                return true;
            }
            return false;
        }

    }
}
