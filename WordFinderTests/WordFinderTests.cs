using WordFinderLib;

namespace WordFinderTests
{
    public class WordFinderTests
    {
        [Fact]
        public async Task Find_NoWordsFound_ReturnsEmpty()
        {
            var matrix = new List<string>
            {
                "abcd",
                "efgh",
                "ijkl",
                "mnop"
            };
            var wordFinder = new WordFinder(matrix);
            var wordstream = new List<string> { "xyz", "hello", "world" };

            var result = await wordFinder.FindAsync(wordstream, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task Find_SomeWordsFound_ReturnsCorrectWords()
        {
            var matrix = new List<string>
            {
                "abcd",
                "efgh",
                "ijkl",
                "mnop"
            };
            var wordFinder = new WordFinder(matrix);
            var wordstream = new List<string> { "abc", "efg", "mnop", "gh", "nop" };

            var result = await wordFinder.FindAsync(wordstream, CancellationToken.None);

            Assert.Equal(5, result.Count);
            Assert.Contains("abc", result);
            Assert.Contains("efg", result);
            Assert.Contains("mnop", result);
            Assert.Contains("nop", result);
        }

        [Fact]
        public async Task Find_WordsFoundMultipleTimes_ReturnsTop10()
        {
            var matrix = new List<string>
            {
                "abcabcabcabc",
                "defdefdefdef",
                "ghighighighi",
                "jkljkljkljkl"
            };
            var wordFinder = new WordFinder(matrix);
            var wordstream = new List<string> { "abc", "def", "ghi", "jkl", "abcd", "defg", "ghij" };

            var result = await wordFinder.FindAsync(wordstream, CancellationToken.None);

            Assert.Equal(4, result.Count);
            Assert.Contains("abc", result);
            Assert.Contains("def", result);
            Assert.Contains("ghi", result);
            Assert.Contains("jkl", result);
        }

        [Fact]
        public async Task Find_MoreThanTenWordsFound_ReturnsTopTen()
        {
            var matrix = new List<string>
            {
                "abcdefghij",
                "klmnopqrst",
                "uvwxyzabcd",
                "efghijklmn",
                "opqrstuvwx",
                "yzabcdefgh"
            };
            var wordFinder = new WordFinder(matrix);
            var wordstream = new List<string>
            {
                "abc", "def", "ghi", "jkl", "mno",
                "pqr", "stu", "vwx", "yz", "klm",
                "nop", "qrs", "tuv", "wxy", "zab"
            };

            var result = await wordFinder.FindAsync(wordstream, CancellationToken.None);

            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void Constructor_InvalidMatrix_ThrowsException()
        {
            var invalidMatrix = new List<string>
            {
                "abcd",
                "efg",
                "hijk"
            };

            Assert.Throws<ArgumentException>(() => new WordFinder(invalidMatrix));
        }

        [Fact]
        public async Task Find_NullWordstream_ThrowsException()
        {
            var matrix = new List<string>
            {
                "abcd",
                "efgh",
                "ijkl",
                "mnop"
            };
            var wordFinder = new WordFinder(matrix);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await wordFinder.FindAsync(null, CancellationToken.None));
        }
    }
}
