namespace LionKingAnimals.Models
{
    internal class SpeciesInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Animals { get; set; } = 0;

        public override string ToString()
            => $"{Name}, {Animals}";
    }
}
