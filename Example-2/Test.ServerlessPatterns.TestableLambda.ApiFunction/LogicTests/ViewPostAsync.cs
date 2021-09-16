using System.Threading.Tasks;
using FluentAssertions;
using ServerlessPatterns.TestableLambda.ApiFunction;
using Xunit;

namespace Test.ServerlessPatterns.TestableLambda.ApiFunction.LogicTests {

    public class ViewPostAsync : _Init {

        //--- Methods ---

        [Fact]
        public async Task View_valid_post() {

            // arrange
            var logic = new Logic(Provider);
            await logic.CreatePostAsync("*Hello World*");

            // act
            var (record, html) = await logic.ViewPostAsync("1");

            // assert
            record.Id.Should().Be("1");
            record.DateTime.Should().Be(TestProvider.DefaultUtcNow);
            record.Markdown.Should().Be("*Hello World*");
            html.Should().Be("<p><em>Hello World</em></p>\n");
        }
    }
}
