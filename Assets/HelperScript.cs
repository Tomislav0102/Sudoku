using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FirstCollection
{
    public class HelperScript 
    {
        public static Vector3 MousePoz(Camera cam, LayerMask lay)
        {
            Vector3 v3 = Vector3.up;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, lay))
            {
              //  Debug.Log(hit.collider.name);
                v3 = new Vector3(hit.point.x, Mathf.Clamp(hit.point.y, -3.43f, hit.point.y), hit.point.z);
            }

            return v3;
        }

        public static T[] GetAllChildernByType<T>(Transform par)
        {
            T[] tip = new T[par.childCount];
            for (int i = 0; i < par.childCount; i++)
            {
                tip[i] = par.GetChild(i).GetComponent<T>();
            }
            return tip;
        }
        public static List<int> RandomList(int size)
        {
            List<int> brojevi = Enumerable.Range(0, size).ToList();
            var rnd = new System.Random();
            var randNums = brojevi.OrderBy(n => rnd.Next());
            List<int> list = new List<int>();
            foreach (var item in randNums)
            {
                list.Add(item);
            }

            return list;
        }
        public static List<T> RandomListByType<T>(List<T> pocetna)
        {
            var rnd = new System.Random();
            var randNums = pocetna.OrderBy(n => rnd.Next());
            List<T> list = new List<T>();
            foreach (var item in randNums)
            {
                list.Add(item);
            }

            return list;
        }
    }

    public enum UniqueValGroup
    {
        Column,
        Row,
        Block3,
        FixedValues //vjerovatno nepotrebno
    }
}

