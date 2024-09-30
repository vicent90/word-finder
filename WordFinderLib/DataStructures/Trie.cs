namespace WordFinderLib.DataStructures
{
    public class Trie
    {
        public TrieNode Root { get; }

        public Trie()
        {
            Root = new TrieNode();
        }

        public void Insert(string word)
        {
            var node = Root;
            foreach (var ch in word)
            {
                if (!node.Children.TryGetValue(ch, out TrieNode? value))
                {
                    value = new TrieNode();
                    node.Children[ch] = value;
                }
                node = value;
            }
            node.IsWord = true;
            node.Word = word;
        }
    }
}
