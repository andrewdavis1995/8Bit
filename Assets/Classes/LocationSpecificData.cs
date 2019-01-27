using System.Collections.Generic;

namespace Assets.Classes
{
    public static class LocationSpecificData
    {
        public static Location CurrentLocation = Location.Trials;

        public static List<string> GetLocationSpecificQuestions()
        {
            var list = new List<string>();

            switch (CurrentLocation)
            {
                case Location.Trials:
                    list.Add("Ask if this is a demo level");
                    break;
                case Location.Caredall:
                    list.Add("Ask when this place was created");
                    break;
            }

            return list;
        }
    }

    public enum Location
    {
        Trials,
        Caredall
    }

}
