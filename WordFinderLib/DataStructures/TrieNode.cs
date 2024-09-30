namespace WordFinderLib.DataStructures
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public bool IsWord { get; set; }
        public string? Word { get; set; }
    }
}
