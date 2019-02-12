using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ChangeProfileBtn : MonoBehaviour {

    public GameObject cam;

    public PostProcessingProfile profile;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeProfile()
    {
        var processing = cam.GetComponent<PostProcessingBehaviour>();
        processing.profile = profile;
    }
}
