using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

public class MoveAgent : Agent
{
    [SerializeField] private Transform targetTransform; //coin
    [SerializeField] private Transform Bomb1Transform;  
    [SerializeField] private Transform Bomb2Transform;
    [SerializeField] private Transform Bomb3Transform;

    [SerializeField] private GameObject StatusBAR;



    private float lastDistance;
    private float currentDistance;


    int WallHit = 0;
    int EnemyHit = 0;
    int CoinHit = 0;

    public TextMeshProUGUI WallHitTXT;
    public TextMeshProUGUI EnemyHitTXT;
    public TextMeshProUGUI CoinHitTXT;

    public override void OnEpisodeBegin()
    {

        transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(7f, 9f), 0);

        targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-7f, -9f), 0);


        Bomb1Transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(2f, 5f), 0);

        Bomb2Transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(2f, -2f), 0);

        Bomb3Transform.localPosition = new Vector3(Random.Range(-4f, 4f), Random.Range(-2f, -5f), 0);



        lastDistance = Vector2.Distance(transform.position, targetTransform.position);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 dirAI = (transform.localPosition - transform.localPosition).normalized;
        Vector3 dirtargetTransform = (targetTransform.transform.localPosition - transform.localPosition).normalized;
        Vector3 dirbomb1 = (Bomb1Transform.transform.localPosition - transform.localPosition).normalized;
        Vector3 dirbomb2 = (Bomb2Transform.transform.localPosition - transform.localPosition).normalized;
        Vector3 dirbomb3 = (Bomb3Transform.transform.localPosition - transform.localPosition).normalized;




        sensor.AddObservation(dirAI.x); //player position u artýk biliyor
        sensor.AddObservation(dirAI.y); //player position u artýk biliyor
        sensor.AddObservation(dirtargetTransform.x); //player position u artýk biliyor
        sensor.AddObservation(dirtargetTransform.y); //player position u artýk biliyor
        sensor.AddObservation(dirbomb1.x); //player position u artýk biliyor
        sensor.AddObservation(dirbomb1.y); //player position u artýk biliyor
        sensor.AddObservation(dirbomb2.x); //player position u artýk biliyor
        sensor.AddObservation(dirbomb2.y); //player position u artýk biliyor
        sensor.AddObservation(dirbomb3.x); //player position u artýk biliyor
        sensor.AddObservation(dirbomb3.y); //player position u artýk biliyor


        sensor.AddObservation(currentDistance); // 1

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;

        //yakýnlýk uzaklýða göre ödül ver

        //sürekli yaklaþmasý için. yaklaþmak yerine uzaklaþýrsa cezalandýrýp yakýnklaþtýkça ödül
        currentDistance = Vector2.Distance(transform.position, targetTransform.position);
        if (currentDistance < lastDistance)
        {
            AddReward(0.005f);
            lastDistance = currentDistance;
        }
        else
        {
            AddReward(-0.005f);
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.tag == ("Coin")) //baþardý
        {
            AddReward(+5f);
            StatusBAR.GetComponent<SpriteRenderer>().color = Color.green;

            CoinHit++;
            CountUpdate();
            EndEpisode();
        }


        if (collision.gameObject.tag == ("Wall")) //baþarýsýz duvara çarptý
        {
            AddReward(-5f);
            StatusBAR.GetComponent<SpriteRenderer>().color = Color.yellow;

            WallHit++;
            CountUpdate();

            EndEpisode();
        }
        if (collision.gameObject.tag == ("Enemy")) //baþarýsýz bombaya çarptý
        {
            AddReward(-5f);
            StatusBAR.GetComponent<SpriteRenderer>().color = Color.red;

            EnemyHit++;
            CountUpdate();

            //StartCoroutine(ENDEPÝSODECONTROLLED());
            EndEpisode();
        }


    }
    public void CountUpdate()
    {
        WallHitTXT.text = "" + WallHit;
        EnemyHitTXT.text = "" + EnemyHit;
        CoinHitTXT.text = "" + CoinHit;
    }

    void Update()
    {
        Debug.Log(GetCumulativeReward());
    }


}

