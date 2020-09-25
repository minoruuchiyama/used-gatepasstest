using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ReadTags.Entities.Api;
using ReadTags.Exceptions;

namespace ReadTags.Models {
    /// <summary>
    /// WebServerModel
    /// </summary>
    public static class WebServerModel {

        /// <summary>
        /// リソースに対するクライアントの要求を表します
        /// </summary>
        public static HttpListenerRequest Request { set; get; }
        public static HttpListenerRequest Request1 { set; get; }

        /// <summary>
        /// クライアントの要求に対する応答で、クライアントに送信されるオブジェクト
        /// </summary>
        public static HttpListenerResponse Response { set; get; }
        public static HttpListenerResponse Response1 { set; get; }

        /// <summary>
        /// Response body
        /// </summary>
        public static byte[] Content { set; get; }
        public static byte[] Content1 { set; get; }
        public static Task<bool> task { get; private set; }

        /// <summary>
        /// サーバーデーモン
        /// </summary>
        public static async void ServerStartAsync() {

            //HTTPリスナーの作成
            HttpListener listener = new HttpListener();

            //待ち受けURL・ポートの設定
            listener.Prefixes.Add($"{Properties.Settings.Default.Protocol}://{Properties.Settings.Default.HostName}:{Properties.Settings.Default.Port}/");

            //HTTPリスナー受信準備
            listener.Start();
            listener.IgnoreWriteExceptions = true;

            await Task.Run(() => {
                while (true) {
                    try {
                        //受信要求を待機し、受信するとその要求を返します
                        HttpListenerContext context = listener.GetContext();
                        Request = context.Request;
                        Response = context.Response;

                        Response.AppendHeader("Access-Control-Allow-Origin", "*");
                         
                        string modelName = "ReadTags.Models.ServerSideModels.ReadTagsModel";
                        Type type = Type.GetType(modelName);

//                        string modelName1 = "ReadTags.Models.ServerSideModels.ReadTagsWaitModel";
//                        Type type1 = Type.GetType(modelName1);

                        //サーバーサイド処理モデルクラスが存在しない場合
                        if (type == null) {
                            throw new Http404NotFoundException("URLが存在しません");
                        }

                        object instance = Activator.CreateInstance(type, new object[] { Request, Response});
//                        object instance1 = Activator.CreateInstance(type1, new object[] { Request1, Response1 });

                        //サーバーサイド処理実行
                        try
                        {
                            Type.GetType(modelName).GetMethod("Handle").Invoke(instance, new object[] { });
                            Content = (byte[])Type.GetType(modelName).GetProperty("Content").GetValue(instance);

                            //                            if (Content == null)
                            //                            {
//                            Task<bool> task = Task.Run(() => {
                            //Task.Run(() =>
//                                {
//                                    Type.GetType(modelName1).GetMethod("Handle").Invoke(instance1, new object[] { });
//                                    Content1 = (byte[])Type.GetType(modelName1).GetProperty("Content1").GetValue(instance1);
//                                    if (Content1 != null) {
//                                        Response1.OutputStream.Write(Content1, 0, Content1.Length);
//                                        Content1 = null;
//                                        Response1.Close();
//                                        return true;    
//                                    }

//                                return false;    
//                            });
                            //                            }
//                            bool ans = task.Result;
//                            if (ans) {
//                                Response.OutputStream.Write(Content1, 0, Content1.Length);
//                            }


                        }
                        catch (TargetInvocationException ex) {
                            throw ex.InnerException;
                        }

                    } catch (Http400BadRequestException ex) {
                        Response.StatusCode = 400;
                        CreateErrorResponse(ex.Message);

                    } catch (Http403ForbiddenException ex) {
                        Response.StatusCode = 403;
                        CreateErrorResponse(ex.Message);

                    } catch (Http404NotFoundException ex) {
                        Response.StatusCode = 404;
                        CreateErrorResponse(ex.Message);

                    } catch (Exception ex) {
                        Response.StatusCode = 500;
                        CreateErrorResponse(ex.Message);
                    }

                    try {

//                        if (Content1 != null) {
//                            Response.OutputStream.Write(Content, 0, Content.Length);
//                            Content1 = null;
//                        }
//                        if (Content != null) {
                            Response.OutputStream.Write(Content, 0, Content.Length);
//                            Content = null;
//                        }

                    }
                    catch (Exception ex) {
                        MessageBox.Show("エラーが発生しました。",
                            "返却ストリーム書き込みエラー",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information,
                            MessageBoxResult.OK,
                            MessageBoxOptions.DefaultDesktopOnly
                        );
                    }


                    try {
                        Response.Close();
//                        Response1.Close();

                    } catch (Exception ex) {
                        //何もしない
                    }
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// エラー用レスポンスの作成
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private static void CreateErrorResponse(string message) {
            var errorResponseEntitie = new ErrorResponseEntitie {
                Message = message.Trim(),
            };
            string json = JsonConvert.SerializeObject(errorResponseEntitie);
            Content = Encoding.UTF8.GetBytes(json);
            Response.AppendHeader("Content-Type", "application/json");
        }
    }
}
