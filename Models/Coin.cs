namespace Models
{
    public class Coin
    {
        public string Name { get; set; }
        public decimal Course { get; set; }
        
        public Coin()
        {
        }
        
        public Coin(string name, decimal course)
        {
            Name = name;
            Course = course;
        }
    }
}