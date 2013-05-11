using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WarframeUnity.Feed
{
    public class Tweet
    {
        #region Fields
        public DateTime Created;
        public long Id;
        public string Author;
        public long AuthorId;
        public string Text;
        #endregion

        #region Constructors
        public Tweet()
        {

        }
        #endregion

        #region Methods
        //public static Tweet Parse(Dictionary<string, object> data)
        //{
        //    if (data.ContainsKey("created_at") && data.ContainsKey("id") && data.ContainsKey("from_user") && data.ContainsKey("from_user_id") && data.ContainsKey("text"))
        //    {
        //        Tweet newTweet = new Tweet();
        //        newTweet.created = DateTime.Parse(data["created_at"].ToString());
                
        //    }
        //    return null;
        //}
        #endregion
    }
}
