using System;
using UnityEngine;

public class CentrexusGeyserAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool fire = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (fire)
        {
            if (Math.Round(GetComponent<SpriteRenderer>().size.y - 0.4f) != 11)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, -2.2179f, 0), Time.deltaTime * 2.5f);
                GetComponent<SpriteRenderer>().size = Vector2.Lerp(GetComponent<SpriteRenderer>().size, new Vector2(1.04f, 10.98417f), Time.deltaTime * 2.5f);
            }
            else
            {
                fire = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, -6.6225f, 0), Time.deltaTime * 2.5f);
            GetComponent<SpriteRenderer>().size = Vector2.Lerp(GetComponent<SpriteRenderer>().size, new Vector2(1.04f, 2.175001f), Time.deltaTime * 2.5f);
        }
    }
}
