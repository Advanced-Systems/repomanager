namespace RepoManager
{
    public struct Remote
    {
        public string Push { get; set; }

        public string Fetch { get; set; }

        public override string ToString() => Fetch.Equals(Push) ? Push : $"{Push} ({Fetch})";
    }
}
