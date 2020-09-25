using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using ReadTags.Entities.Api;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadTags.Models.ServerSideModels {
    /// <summary>
    /// ReadTags API 
    /// </summary>
    public class ReadTagsModel : ServerSideModel{

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="req">リソースに対するクライアントの要求を表します</param>
        /// <param name="res">クライアントの要求に対する応答で、クライアントに送信されるオブジェクト</param>
        public ReadTagsModel( HttpListenerRequest req, HttpListenerResponse res) {
            Request  = req;
            Response = res;
        }

        /// <summary>
        /// 読み取ったタグ保持List用
        /// </summary>
        public class TagsList {
            public string OriTag { get; set; }
            public string Tags { get; set; }
        }
        /// <summary>
        /// DBのタグ保持List用
        /// </summary>
        public class TagsDB {
            public string OriNo { get; set; }
            public string OriTag { get; set; }
            public string Tags { get; set; }
        }

        public static bool PROCESSING = false;                          // 処理中フラグ
        public static int RENBAN = 0;                                   // 通過連番
        public static string ORI_TAG = "";                              // 前回オリコンタグ
        public static DateTime DT_NOW = DateTime.Now;                   // 時間保持用
        public static List<TagsList> TAGS_LIST = new List<TagsList>();  // 読み取ったタグ保持用List
        public static List<TagsDB> TAGS_DB = new List<TagsDB>();        // DBのタグ保持用List

        // 一致List
        public List<string> MatchList = new List<string>();
        // 過剰List
        public List<string> OverList = new List<string>();
        // 過少List
        public List<string> LackList = new List<string>();


        /// <summary>
        /// ハンドル
        /// </summary>
        public void Handle() {

            string msg = "";
            bool flgFirst = false;

            try
            {

                //リクエストを読み込む為のストリーム作成
                StreamReader reader = new StreamReader(Request.InputStream);
                //リクエストデータを読み込み
                string str = reader.ReadToEnd();

                // パラメータ受取
                ReadTagsRequestEntitie query = JsonConvert.DeserializeObject<ReadTagsRequestEntitie>(str);
                string statuscheck = query.StatusCheck;
                string sec = query.Sec;
                string oriTag = query.OriTag;
                string tag = query.Tag;
                // ステータスチェック
                if (statuscheck == "1") {
                    // 状態を返して終了
                    if (PROCESSING) {
                        SendResult("Processing..");
                    } else {
                        SendResult("Ready");
                    }
                    return;
                }

                // 待機秒数が取得出来ない場合は"5"固定
                if (string.IsNullOrEmpty(sec)) {
                    sec = "5";
                }
                // 待機秒数（ミリ秒）
                int waiting = (int)(float.Parse(sec) * 1000);
                TimeSpan ts = TimeSpan.FromMilliseconds(waiting);

                // 処理中でない場合は初回読み取りとする
                if (!PROCESSING) {
                    flgFirst = true;
                }

                // 処理中フラグON
                PROCESSING = true;

                if (flgFirst) {
                    // 初回読み取り
                    bool ans = FirstRead();
                    // DB（ファイル）読み込み
                    TAGS_DB = new List<TagsDB>();
                    TAGS_DB = ReadDB();
                    ORI_TAG = "";
                }
                // オリコンタグが取得出来ない場合は前回オリコンタグ
                if (string.IsNullOrEmpty(oriTag)) {
                    oriTag = ORI_TAG;
                }
                // オリコンタグ保持
                ORI_TAG = oriTag;

                // 前回オリコンタグもない場合はエラー
                if (string.IsNullOrEmpty(oriTag)) {
                    SendResult("NG", "ERROR：パラメータなし");
                    return;
                }


                // タグ読み取り、マッチング
                ReadTag(oriTag, tag);

                // 待機後、処理中フラグクリア
//                Task<bool> task = Task.Run(() =>
                Task.Run(async () =>
                {
                    PROCESSING = true;
                    DT_NOW = DateTime.Now;
                    // sec秒 待機
//                    Thread.Sleep(waiting);
                    await Task.Delay(waiting).ConfigureAwait(true);
                    // 保持時点よりsec秒以上経過で処理中状態を終了
                    if (DateTime.Now >= DT_NOW + ts) {
                        PROCESSING = false;
                    }
                });

                //               end = !task.Result;

//OverList.ForEach(Console.WriteLine);
                // Response
                SendResult("Processing..", "Processed:  OriconTag: " + oriTag + ", Tag: " + tag);

            }
            catch (Exception ex) {
                msg = "ERROR: " + ex.Message;
                TAGS_LIST = new List<TagsList>();
                // Response
                SendResult("NG", msg);
            }

        }

        /// <summary>
        /// JSON送信
        /// </summary>
        private void SendResult(string status, string msg = "")
        {

            ReadTagsResponseEntitie readTagsResponseEntitie = new ReadTagsResponseEntitie
            {
                Status = status,
                Message = msg,
                Renban = RENBAN.ToString(),
                Match = MatchList.ToArray(),
                Over = OverList.ToArray(),
                Lack = LackList.ToArray(),

            };

            string resContent = JsonConvert.SerializeObject(readTagsResponseEntitie);
            //Content = Encoding.ASCII.GetBytes(resContent);
            Content = Encoding.UTF8.GetBytes(resContent);

            Response.StatusCode = 200;
            Response.AppendHeader("Content-Type", "application/json");

        }

        /// <summary>
        /// 初回読み取り
        /// </summary>
        private bool FirstRead() {

            // タグ保持list クリア
            TAGS_LIST = new List<TagsList>();
//            TAGS_DB = new List<TagsDB>();

            // 通過連番+1
            RENBAN++;

            return true;
        }

        /// <summary>
        /// タグ読み取り、一致チェック
        /// </summary>
        private bool ReadTag(string oriTag, string tag) {

            // タグ保持Listに追加            
            var s = new TagsList();
            s.OriTag = oriTag;
            s.Tags = tag;
            TAGS_LIST.Add(s);

            // ソート
            TAGS_LIST = TAGS_LIST.OrderBy(x => x.OriTag).ThenBy(x => x.Tags).ToList();


            //            var tags = TAGS_LIST
            //              .AsEnumerable().Select(x => x.Tags).ToArray();


            // マッチング
            // 過剰チェック
            foreach (TagsList taglist in TAGS_LIST) {

                // DB(List)に存在するか
                bool b = TAGS_DB
                        .Exists(x => x.OriTag == taglist.OriTag && x.Tags == taglist.Tags);
                if (!b) {
                    // 存在しない場合過剰
                    OverList.Add(taglist.Tags);
                }

            }
            // 過少チェック
            foreach (TagsDB dblist in TAGS_DB) {

                // 保持Tag(List)に存在するか
                bool ext = TAGS_LIST
                        .Exists(x => x.OriTag == dblist.OriTag && x.Tags == dblist.Tags);
                if (!ext) {
                    // 存在しない場合過少
                    LackList.Add(dblist.Tags);
                }

            }

            // 一致チェック

            // DBのオリコンタグのみ取得（グループ化）
            string[] dbOritags = TAGS_DB.GroupBy(x => x.OriTag).Select(x => x.Key).ToArray();

            bool matchFlg = false;

            // DBのオリコンタグ数分ループ
            foreach (string dbOri in dbOritags) {

                // 該当オリコンタグ分のみ取得
                var ori1List = TAGS_DB.Where(x => x.OriTag == dbOri).ToList();
                var ori2List = TAGS_LIST.Where(x => x.OriTag == dbOri).ToList();

                // カウントが同じ且つDBのタグが全てListに存在すれば一致と見なす
                int cnt = ori1List.Count();
                if (cnt > 0 && cnt == ori2List.Count()) {

                    matchFlg = true;
                    foreach (var ori in ori1List) {
                        // 保持Tag(List)存在チェック
                        bool ext = ori2List.Exists(x => x.OriTag == ori.OriTag && x.Tags == ori.Tags);
                        if (!ext) {
                            matchFlg = false;
                            break;
                        }
                    }
                    if (matchFlg) {
                        // 一致
                        MatchList.Add(dbOri);
                    }

                }

            }

            return true;

        }

        /// <summary>
        /// ファイル読み込み(DBの代わり)
        /// </summary>
        private List<TagsDB> ReadDB() {

            var tagsDbList = new List<TagsDB>();
            // ファイル読み込み
            //            using (StreamReader st = new StreamReader(@System.Environment.CurrentDirectory + "\\TagData1.csv")) {
            using (StreamReader st = new StreamReader(@"C:\works\readtags\TagData1.csv")) {
                string line;
                while ((line = st.ReadLine()) != null) {
                    // タブ区切り
                    string[] body = line.Split('\t');
                    // Listに格納
                    for (int i = 2; i < body.Length; i++) {
                        var s = new TagsDB();
                        s.OriNo = body[0];
                        s.OriTag = body[1];
                        s.Tags = body[i];
                        tagsDbList.Add(s);
                    }
                }
            }
            // ソート
            tagsDbList = tagsDbList.OrderBy(x => x.OriTag).ThenBy(x => x.Tags).ToList();
            return tagsDbList;

        }

    }



}
