using System.Threading.Tasks;
using FluentAssertions;
using ServerlessPatterns.TestableLambda.ApiFunction;
using Xunit;

namespace Test.ServerlessPatterns.TestableLambda.ApiFunction.LogicTests {

    public class ListPostsAsync : _Init {

        //--- Methods ---

        [Fact]
        public async Task List_all_posts() {

            // arrange
            var logic = new Logic(Provider);
            await logic.CreatePostAsync("*First Post*");
            await logic.CreatePostAsync("*Second Post*");

            // act
            var recordList = await logic.ListPostsAsync(10);

            // assert
            recordList.Should().HaveCount(2);
            recordList.Should().HaveElementAt(0, ("1", TestProvider.DefaultUtcNow));
            recordList.Should().HaveElementAt(1, ("2", TestProvider.DefaultUtcNow));
        }
    }
}
