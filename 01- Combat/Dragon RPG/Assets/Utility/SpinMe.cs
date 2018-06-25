using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour {

	[SerializeField] float xRotationsPerMinute = 1f;
	[SerializeField] float yRotationsPerMinute = 1f;
	[SerializeField] float zRotationsPerMinute = 1f;
	
	void Update () {
        //https://docs.google.com/document/d/1QHufPfcPpcfGGqJx65AajMVGC6Uy3NHHViKeEdIPqAY/edit
        float xDegreesPerFrame = Time.deltaTime / 60 * 360 * xRotationsPerMinute; // TODO COMPLETE ME
        transform.RotateAround (transform.position, transform.right, xDegreesPerFrame);

		float yDegreesPerFrame = Time.deltaTime / 60 * 360 * yRotationsPerMinute;
        ; // TODO COMPLETE ME
        transform.RotateAround (transform.position, transform.up, yDegreesPerFrame);

        float zDegreesPerFrame = Time.deltaTime / 60 * 360 * zRotationsPerMinute;
        ; // TODO COMPLETE ME
        transform.RotateAround (transform.position, transform.forward, zDegreesPerFrame);
	}
}
