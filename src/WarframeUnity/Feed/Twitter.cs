using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;

namespace WarframeUnity.Feed
{
    public class Twitter
    {
        #region Fields
        private WebClient client;
        private string queryUrl = "http://search.twitter.com/search.json?q=WarframeAlerts&include_entities=false&result_type=recent";

        private DateTime lastQuery;
        private long maxId;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the largest tweet Id from the latest query.
        /// </summary>
        public long MaxId { get { return maxId; } }

        /// <summary>
        /// Gets the timestamp of the latest query.
        /// </summary>
        public DateTime LastQuery { get { return lastQuery; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Twitter instance to manage the warframe alert feed.
        /// </summary>
        /// <param name="queryInterval">The time between each query attempt.</param>
        public Twitter()
        {
            this.client = new WebClient();
            this.lastQuery = DateTime.MinValue;
            this.maxId = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Queries the warframe alert feed on twitter and returns the results in a string.
        /// </summary>
        private string Query()
        {
            string result = client.DownloadString(HttpUtility.UrlPathEncode(queryUrl + (maxId != -1 ? String.Format("&since_id={0}", maxId) : "")));
            if (!string.IsNullOrWhiteSpace(result))
                return result;

            return null;
        }

        public List<Tweet> FetchTweets()
        {
            string data = Query();
            if (string.IsNullOrWhiteSpace(data))
                return null;

            List<Tweet> fresh = new List<Tweet>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> parsed = serializer.Deserialize<Dictionary<string, object>>(data);

            long tempId;
            if (long.TryParse(parsed["max_id"].ToString(), out tempId) && tempId > maxId)
                maxId = tempId;

            if (parsed.ContainsKey("results"))
            {
                ArrayList results = parsed["results"] as ArrayList;
                foreach (object o in results)
                {
                    Dictionary<string, object> r = o as Dictionary<string, object>;
                    if (r.ContainsKey("from_user") && r["from_user"].ToString() == "WarframeAlerts")
                    {
                        if (r.ContainsKey("created_at") && r.ContainsKey("id") && r.ContainsKey("from_user_id") && r.ContainsKey("text"))
                        {
                            Tweet tweet = new Tweet();
                            tweet.Created = DateTime.Parse(r["created_at"].ToString());
                            tweet.Id = Convert.ToInt64(r["id"].ToString());
                            tweet.Author = r["from_user"].ToString();
                            tweet.AuthorId = Convert.ToInt64(r["from_user_id"].ToString());
                            tweet.Text = r["text"].ToString();
                            fresh.Add(tweet);
                        }
                    }
                }
            }

            if (fresh.Count > 0)
                return fresh;

            return null;
        }
        #endregion
    }
}
