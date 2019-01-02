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

            var relevant = jokes.Where(j => j[0] == category).ToList();

            var random = UnityEngine.Random.Range(0, relevant.Count());
            var randomJoke = relevant[random];
            return randomJoke[1];
        }

        public static string GetCompliment(string category)
        {
            List<string[]> compliments = new List<string[]>
            {
                new string[]{"Hair", "Nice hair!" },
                new string[]{"Personality", "You're awesome!" },
            };

            var relevant = compliments.Where(j => j[0] == category).ToList();

            var random = UnityEngine.Random.Range(0, relevant.Count());
            var randomCompliment = relevant[random];
            return randomCompliment[1];
        }

        public static string GetQuestion(string category, ref KnowledgeCategory knowledgeCategory)
        {
            List<object[]> questions = new List<object[]>
            {
                new object[]{ "Ask how to build a ladder", "How do you build a ladder?", KnowledgeCategory.HowToMakeALadder },
                new object[]{ "Ask age", "How old are you?", KnowledgeCategory.HowOldAreYou },
                new object[]{ "Ask name", "What is your name?", KnowledgeCategory.WhatIsYourName },
                new object[]{ "Ask where to buy weapons", "Where can I buy weapons?", KnowledgeCategory.WhereToBuyWeapons },

                new object[]{ "Ask if this is a demo level", "Is this a demo level?", KnowledgeCategory.IsThisADemoLevel },

                new object[]{ "Ask when this place was built", "When was this place built?", KnowledgeCategory.WhenWasThisPlaceBuilt},
            };

            var relevant = questions.Where(q => (string)q[0] == category).ToList();

            var random = UnityEngine.Random.Range(0, relevant.Count());
            var randomQuestion = relevant[random];
            knowledgeCategory = (KnowledgeCategory)randomQuestion[2];
            return (string)randomQuestion[1];
        }
    }
}
