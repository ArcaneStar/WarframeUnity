using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarframeUnity.Feed;

namespace WarframeUnity
{
    public class Alerts
    {
        #region Fields
        private Twitter twitter;
        private List<Alert> list;
        private DateTime lastCheck;
        private int refreshInterval;

        public delegate void HandleAlert();
        public event HandleAlert OnAlert;
        #endregion

        #region Properties
        public List<Alert> List { get { return list; } }
        public int RefreshInterval
        {
            get { return refreshInterval; }
            set { refreshInterval = value; }
        }
        #endregion

        #region Constructors
        public Alerts()
        {
            twitter = new Twitter();
            list = new List<Alert>();
            lastCheck = DateTime.MinValue;
            refreshInterval = 10;
        }
        #endregion

        #region Methods
        public void Update()
        {
            if ((DateTime.Now - lastCheck).TotalSeconds > refreshInterval)
            {
                lastCheck = DateTime.Now;

                List<Tweet> fresh = twitter.FetchTweets();
                if (fresh != null && fresh.Count > 0)
                {
                    foreach (Tweet t in fresh)
                    {
                        Alert newAlert = new Alert(t.Text);
                        // TODO
                        list.Add(newAlert);
                    }

                    if (OnAlert != null)
                        OnAlert.Invoke();
                }
            }
        }
        #endregion
    }
}
