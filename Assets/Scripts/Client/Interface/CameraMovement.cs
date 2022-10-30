using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform target {  get
        {
            GameObject localPlayer = GameObject.FindWithTag("LocalPlayer");
            if(localPlayer != null)
            {
                return GameObject.FindWithTag("LocalPlayer").transform;
            }
            return null;
            
        } 
    }
    public float smoothing;

    // Update is called once per frame
    void LateUpdate()
    {
        if(target == null) { return;  }

        if(transform.position != target.position)
        {
            Vector3 targetPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
