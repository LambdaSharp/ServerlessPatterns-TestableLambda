using System.Threading.Tasks;
using FluentAssertions;
using ServerlessPatterns.TestableLambda.ApiFunction;
using Xunit;

namespace Test.ServerlessPatterns.TestableLambda.ApiFunction.LogicTests {

    public class CreatePostAsync : _Init {

        //--- Methods ---

        [Fact]
        public async Task Create_valid_record() {

            // arrange
            var logic = new Logic(Provider);

            // act
            var record = await logic.CreatePostAsync("*Hello World*");

            // assert
            record.Id.Should().Be("1");
            record.DateTime.Should().Be(TestProvider.DefaultUtcNow);
            record.Markdown.Should().Be("*Hello World*");
        }
    }
}
