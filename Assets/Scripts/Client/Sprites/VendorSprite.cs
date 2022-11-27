using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VendorSprite : BaseSprite
{
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0) && this.GetComponent<CapsuleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            var playerObject = GameObject.FindWithTag("LocalPlayer");
            var playerSprite = playerObject.GetComponent<PlayerSprite>();

            if (Vector2.Distance(GameObject.FindWithTag("LocalPlayer").transform.position, transform.position) <= playerSprite.PickupRadius)
            {
                Debug.Log("Trade with Vendor!");
            }
        }
    }
}

