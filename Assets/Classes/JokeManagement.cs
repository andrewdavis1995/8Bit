using System.Collections.Generic;

namespace Assets.Classes
{
    public static class JokeManagement
    {
        private static List<Joke> _jokes = new List<Joke>();

        static JokeManagement()
        {
            _jokes.Add(new Joke("Here is a joke", 1));
            _jokes.Add(new Joke("Jokey wokey", 4));
            _jokes.Add(new Joke("That's the joke", 2));
        }
    }

    public class Joke
    {
        public string TheJoke;
        public int Rating;

        public Joke(string joke, int rating)
        {
            TheJoke = joke;
            Rating = rating;
        }

    }
}
