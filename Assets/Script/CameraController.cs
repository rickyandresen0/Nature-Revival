using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;

    //follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float yFollowSpeed;
    private float lookAhead;


    private void Update()
    {
        //Follow player
        float targetX = player.position.x + lookAhead;
        float smoothY = Mathf.SmoothDamp(transform.position.y, player.position.y, ref velocity.y, yFollowSpeed);

        transform.position = new Vector3(targetX, smoothY, transform.position.z);
        
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * -player.localScale.x), Time.deltaTime * cameraSpeed);
    }

}
