namespace Model
{
    public enum Food
    {
        [SomeString("Yummy!")]
        Pizza,
        
        [SomeString("Well...")]
        Kale,
    }

    public enum WarhammerFigurine
    {
        [SomeString("For the Emperor!")]
        SpaceMarine,
        
        [SomeString("For the Greater Good!")]
        FireWarrior
    }
}