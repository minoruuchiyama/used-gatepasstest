/*
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ReadTags.Entities.Api;
using ReadTags.Exceptions;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Documents;

namespace ReadTags.Models.ServerSideModels {
    /// <summary>
    /// ReadTags API 
    /// </summary>
    public class ReadTagsWaitModel : ServerSideModel{

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="req">リソースに対するクライアントの要求を表します</param>
        /// <param name="res">クライアントの要求に対する応答で、クライアントに送信されるオブジェクト</param>
        public ReadTagsWaitModel( HttpListenerRequest req, HttpListenerResponse res) {
            Request1  = req;
            Response1 = res;
        }

        public static bool PROCESSING = false;  // 処理中フラグ
        public static string ORICON_NO = "";    // 処理中のオリコンNo
//        public static DateTime DT_NOW = DateTime.Now;                   // 現在日時

        /// <summary>
        /// ハンドル
        /// </summary>
        public void Handle() {

            string msg = "";

            try
            {

                string sec = ReadTagsModel.SEC;


                // 待機秒数（ミリ秒）
                int waiting = (int)(float.Parse(sec) * 1000);
                TimeSpan ts = TimeSpan.FromMilliseconds(waiting);


                // 読み取り直後の時刻を保持
//                DT_NOW = DateTime.Now;
//Console.WriteLine("DT_NOW");
//Console.WriteLine(DT_NOW.ToString("hh:mm:ss.fff"));

                bool end = false;
                // 待機後 処理中フラグクリア
                Task<bool> task = Task.Run(() => {
                    PROCESSING = true;
                    // sec秒 待機
                    Thread.Sleep(waiting);

                    // 保持時点よりsec秒以上経過していれば処理中状態を終了
                    if (DateTime.Now >= (ReadTagsModel.DT_NOW + ts)) {
                        // 処理中状態を終了
                        PROCESSING = false;
                    }

Console.WriteLine("PROCESSING");
Console.WriteLine(PROCESSING);
                    if (!PROCESSING) {
                        // response

                        SendResult("End", "Read End");
//                        Content1 = (byte[])Type.GetType(modelName1).GetProperty("Content1").GetValue(instance1);
                        Response1.OutputStream.Write(Content1, 0, Content1.Length);
                        Response1.Close();
                    }
                    return PROCESSING;

                });

//               end = !task.Result;


                if (PROCESSING) {
                    // response
//                    SendResult("End", "Read End");
                } else {
//                    SendResult("OK", "Read OK");
                }

                return;
            }
            catch (Exception ex) {
                msg = "ERROR: " + ex.Message;
            }


        }

        /// <summary>
        /// JSON送信
        /// </summary>
        private void SendResult(string status = "", string msg = "")
        {
            ReadTagsResponseEntitie readTagsResponseEntitie = new ReadTagsResponseEntitie
            {
                Status = status,
                Message = msg,
                Renban = ReadTagsModel.RENBAN.ToString(),
                Tags = ReadTagsModel.TAGS_LIST.ToArray(),
            };

            string resContent = JsonConvert.SerializeObject(readTagsResponseEntitie);
            //Content = Encoding.ASCII.GetBytes(resContent);
            Content1 = Encoding.UTF8.GetBytes(resContent);

            Response1.StatusCode = 200;
            Response1.AppendHeader("Content-Type", "application/json");

        }

    }


}
*/