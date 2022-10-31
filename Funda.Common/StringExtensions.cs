namespace Funda.Common
{
    /*
        NOTE: 
        We have to be really careful with adding new methods here.
        This "syntactic sugar" exists only for the sake of readability and acts like a kind of gap filler for the APIs that should be there in BCL, but they do not exist yet.
        Only primitive, dependency-less, methods with strings should be added here.
    */
    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string str, string with) => 
            str.EndsWith(with) ? str : str + with;
    }
}
