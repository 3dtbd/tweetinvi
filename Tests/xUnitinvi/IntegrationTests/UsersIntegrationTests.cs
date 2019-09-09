﻿using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Xunit;
using Xunit.Abstractions;

namespace xUnitinvi.IntegrationTests
{
    public class UserIntegrationTests
    {
        private readonly ITestOutputHelper _logger;

        public UserIntegrationTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        //[Fact]
        [Fact(Skip = "IntegrationTests")]
        public async Task TestUsers()
        {
            TweetinviEvents.QueryBeforeExecute += (sender, args) =>
            {
                _logger.WriteLine(args.Url);
            };

            var credentials = new TwitterCredentials("A", "B", "C", "D");

            var client = new TwitterClient(credentials);

            // act
            var authenticatedUser = await client.Users.GetAuthenticatedUser();
            var tweetinviUser = await client.Users.GetUser("tweetinviapi");
            var friendIdsIterator = await client.Users.GetFriendIds("tweetinviapi");
            var friends = await client.Users.GetUsers(friendIdsIterator.Items);
            var tweetinviFriends = (await tweetinviUser.GetFriends()).Items;
            var followers = (await authenticatedUser.GetFollowers()).Items;

            var user = await client.Users.GetUser("artwolkt");
            var blockSuccess = await user.BlockUser();
            var blockedUsers = await client.Users.GetBlockedUserIds();
            var unblockSuccess = await user.UnBlockUser();

            // assert
            Assert.Equal(tweetinviUser.Id, 1577389800);
            Assert.NotNull(authenticatedUser);
            Assert.Contains(1693649419, friendIdsIterator.Items);
            Assert.Contains(friends, (item) => { return item.ScreenName == "tweetinvitest"; });
            Assert.Contains(followers, (item) => { return item.ScreenName == "tweetinvitest"; });
            Assert.Equal(friends.Select(x => x.ToString()), tweetinviFriends.Select(x => x.ToString()));

            Assert.True(blockSuccess);
            Assert.Contains(blockedUsers.Items, item => item == user.Id);
            Assert.True(unblockSuccess);
        }
    }
}