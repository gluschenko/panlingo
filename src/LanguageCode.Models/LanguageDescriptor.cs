namespace Panlingo.LanguageCode.Models
{
    public class LanguageDescriptor
    {
        public string Id { get; set; }
        public string Part2b { get; set; }
        public string Part2t { get; set; }
        public string Part1 { get; set; }
        public string Scope { get; set; }
        public string LanguageType { get; set; }
        public string RefName { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{Id}; {Part2b}; {Part2t}; {Part1}; {Scope}";
        }
    }
}
