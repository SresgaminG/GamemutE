namespace SresgaminG.GamemutE.Entities
{
    public class Game
    {
        public string Name { get; set; }
        public string ExeName { get; set; }

        public bool IsActive { get; set; }

        public Game() { }
        public Game(string name, string exeName)
        {
            this.Name = name;
            this.ExeName = exeName;
        }
    }
}
