using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Collider[] enemiesArray;
    private List<GameObject> enemiesList;
    private float range = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindEnemies();
    }

    private void FindEnemies()
    {
        enemiesList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Target"));

        //Range
        foreach(GameObject enemy in enemiesList)
        {
            if((enemy.transform.position - transform.position).sqrMagnitude > range * range)
            {
                enemiesList.Remove(enemy);
            }
        }

        //Set Attacking
        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (i == 0)
            {
                enemiesList[0].GetComponent<Enemy>().attacking = true;
            }
            else
            {
                enemiesList[0].GetComponent<Enemy>().attacking = false;
            }
        }

    }
    private void CheckEnemies()
    {
        enemiesArray = Physics.OverlapSphere(transform.position, range);

        //Convert Array to List
        foreach(Collider enemy in enemiesArray)
        {
            enemiesList.Add(enemy.gameObject);
        }

        //Remove Null
        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (enemiesList[i] == null)
            {
                enemiesList.Remove(enemiesList[i]);
            }
        }

            //Set Attacking
        for (int i = 0; i < enemiesList.Count; i++)
        {
            if(i == 0)
            {
                enemiesList[0].GetComponent<Enemy>().attacking = true;
            }
            else
            {
                enemiesList[0].GetComponent<Enemy>().attacking = false;
            }

            enemiesList[0].GetComponent<Enemy>().attacking = true;
            Debug.Log("attack");

        }
    }
}
