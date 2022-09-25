using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatcha : MonoBehaviour
{
    public static class WeightedRandomizer
    {
        public static WeightedRandomizer<R> From<R>(Dictionary<R, int> spawnRate)
        {
            return new WeightedRandomizer<R>(spawnRate);
        }
    }

    public class WeightedRandomizer<T>
    {
        private static System.Random _random = new System.Random();
        private Dictionary<T, int> _weight;

        public WeightedRandomizer(Dictionary<T, int> weight)
        {
            _weight = weight;
        }

        public T TakeOne()
        {
            var sortedSpawnRate = Sort(_weight);
            int sum = 0;
            foreach (var spawn in _weight)
            {
                sum += spawn.Value;
            }

            int roll = _random.Next(0, sum);

            T selected = sortedSpawnRate[sortedSpawnRate.Count - 1].Key;
            foreach (var spawn in sortedSpawnRate)
            {
                if (roll < spawn.Value)
                {
                    selected = spawn.Key;
                    break;
                }
                roll -= spawn.Value;
            }
            return selected;
        }
        private List<KeyValuePair<T, int>> Sort(Dictionary<T, int> weights)
        {
            var list = new List<KeyValuePair<T, int>>(weights);

            list.Sort(
                delegate (KeyValuePair<T, int> firstPair, KeyValuePair<T, int> NextPair)
                {
                    return firstPair.Value.CompareTo(NextPair.Value);
                }
                );

            return list;
        }
    }

    enum Card { Bronze, Silver, Gold, Diamond}
    public GameObject Bronze_image = null;
    public GameObject Silver_image = null;
    public GameObject Gold_image = null;
    public GameObject Diamond_image = null;
    private Dictionary<Card, int> m_Cards = new Dictionary<Card, int>();
    [SerializeField]
    private Card m_card = Card.Bronze;

    // Start is called before the first frame update
    void Start()
    {
        m_Cards.Add(Card.Bronze, 90);
        m_Cards.Add(Card.Silver, 6);
        m_Cards.Add(Card.Gold, 3);
        m_Cards.Add(Card.Diamond, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            m_card = WeightedRandomizer.From(m_Cards).TakeOne();
            if(m_card == Card.Bronze)
            {
                Bronze_image.SetActive(true);
                if (3f - Time.deltaTime < 0)
                    Bronze_image.SetActive(false);
            }
            else if (m_card == Card.Silver)
            {
                Silver_image.SetActive(true);
                if (3f - Time.deltaTime < 0)
                    Silver_image.SetActive(false);
            }
            else if(m_card == Card.Gold)
            {
                Gold_image.SetActive(true);
                if (3f - Time.deltaTime < 0)
                    Gold_image.SetActive(false);
            }
            else
            {
                Diamond_image.SetActive(true);
                if (3f - Time.deltaTime < 0)
                    Diamond_image.SetActive(false);
            }
            Debug.Log(m_card.ToString());
        }
    }
}
