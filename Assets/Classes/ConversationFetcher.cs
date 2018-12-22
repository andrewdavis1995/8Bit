using System.Collections.Generic;
using System.Linq;

namespace Assets.Classes
{
    public static class ConversationFetcher
    {
        public static string GetJoke(string category)
        {
            List<string[]> jokes = new List<string[]>
            {
                new string[]{"Music", "Never take stock market advice from N-Sync... It's always Buy Buy Buy!" },
                new string[]{"Disney", "I recently invested all my money in a Disney movie about an ice princess... All my assets are frozen!" },
                new string[]{"Scottish", "What do you call a Scottish man is is half in his house? Hamish!" },
            };

            var relevant = jokes.Where(j => j[0] == category);

            var random = UnityEngine.Random.Range(0, jokes.Count);
            var randomJoke = jokes[random];
            return randomJoke[1];
        }

        public static string GetCompliment(string category)
        {
            List<string[]> compliments = new List<string[]>
            {
                new string[]{"Hair", "Nice hair!" },
                new string[]{"Personality", "You're awesome!" },
            };

            var relevant = compliments.Where(j => j[0] == category);

            var random = UnityEngine.Random.Range(0, compliments.Count);
            var randomCompliment = compliments[random];
            return randomCompliment[1];
        }
    }
}
