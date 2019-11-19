﻿using Tweetinvi.Core.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Tweetinvi.Logic.QueryParameters
{
    public class TwitterListUpdateQueryParameters : ITwitterListUpdateQueryParameters
    {
        public TwitterListUpdateQueryParameters(ITwitterListIdentifier listIdentifier, ITwitterListUpdateParameters parameters)
        {
            TwitterListIdentifier = listIdentifier;
            Parameters = parameters;
        }

        public ITwitterListIdentifier TwitterListIdentifier { get; }
        public ITwitterListUpdateParameters Parameters { get; }
    }
}