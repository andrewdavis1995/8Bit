using System;
using System.Collections.Generic;

namespace Assets.Classes
{
    [Serializable]
    public class Person
    {
        public int Age;
        public string Name;
        public Gender Gender;
        public int Anger;
        public int Love;
        public int Chattiness;
        public int Friendliness;
        public CharacterType CharacterType;
        public bool RomanticallyAvailable;
        public int Health = 0;

        private List<string> _jokesTold;
        private int _complimentsGiven = 0;
        private int _questionsAsked = 0;

        public void TellJoke()
        {

        }


    }
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum CharacterType
{
    Friendly,
    Normal,
    Bully,
    Professional,
    Grumpy,
}