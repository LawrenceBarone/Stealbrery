using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BookPile : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] Rigidbody rb;
    [SerializeField] S_InputReader inputReader;

    [Header("Param")]
    [SerializeField] int maxPileUp = 10;
    [SerializeField] int currentAmount = 0;
    [SerializeField] float sensivity = 5;
    [SerializeField] float timeBeforeFall = 3;

    [SerializeField] Transform headBookPosition;
    [SerializeField] Transform lookDirection;
    [SerializeField] GameObject[] bookTypes;
    [SerializeField] Vector3 offSetHeight;
    [SerializeField] CMF.SimpleWalkerController simpleWalkerController;
    Vector3 currentEulerAngles;

    [SerializeField] float forceBookMultiplier = 0.3f;
    [SerializeField] float timeBeforeFallModifier = 1f;
    [SerializeField] int timeSetForFall = 2;
    [SerializeField] float shakeIntensity;
    [SerializeField] float angleToFall = 40;

    List<GameObject> bookpile = new List<GameObject>();

    private void Awake()
    {
        inputReader.AttackEvent += OnGrab;
        simpleWalkerController = GetComponent<CMF.SimpleWalkerController>();
    }

    private void Update()
    {
        Debug.Log("magnitude : " + rb.velocity.magnitude);
        if (bookpile.Count == 0) return;
        if (rb.velocity.magnitude > sensivity)
        {
            Debug.Log("moving");
            //currentAmount = 0;
           
            currentEulerAngles += simpleWalkerController.CalculateMovementDirection() * forceBookMultiplier * timeBeforeFallModifier * 0.7f;
            //currentEulerAngles += new Vector3(timeBeforeFall*0.1f, 0f, 0f);
            headBookPosition.eulerAngles = currentEulerAngles;
            if (checkAngleState())
            {
                timeBeforeFall -= Time.deltaTime * timeBeforeFallModifier * 1.5f;
                if (timeBeforeFall < 0)
                {
                    BookFall();
                }
            }
            
            
        }
        else 
        {
            Debug.Log("recovering");
            if(timeBeforeFall < timeSetForFall) timeBeforeFall += Time.deltaTime;
            if (currentEulerAngles.x > 0f)
            {
                currentEulerAngles -= new Vector3(timeBeforeFall * 0.2f, 0f, 0f);
                headBookPosition.eulerAngles = currentEulerAngles;
            }else if(currentEulerAngles.x < 0f)
            {
                currentEulerAngles += new Vector3(timeBeforeFall * 0.2f, 0f, 0f);
                headBookPosition.eulerAngles = currentEulerAngles;
            }

            if (currentEulerAngles.z > 0f)
            {
                currentEulerAngles -= new Vector3(0f, 0f, timeBeforeFall * 0.2f);
                headBookPosition.eulerAngles = currentEulerAngles;
            }else if (currentEulerAngles.z < 0f)
            {
                currentEulerAngles += new Vector3(0f, 0f, timeBeforeFall * 0.2f);
                headBookPosition.eulerAngles = currentEulerAngles;
            }
        }
    }

    private bool checkAngleState()
    {
        bool state = false;
        if (currentEulerAngles.x > angleToFall)
        {
            state = true;
        }
        if (currentEulerAngles.z > angleToFall)
        {
            state = true;
        }
        if (currentEulerAngles.x < -angleToFall)
        {
            state = true;
        }
        if (currentEulerAngles.z < -angleToFall)
        {
            state = true;
        }
        return state;
    }
    void BookFall()
    {
        Debug.Log("fall");
        CleanBookStack();
        currentAmount = 0;
        offSetHeight = new Vector3(0, 0, 0);
        currentEulerAngles = new Vector3(0f, 0f, 0f);
        headBookPosition.eulerAngles = currentEulerAngles;
        timeBeforeFall = timeSetForFall;
        timeBeforeFallModifier = 1f;
        angleToFall = 40f;
    }
    void OnGrab()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one, Vector3.up, Quaternion.identity, Mathf.Infinity);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("BooksPile"))
            {
                currentAmount++;
                if (currentAmount > maxPileUp) currentAmount = maxPileUp;
                print("Win");
                StackBook();
            }
        }
    }

    void StackBook()
    {
        offSetHeight += new Vector3(0f, 0.20f, 0f);
        timeBeforeFallModifier += 0.1f;
        angleToFall -= 1f;
        int _rand = Random.Range(0, bookTypes.Length);
        GameObject go = Instantiate(bookTypes[_rand], headBookPosition.position + offSetHeight, lookDirection.rotation);
        go.transform.parent = headBookPosition;
        bookpile.Add(go);
    }

    void CleanBookStack()
    {
        
        foreach(GameObject go in bookpile)
        {
            go.transform.parent = null;
            go.AddComponent<Rigidbody>();
            go.GetComponent<Rigidbody>().useGravity = true;
        }
        bookpile.Clear();
    }
}
