namespace RepoManager
{
    public struct Author
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public override string ToString() => $"{Name} ({Email})";
    }
}
