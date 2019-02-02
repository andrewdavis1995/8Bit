using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes
{
    static class Crafting
    {
        public static Dictionary<ObjectType, int> CraftingWith = new Dictionary<ObjectType, int>();

        public static Dictionary<ObjectType, int> Craft(ref bool success)
        {
            success = false;
            var created = CraftingWith;
            if (CraftingWith.Count < 1) return created; // guard cause

            if (GetCount(ObjectType.Wood) == 3)
            {
                created = new Dictionary<ObjectType, int> { { ObjectType.Ladder, 1 } };
                success = true;
            }
            else if (GetCount(ObjectType.Wood) == 1 && GetCount(ObjectType.Stone) == 1)
            {
                //created = new Dictionary<ObjectType, int> { { ObjectType.Hammer, 1 } };
                //success = true;
            }

            // reset
            CraftingWith.Clear();

            return created;
        }

        private static int GetCount(ObjectType type)
        {
            int count = 0;

            if (CraftingWith.ContainsKey(type))
            {
                count = CraftingWith[type];
            }

            return count;
        }

        public static bool ItemSelected(ObjectType type, Transform popup)
        {
            if (CraftingWith.Count() > 2) return false; // guard

            if (CraftingWith.ContainsKey(type))
            {
                CraftingWith[type]++;
                var index = CraftingWith.Keys.ToList().IndexOf(type);
                var display = popup.GetChild(0).GetChild(index);
                display.GetComponentInChildren<Text>().text = CraftingWith[type].ToString();
            }
            else
            {
                CraftingWith.Add(type, 1);
                var display = popup.GetChild(0).GetChild(CraftingWith.Count()-1);
                display.GetComponentInChildren<Text>().text = CraftingWith[type].ToString();
                display.GetComponentInChildren<Image>().color = new Color(1, 1, 1);
                display.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
                display.GetComponentsInChildren<Image>()[1].sprite = UIScript.Instance().GetCollectableImage(type);
            }

            return true;
        }

    }
}
