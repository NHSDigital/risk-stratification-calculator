using FluentAssertions;
using NHSD.RiskStratification.Calculator.Coding;
using Xunit;

namespace NHSD.RiskStratification.Calculator.Tests.Coding
{
    public class PostCodeTests
    {
        [Theory]
        [InlineData("M1 1AA")]
        [InlineData("M60 1NW")]
        [InlineData("CR2 6XH")]
        [InlineData("DN55 1PT")]
        [InlineData("W1A 1HQ")]
        [InlineData("EC1A 1BB")]
        public void Can_parse_valid_postcodes(string s)
        {
            Postcode.TryParse(s, out _).Should().BeTrue();
        }

        [Theory]
        [InlineData("wrong12postcode")]
        public void Will_reject_invalid_postcodes(string s)
        {
            Postcode.TryParse(s, out _).Should().BeFalse();
        }

        [Fact]
        public void Will_not_allocate_new_string_if_format_already_canonical()
        {
            var s = "M60 1NW";

            Postcode.TryParse(s, out var postcode).Should().BeTrue();

            ReferenceEquals(s, postcode.ToString()).Should().BeTrue();
        }

        [Theory]
        [InlineData("M1   1AA ")]
        [InlineData(" M60 1NW")]
        [InlineData("CR2\t6XH")]
        [InlineData("\tDN55 1PT")]
        [InlineData("W1A\u00A01HQ")]
        [InlineData("\u2009EC1A1BB")]
        public void Will_remove_whitespace_from_valid_postcode(string s)
        {
            Postcode.TryParse(s, out _).Should().BeTrue();
        }

        [Theory]
        [InlineData("M1 1AA", "M")]
        [InlineData("M60 1NW", "M")]
        [InlineData("CR2 6XH", "CR")]
        [InlineData("DN55 1PT", "DN")]
        [InlineData("W1A 1HQ", "W")]
        [InlineData("EC1A 1BB", "EC")]
        public void Can_return_area_code(string s, string area)
        {
            Postcode.TryParse(s, out var postcode).Should().BeTrue();

            postcode.Area.Should().Be(area);
        }
    }
}
