using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassDestroy : MonoBehaviour
{
    [SerializeField] private SFXManager sfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

     private void OnCollisionEnter(Collision collision)
    {
            // Check if the player has collided with an object that has the specified tag
            if (collision.gameObject.CompareTag("Player"))
            {
                sfx.PlaySFX(3);
                Destroy(gameObject);
            }
    }
}
