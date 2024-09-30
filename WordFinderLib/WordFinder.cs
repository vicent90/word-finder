using System.Collections.Concurrent;
using WordFinderLib.DataStructures;

namespace WordFinderLib
{
    public class WordFinder
    {
        private readonly char[][] _matrix;
        private readonly int _rows;
        private readonly int _cols;

        public WordFinder(IEnumerable<string> matrix)
        {
            ArgumentNullException.ThrowIfNull(matrix);
            _matrix = matrix.Select(row => row.ToCharArray()).ToArray();

            if (_matrix.Length == 0)
                throw new ArgumentException("Matrix cannot be empty.");

            _rows = _matrix.Length;
            _cols = _matrix[0].Length;

            if (_rows > 64 || _cols > 64)
                throw new ArgumentException("Matrix dimensions cannot exceed 64x64.");

            foreach (var row in _matrix)
            {
                if (row.Length != _cols)
                    throw new ArgumentException("All rows in the matrix must have the same number of characters.");
            }
        }

        public async Task<List<string>> FindAsync(IEnumerable<string> wordstream, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(wordstream);

            var foundWords = new HashSet<string>();
            var wordCounts = new ConcurrentDictionary<string, int>();

            await Task.Run(() =>
            {
                var trie = new Trie();

                foreach (var word in wordstream)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (!foundWords.Contains(word) && word.Length <= _cols)
                    {
                        trie.Insert(word);
                        foundWords.Add(word);
                    }
                }

                Parallel.For(0, _rows, (row, state) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Stop();
                        return;
                    }

                    ScanLine(_matrix[row], trie, wordCounts, cancellationToken);
                });

                Parallel.For(0, _cols, (col, state) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        state.Stop();
                        return;
                    }

                    var column = new char[_rows];
                    for (int row = 0; row < _rows; row++)
                    {
                        column[row] = _matrix[row][col];
                    }
                    ScanLine(column, trie, wordCounts, cancellationToken);
                });
            }, cancellationToken);

            return wordCounts.OrderByDescending(kvp => kvp.Value)
                             .ThenBy(kvp => kvp.Key)
                             .Take(10)
                             .Select(kvp => kvp.Key)
                             .ToList();
        }

        private static void ScanLine(char[] line, Trie trie, ConcurrentDictionary<string, int> wordCounts, CancellationToken cancellationToken)
        {
            int length = line.Length;

            for (int start = 0; start < length; start++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var node = trie.Root;
                int index = start;

                while (index < length && node.Children.TryGetValue(line[index], out node))
                {
                    if (node.IsWord)
                    {
                        var word = node.Word;
                        wordCounts.AddOrUpdate(word!, 1, (key, value) => value + 1);
                    }
                    index++;
                }
            }
        }
    }
}
