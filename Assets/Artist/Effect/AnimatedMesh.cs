using UnityEngine;
using System.Collections;

public class AnimatedMesh : MonoBehaviour {

    public MeshFilter[] meshCache;

    public float playSpeed = 1f;
    public float playSpeedRandom = 0f;
    public float RandomScale = 0f;

    public bool randomSpeedLoop;

    public Transform meshContainerFBX;

    [HideInInspector]
    public float currentFrame;
    [HideInInspector]
    public int meshCacheCount;
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public Renderer rendererComponent;
    [HideInInspector]
    public Transform meshCached;
    
    public bool randomRotateX;
    public bool randomRotateY;
    public bool randomRotateZ;

    public bool randomStartFrame = true;

    public bool randomRotateLoop;

    public bool loop = true;
    public bool pingPong;

    public bool playOnAwake = true;
    public Vector2 randomStartDelay = new Vector2(0,0);

    private Transform mTrans;
    //private float startDelay;
    private float startDelayCounter;
    private bool pingPongToggle;
    private float currentSpeed;

    private bool isPlaying = false;

    private void Awake()
    {
        mTrans = transform;
    }

    private void OnEnable()
    {
        CheckIfMeshHasChanged();
        if(!rendererComponent)
            GetRequiredComponents();

        if (playOnAwake)
            Play();
    }

    private void OnDisable()
    {

    }

    private void Play () {
	    if(randomStartFrame)
		    currentFrame = meshCacheCount * Random.value;
	    else
		    currentFrame = 0;
	
	    meshFilter.sharedMesh = meshCache[(int)currentFrame].sharedMesh;		
	
	    RandomizePlaySpeed();
	    RandomRotate();
        RandomizeScale();
        isPlaying = true;
    }

    private void RandomRotate ()
    {
        Vector3 randomAngle = Vector3.zero;
	    if(randomRotateX)
            randomAngle.x = Random.Range(0, 360);
	    if(randomRotateY)
            randomAngle.y = Random.Range(0, 360);
	    if(randomRotateZ)
            randomAngle.z = Random.Range(0, 360);
        mTrans.localRotation = Quaternion.Euler(randomAngle);
    }

    private void RandomizeScale()
    {
        if(RandomScale != 0)
        {
            float scale = Random.Range(1.0f - RandomScale, 1.0f + RandomScale);
            mTrans.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void GetRequiredComponents () {
	    rendererComponent = GetComponent<Renderer>();
    }

    private void RandomizePlaySpeed (){
	    if(playSpeedRandom > 0)
	        currentSpeed = Random.Range(playSpeed-playSpeedRandom, playSpeed+playSpeedRandom);
	    else
	        currentSpeed = playSpeed;
    }

    protected void LateUpdate()
    {
        if(isPlaying)
        {
            AnimateMesh();
        }
    }

    public void FillCacheArray () {
	    GetRequiredComponents();
	    meshFilter = transform.GetComponent<MeshFilter>();
	    meshCacheCount = meshContainerFBX.childCount;
	    meshCached = meshContainerFBX;
	    meshCache = new MeshFilter[meshCacheCount];
	    for(int i = 0; i < meshCacheCount; i++){
		    meshCache[i] = meshContainerFBX.GetChild(i).GetComponent<MeshFilter>();
	    }
	    currentFrame = meshCacheCount*Random.value;	
	    meshFilter.sharedMesh = meshCache[(int)currentFrame].sharedMesh;
    }

    private void CheckIfMeshHasChanged(){
	    if(meshCached != meshContainerFBX){  
	        if(meshContainerFBX != null)
			    FillCacheArray();
	    }
    }

    private void AnimateMesh () {
        if(rendererComponent.enabled && rendererComponent.isVisible)
            Animate();
    }

    private bool PingPongFrame()
    {	
	    if(pingPongToggle)
            currentFrame += currentSpeed * Time.deltaTime;
	    else
            currentFrame -= currentSpeed * Time.deltaTime;	
	    if(currentFrame <= 0){			
		    currentFrame = 0;
		    pingPongToggle = true;
		    return true;
	    }	
	    if(currentFrame >= meshCacheCount){
		    pingPongToggle = false;
		    currentFrame = meshCacheCount-1;
		    return true;
	    }
	    return false;
    }

    private bool NextFrame()
    {
	    currentFrame += currentSpeed * Time.deltaTime;
	    if(currentFrame >= meshCacheCount)
        {
            if (loop)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame = meshCacheCount - 1;
                isPlaying = false;
            }
		    return true;
	    }
	    return false;
    }

    private void RandomizePropertiesAfterLoop () 
    {
	    if(randomSpeedLoop) 
		    RandomizePlaySpeed();
	    if(randomRotateLoop) 
            RandomRotate();
    }

    private void Animate () 
    {
        if (pingPong && PingPongFrame())
        {
            RandomizePropertiesAfterLoop();
        }
        else if (!pingPong && NextFrame())
        {
            RandomizePropertiesAfterLoop();
        }
        meshFilter.sharedMesh = meshCache[(int)currentFrame].sharedMesh;		
    }
}