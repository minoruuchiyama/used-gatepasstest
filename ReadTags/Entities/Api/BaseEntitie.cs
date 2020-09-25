using Newtonsoft.Json;
using System.Collections.Generic;

namespace ReadTags.Entities.Api
{
    /// <summary>
    /// API用Entitie共通処理
    /// </summary>
    class BaseEntitie{
        /// <summary>
        /// API処理の成功・失敗
        /// </summary>
        public enum Statuses { Failure, Success }

    }
}
