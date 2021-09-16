using System.Linq;
using FluentAssertions;
using ServerlessPatterns.TestableLambda.ApiFunction;
using Xunit;

namespace Test.ServerlessPatterns.TestableLambda.ApiFunction.LogicTests {

    public class ConvertMarkdownToHtml : _Init {

        //--- Methods ---

        [Fact]
        public void Convert_valid_markdown() {

            // arrange
            var logic = new Logic(Provider);

            // act
            var html = logic.ConvertMarkdownToHtml("*Hello World*");

            // assert
            html.Should().Be("<p><em>Hello World</em></p>\n");
        }
    }
}
