﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi.Core.Controllers;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Tweetinvi
{
    /// <summary>
    /// Access Twitter the user timelines.
    /// </summary>
    public static class Timeline
    {
        [ThreadStatic]
        private static ITimelineController _timelineController;

        /// <summary>
        /// Controller handling any Timeline request
        /// </summary>
        public static ITimelineController TimelineController
        {
            get
            {
                if (_timelineController == null)
                {
                    Initialize();
                }

                return _timelineController;
            }
        }

        private static IFactory<IGetHomeTimelineParameters> _homeTimelineParameterFactory;
        private static IFactory<IUserTimelineParameters> _userTimelineParameterFactory;
        private static IFactory<IMentionsTimelineParameters> _mentionsTimelineParameterFactory;
        private static IFactory<IGetRetweetsOfMeTimelineParameters> _retweetsOfMeTimelineParameterFactory;

        static Timeline()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _timelineController = TweetinviContainer.Resolve<ITimelineController>();
            _homeTimelineParameterFactory = TweetinviContainer.Resolve<IFactory<IGetHomeTimelineParameters>>();
            _userTimelineParameterFactory = TweetinviContainer.Resolve<IFactory<IUserTimelineParameters>>();
            _mentionsTimelineParameterFactory = TweetinviContainer.Resolve<IFactory<IMentionsTimelineParameters>>();
            _retweetsOfMeTimelineParameterFactory = TweetinviContainer.Resolve<IFactory<IGetRetweetsOfMeTimelineParameters>>();
        }

        // Parameter generator

        /// <summary>
        /// Parameters available to refine the results from the Home Timeline
        /// </summary>
        public static IGetHomeTimelineParameters CreateHomeTimelineParameter()
        {
            return _homeTimelineParameterFactory.Create();
        }

        /// <summary>
        /// Parameters available to refine the results from a User Timeline
        /// </summary>
        public static IUserTimelineParameters CreateUserTimelineParameter()
        {
            return _userTimelineParameterFactory.Create();
        }

        /// <summary>
        /// Parameters available to refine the results from the Mentions Timeline
        /// </summary>
        public static IMentionsTimelineParameters CreateMentionsTimelineParameters()
        {
            return _mentionsTimelineParameterFactory.Create();
        }

        /// <summary>
        /// Parameters available to refine the results from the Retweets of Me Timeline
        /// </summary>
        public static IGetRetweetsOfMeTimelineParameters CreateRetweetsOfMeTimelineParameters()
        {
            return _retweetsOfMeTimelineParameterFactory.Create();
        }

        // User Timeline

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(IUserIdentifier user, int maximumTweets = 40)
        {
            return TimelineController.GetUserTimeline(user, maximumTweets);
        }

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(long userId, int maximumTweets = 40)
        {
            return TimelineController.GetUserTimeline(userId, maximumTweets);
        }

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(string userScreenName, int maximumTweets = 40)
        {
            return TimelineController.GetUserTimeline(userScreenName, maximumTweets);
        }

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(long userId, IUserTimelineParameters userTimelineParameters)
        {
            return TimelineController.GetUserTimeline(userId, userTimelineParameters);
        }

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(string userScreenName, IUserTimelineParameters userTimelineParameters)
        {
            return TimelineController.GetUserTimeline(userScreenName, userTimelineParameters);
        }

        /// <summary>
        /// Get the tweets visible on the specified user Timeline
        /// </summary>
        public static Task<IEnumerable<ITweet>> GetUserTimeline(IUserIdentifier user, IUserTimelineParameters userTimelineParameters)
        {
            return TimelineController.GetUserTimeline(user, userTimelineParameters);
        }

        // Mention Timeline

        /// <summary>
        /// Get the tweets visible on your mentions timeline
        /// </summary>
        public static Task<IEnumerable<IMention>> GetMentionsTimeline(int maximumTweets = 40)
        {
            return TimelineController.GetMentionsTimeline(maximumTweets);
        }

        /// <summary>
        /// Get the tweets visible on your mentions timeline
        /// </summary>
        public static Task<IEnumerable<IMention>> GetMentionsTimeline(IMentionsTimelineParameters mentionsTimelineParameters)
        {
            return TimelineController.GetMentionsTimeline(mentionsTimelineParameters);
        }
    }
}