using System;
using FluentAssertions;
using Xunit;
using AStar.Dev.OneDrive.Client.Models;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public class GraphFileTests
    {
        [Fact]
        public void SizeText_Shows_Bytes_When_SizeSet()
        {
            var gf = new GraphFile { Size = 1234 };
            gf.SizeText.Should().Be("1234 bytes");
        }

        [Fact]
        public void SizeText_Empty_When_NoSize()
        {
            var gf = new GraphFile { Size = null };
            gf.SizeText.Should().BeEmpty();
        }

        [Fact]
        public void LastModifiedText_Formats_When_Set()
        {
            var dt = new DateTimeOffset(2020,1,2,3,4,5, TimeSpan.Zero);
            var gf = new GraphFile { LastModified = dt };
            gf.LastModifiedText.Should().Be(dt.ToString("g"));
        }

        [Fact]
        public void LastModifiedText_Empty_When_Null()
        {
            var gf = new GraphFile { LastModified = null };
            gf.LastModifiedText.Should().BeEmpty();
        }
    }
}
