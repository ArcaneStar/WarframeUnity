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
    /// <summary>
    /// Holds the information of a parsed tweet.
    /// </summary>
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
        public Tweet() { }
        #endregion
    }

    /// <summary>
    /// Manages connection to twitter and fetching tweets.
    /// </summary>
    public class Twitter
    {
        #region Fields
        private WebClient client;
        private JavaScriptSerializer serializer;
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
        /// Gets the time of the latest query.
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
            client = new WebClient();
            serializer = new JavaScriptSerializer();
            lastQuery = DateTime.MinValue;
            maxId = FetchMaxId();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the id of the most recent tweet.
        /// </summary>
        public long FetchMaxId()
        {
            long tempId = -1;
            string data = client.DownloadString(queryUrl + "&rpp=1");
            if (!string.IsNullOrWhiteSpace(data))
            {
                Dictionary<string, object> query = serializer.Deserialize<Dictionary<string, object>>(data);
                long.TryParse(query["max_id"].ToString(), out tempId);
            }
            return tempId;
        }

        /// <summary>
        /// Checks for new tweets and returns them in a list if any are found.
        /// </summary>
        /// <param name="fetchAll">Specifies whether all tweets should be collected or just unread tweets.</param>
        /// <param name="limit">Specifies the maximum amount of tweets that will be collected.</param>
        public List<Tweet> FetchTweets(bool fetchAll, int limit = 15)
        {
            string url = String.Format("{0}&rpp={1}", queryUrl, limit); 
            if (fetchAll && maxId != -1)
                url += "&since_id=" + maxId.ToString();

            string data = client.DownloadString(HttpUtility.UrlPathEncode(url));
            if (string.IsNullOrWhiteSpace(data))
                return null;

            List<Tweet> fresh = new List<Tweet>();
            Dictionary<string, object> parsed = serializer.Deserialize<Dictionary<string, object>>(data);

            long tempId;
            if (parsed.ContainsKey("max_id") && long.TryParse(parsed["max_id"].ToString(), out tempId) && tempId > maxId)
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
            lastQuery = DateTime.Now;
            return fresh;
        }
        #endregion
    }
}
