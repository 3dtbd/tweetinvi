﻿using System;
using System.Collections.Generic;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using HttpMethod = Tweetinvi.Models.HttpMethod;

namespace Tweetinvi
{
    public class TwitterQuery : TwitterRequestParameters, ITwitterQuery
    {
        public TwitterQuery()
        {
            _timeout = TimeSpan.FromSeconds(10);

            AcceptHeaders = new List<string>
            {
                "image/jpeg",
                "application/json"
            };

            HttpMethod = HttpMethod.GET;
            CustomHeaders = new Dictionary<string, string>();
        }

        public TwitterQuery(string queryURL, HttpMethod httpMethod) : this()
        {
            Url = queryURL;
            HttpMethod = httpMethod;
        }

        public IProxyConfig ProxyConfig { get; set; }

        private TimeSpan _timeout;
        public TimeSpan Timeout
        {
            get { return _timeout; }
            set
            {
                if ((int)value.TotalMilliseconds == 0) // Default
                {
                    _timeout = TimeSpan.FromSeconds(10);
                    return;
                }

                if (value.TotalMilliseconds < 0) // Infinite
                {
                    _timeout = TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite);
                    return;
                }

                _timeout = value;
            }
        }

        public ITwitterCredentials TwitterCredentials { get; set; }
        public IEnumerable<IOAuthQueryParameter> QueryParameters { get; set; }

        public IEndpointRateLimit QueryRateLimit { get; set; }
        public ICredentialsRateLimits CredentialsRateLimits { get; set; }

        /// <summary>
        /// Date at which the Twitter query will be ready to be executed
        /// </summary>
        public DateTime? DateWhenCredentialsWillHaveTheRequiredRateLimits { get; set; }

        public int? TimeToWaitBeforeExecutingTheQueryInMilliSeconds
        {
            get
            {
                if (DateWhenCredentialsWillHaveTheRequiredRateLimits == null)
                {
                    return null;
                }

                var timeToWait = DateWhenCredentialsWillHaveTheRequiredRateLimits.Value.Subtract(DateTime.Now).TotalMilliseconds;
                return (int)Math.Max(0, timeToWait);
            }
        }

        public IMultipartHttpRequest MultipartHttpRequest { get; set; }

        public ITwitterQuery Clone()
        {
            var clone = new TwitterQuery(Url, HttpMethod)
            {
                TwitterCredentials = TwitterCredentials,
                QueryParameters = QueryParameters,
                ProxyConfig = ProxyConfig
            };

            return clone;
        }

        public override string ToString()
        {
            return Url;
        }
    }
}