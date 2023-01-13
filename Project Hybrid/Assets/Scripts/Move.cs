/*using UnityEngine;

public class Move : MonoBehaviour
{
    ArduinoInput input;

    [SerializeField] private float minY = -5;
    [SerializeField] private float maxY = 5;

    [SerializeField] private float minWaterLevel = 0;
    [SerializeField] private float maxWaterLevel = 200;

    [SerializeField] private float conditionValue = 10;

    [SerializeField] private float testValue;

    private void Start()
    {
        input = FindObjectOfType<ArduinoInput>();
    }

    private void Update()
    {
        Vector3 newPos = transform.position;

        float waterLevel = input.SendInput();
        //float waterLevel = testValue;
        float newY = Helpers.Map(minWaterLevel, maxWaterLevel, minY, maxY, waterLevel);

        newPos.y = newY;
        transform.position = newPos;
    }

    *//*    private void Update()
        {
            float value = input.GetWaterLevel();

            if (value > conditionValue)
            {
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
        }*//*
}
*/