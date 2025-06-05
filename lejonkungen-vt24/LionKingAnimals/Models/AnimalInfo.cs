namespace LionKingAnimals.Models
{
    class AnimalInfo
    {
        public int Id { get; set; } 
        public string? Name { get; set; } = null;
        public string Species { get; set; } = string.Empty;
        public string? LatinSpecies { get; set; } = null;
        public string Class { get; set; } = string.Empty;
        public string DisplayShort => string.IsNullOrEmpty(Name) ? $"No name, {Species}" : $"{Name}, {Species}";

        public override string ToString()
        {
            string returnString = "";
            if (Name != null)
            {
                returnString += $"{Name}, ";
            }
            returnString += $"{Species}, ";
            if (LatinSpecies != null)
            {
                returnString += $"{LatinSpecies}, ";
            }
            returnString += $"{Class}";
            return returnString;
        }
    }
}
