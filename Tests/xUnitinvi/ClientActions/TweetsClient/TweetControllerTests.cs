﻿using System.Threading.Tasks;
using FakeItEasy;
using Tweetinvi.Controllers.Tweet;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Core.Web;
using Tweetinvi.Exceptions;
using Tweetinvi.Logic.DTO;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Parameters;
using Tweetinvi.WebLogic;
using Xunit;
using xUnitinvi.TestHelpers;

namespace xUnitinvi.ClientActions.TweetsClient
{
    public class TweetControllerTests
    {
        public TweetControllerTests()
        {
            _fakeBuilder = new FakeClassBuilder<TweetController>();
            _fakeTweetQueryExecutor = _fakeBuilder.GetFake<ITweetQueryExecutor>();

            var page1UserDTOs = new ITweetDTO[]
            {
                new TweetDTO { Id = 42 },
                new TweetDTO { Id = 43 }
            };

            var page2UserDTOs = new ITweetDTO[]
            {
                new TweetDTO { Id = 44 }
            };

            var jsonConverter = A.Fake<IJsonObjectConverter>();

            _firstResult = new TwitterResult<ITweetDTO[]>(A.Fake<IJsonObjectConverter>())
            {
                DataTransferObject = page1UserDTOs,
                Response = new TwitterResponse { Text = "json1" }
            };

            _secondResult = new TwitterResult<ITweetDTO[]>(A.Fake<IJsonObjectConverter>())
            {
                DataTransferObject = page2UserDTOs,
                Response = A.Fake<ITwitterResponse>()
            };

            A.CallTo(() => jsonConverter.DeserializeObject<ITweetDTO[]>(_firstResult.Response.Text, null)).Returns(page1UserDTOs);
            A.CallTo(() => jsonConverter.DeserializeObject<ITweetDTO[]>(_secondResult.Response.Text, null)).Returns(page2UserDTOs);
        }

        private readonly FakeClassBuilder<TweetController> _fakeBuilder;
        private readonly Fake<ITweetQueryExecutor> _fakeTweetQueryExecutor;
        private readonly TwitterResult<ITweetDTO[]> _firstResult;
        private readonly TwitterResult<ITweetDTO[]> _secondResult;

        private TweetController CreateTweetController()
        {
            return _fakeBuilder.GenerateClass();
        }

        [Fact]
        public async Task GetFavoriteTweets_MoveToNextPage_ReturnsAllPages()
        {
            // arrange
            var parameters = new GetFavoriteTweetsParameters("username") { PageSize = 2 };
            var request = A.Fake<ITwitterRequest>();

            _fakeTweetQueryExecutor
                .CallsTo(x => x.GetFavoriteTweets(It.IsAny<IGetFavoriteTweetsParameters>(), It.IsAny<ITwitterRequest>()))
                .ReturnsNextFromSequence(_firstResult, _secondResult);

            var controller = CreateTweetController();
            var friendIdsIterator = controller.GetFavoriteTweets(parameters, request);

            // act
            var page1 = await friendIdsIterator.MoveToNextPage();
            var page2 = await friendIdsIterator.MoveToNextPage();

            // assert
            Assert.Equal(page1.Content, _firstResult);
            Assert.False(page1.IsLastPage);

            Assert.Equal(page2.Content, _secondResult);
            Assert.True(page2.IsLastPage);
        }

        [Fact]
        public async Task GetFavoriteTweets_MoveToNextPage_ThrowsIfCompleted()
        {
            // arrange
            var parameters = new GetFavoriteTweetsParameters("username") { PageSize = 2 };
            var request = A.Fake<ITwitterRequest>();

            _fakeTweetQueryExecutor
                .CallsTo(x => x.GetFavoriteTweets(It.IsAny<IGetFavoriteTweetsParameters>(), It.IsAny<ITwitterRequest>()))
                .ReturnsNextFromSequence(_firstResult, _secondResult);

            var controller = CreateTweetController();
            var friendIdsIterator = controller.GetFavoriteTweets(parameters, request);

            await friendIdsIterator.MoveToNextPage();
            await friendIdsIterator.MoveToNextPage();

            // act
            await Assert.ThrowsAsync<TwitterIteratorAlreadyCompletedException>(() => friendIdsIterator.MoveToNextPage());
        }
    }
}