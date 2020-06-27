using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed;
    int trailTime = 5;
    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 30;
        guiStyle.normal.textColor = Color.cyan;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0:00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population:  " + population.Count, guiStyle);
        GUI.EndGroup();
    }


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 starting_pos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
                                               this.transform.position.y,
                                               this.transform.position.z + Random.Range(-2, 2));

            GameObject b = Instantiate(botPrefab, starting_pos, this.transform.rotation);
            b.GetComponent<Brain>().Init();
            population.Add(b);
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trailTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 starting_pos = new Vector3(this.transform.position.x + Random.Range(-2, 2),
                                               this.transform.position.y,
                                               this.transform.position.z + Random.Range(-2, 2));

        GameObject offspring = Instantiate(botPrefab, starting_pos, this.transform.rotation);
        Brain b = offspring.GetComponent<Brain>();

        if(Random.Range(0,100) == 1)
        {
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
        }

        return offspring;
    }

    void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();
        //get rid of unfit objects

        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().distanceTravelled).ToList();
        population.Clear();

        //breed only the most fit half of the total population
        for (int i = (int)(sortedList.Count / 2) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        //destory all previous parents and population
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        generation++;
    }
}
