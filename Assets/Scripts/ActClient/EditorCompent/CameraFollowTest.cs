using UnityEngine;

public class CameraFollowTest : MonoBehaviour
{
    public float distanceAway;          // distance from the back of the craft
    public float distanceUp;            // distance above the craft
    public float smooth;                // how smooth the camera movement is
    public float horizontalRotateSpeed;
    public float verticalRotateSpeed;

    private Vector3 targetPosition;
    private Vector3 targetDirction;

    private GameObject follow;
    private GameObject target;
    private CharacterController characterCon;
    private ETCTouchPad pad;

    private float hDelta;
    private float vDelta;

    private float eulerAngles_x;
    private float eulerAngles_y;

    private void Awake()
    {
        target = new GameObject("CameraTarget");
        

        
    }

    void LateUpdate()
    {
        if (follow == null)
        {
            follow = GameObject.FindWithTag("Player");
            if (follow)
            {
                Vector3 eulerAngles = follow.transform.eulerAngles;//当前物体的欧拉角
                this.eulerAngles_x = eulerAngles.y;

                this.eulerAngles_y = eulerAngles.x;
            }
        }

        if (follow != null && characterCon == null)
        {
            characterCon = follow.transform.GetComponent<CharacterController>();
        }
        

        if (!pad)
        {
            pad = ETCInput.GetControlTouchPad("FreeLookTouchPad");
        }
        if (follow != null)
        {
            if (pad)
            {

                if (Mathf.Abs(pad.axisX.axisValue) > 0)
                {
                    this.eulerAngles_x += (pad.axisX.axisValue * this.horizontalRotateSpeed) * Time.deltaTime;
                }
                if (Mathf.Abs(pad.axisY.axisValue) > 0)
                {
                    this.eulerAngles_y += (pad.axisY.axisValue * this.verticalRotateSpeed) * Time.deltaTime;
                    this.eulerAngles_y = ClampAngle(this.eulerAngles_y, 5, 75);
                }

                Quaternion quaternion = Quaternion.Euler(this.eulerAngles_y, this.eulerAngles_x, (float)0);
                transform.rotation = target.transform.rotation = quaternion;
            }

        
            // set helper gameobject's position equal the follow gameobject  摄像机目标设置到头顶
            target.transform.position = follow.transform.position + follow.transform.up * distanceUp;

            // setting the target position to be the correct offset from the hovercraft  
            targetPosition = target.transform.position - target.transform.forward * distanceAway;

            // making a smooth transition between it's current position and the position it wants to be in
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

            //transform.LookAt(target.transform);
        }

        // fix the camera rotation
        //if (transform.localEulerAngles.z != 0)
        //{
        //    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        //}
    }

    //限制旋转角度
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
