﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Client;
using Tweetinvi.Core.Events;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Core.Factories;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Core.Streaming;
using Tweetinvi.Core.Wrappers;
using Tweetinvi.Events;
using Tweetinvi.Models;
using Tweetinvi.Models.Interfaces;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming;
using Tweetinvi.Streams.Properties;

namespace Tweetinvi.Streams
{
    public class TrackedStream : TwitterStream, ITrackedStream
    {
        public event EventHandler<MatchedTweetReceivedEventArgs> TweetReceived;
        public event EventHandler<MatchedTweetReceivedEventArgs> MatchingTweetReceived;
        public event EventHandler<TweetEventArgs> NonMatchingTweetReceived;

        protected readonly IStreamTrackManager<ITweet> _streamTrackManager;
        protected readonly ITweetFactory _tweetFactory;

        private readonly ITwitterQueryFactory _twitterQueryFactory;

        public override event EventHandler<JsonObjectEventArgs> JsonObjectReceived;

        public TrackedStream(
            IStreamTrackManager<ITweet> streamTrackManager,
            IJsonObjectConverter jsonObjectConverter,
            IJObjectStaticWrapper jObjectStaticWrapper,
            IStreamResultGenerator streamResultGenerator,
            ITweetFactory tweetFactory,
            ISynchronousInvoker synchronousInvoker,
            ICustomRequestParameters customRequestParameters,
            ITwitterQueryFactory twitterQueryFactory,
            ISingleAggregateExceptionThrower singleAggregateExceptionThrower)

            : base(streamResultGenerator, jsonObjectConverter, jObjectStaticWrapper, customRequestParameters)
        {
            _streamTrackManager = streamTrackManager;
            _tweetFactory = tweetFactory;
            _twitterQueryFactory = twitterQueryFactory;
        }

        public async Task StartStreamAsync(string url)
        {
            Func<ITwitterRequest> generateTwitterRequest = delegate
            {
                var queryBuilder = new StringBuilder(url);
                AddBaseParametersToQuery(queryBuilder);

                return new TwitterRequest
                {
                    Query = _twitterQueryFactory.Create(queryBuilder.ToString(), HttpMethod.GET, Credentials)
                };
            };

            Action<string> generateTweetDelegate = json =>
            {
                RaiseJsonObjectReceived(json);

                var tweet = _tweetFactory.GenerateTweetFromJson(json, TweetMode, null);
                if (tweet == null)
                {
                    TryInvokeGlobalStreamMessages(json);
                    return;
                }

                var detectedTracksAndActions = _streamTrackManager.GetMatchingTracksAndActions(tweet.FullText);
                var detectedTracks = detectedTracksAndActions.Select(x => x.Item1);

                var eventArgs = new MatchedTweetReceivedEventArgs(tweet, json)
                {
                    MatchingTracks = detectedTracks.ToArray(),
                };

                if (detectedTracksAndActions.Any())
                {
                    eventArgs.MatchOn = MatchOn.TweetText;

                    RaiseTweetReceived(eventArgs);
                    RaiseMatchingTweetReceived(eventArgs);
                }
                else
                {
                    RaiseTweetReceived(eventArgs);
                    RaiseNonMatchingTweetReceived(new TweetEventArgs(tweet, json));
                }
            };

            await _streamResultGenerator.StartStreamAsync(generateTweetDelegate, generateTwitterRequest);
        }

        protected void RaiseJsonObjectReceived(string json)
        {
            this.Raise(JsonObjectReceived, new JsonObjectEventArgs(json));
        }

        public int TracksCount
        {
            get { return _streamTrackManager.TracksCount; }
        }

        public int MaxTracks
        {
            get { return _streamTrackManager.MaxTracks; }
        }

        public Dictionary<string, Action<ITweet>> Tracks
        {
            get { return _streamTrackManager.Tracks; }
        }

        public void AddTrack(string track, Action<ITweet> trackReceived = null)
        {
            if (_streamResultGenerator.StreamState != StreamState.Stop)
            {
                throw new InvalidOperationException(Resources.TrackedStream_ModifyTracks_NotStoppedException_Description);
            }

            _streamTrackManager.AddTrack(track, trackReceived);
        }

        public void RemoveTrack(string track)
        {
            if (_streamResultGenerator.StreamState != StreamState.Stop)
            {
                throw new InvalidOperationException(Resources.TrackedStream_ModifyTracks_NotStoppedException_Description);
            }

            _streamTrackManager.RemoveTrack(track);
        }

        public bool ContainsTrack(string track)
        {
            return _streamTrackManager.ContainsTrack(track);
        }

        public void ClearTracks()
        {
            if (_streamResultGenerator.StreamState != StreamState.Stop)
            {
                throw new InvalidOperationException(Resources.TrackedStream_ModifyTracks_NotStoppedException_Description);
            }

            _streamTrackManager.ClearTracks();
        }

        protected void RaiseTweetReceived(MatchedTweetReceivedEventArgs eventArgs)
        {
            this.Raise(TweetReceived, eventArgs);
        }

        protected void RaiseMatchingTweetReceived(MatchedTweetReceivedEventArgs eventArgs)
        {
            this.Raise(MatchingTweetReceived, eventArgs);
        }

        protected void RaiseNonMatchingTweetReceived(TweetEventArgs eventArgs)
        {
            this.Raise(NonMatchingTweetReceived, eventArgs);
        }
    }
}