using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorMapShip : MonoBehaviour {

    public float blastOnSpeed = 60f;
    public float blastOnStart = -1590f;
    public float blastOnEnd = -1495f;
    public float blastOffEnd = -950f;
    public float blastOffSpeed = 150f;
    public float shipResetPostionX = -100f;
    

    public IEnumerator BlastOn()
    {
        transform.position = new Vector3((blastOnStart), transform.position.y, transform.position.z);

        //Debug.Log("Enter BlastOn: " + transform.position);

        while (transform.position.x < blastOnEnd)
        {
            //Debug.Log("BlastOn Loop: " + transform.position);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(blastOnEnd, transform.position.y, transform.position.z), blastOnSpeed * Time.deltaTime);
            yield return null;
        }

    }

    public IEnumerator BlastOff () {
		while(transform.position.x < blastOffSpeed)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(blastOffSpeed, transform.position.y, transform.position.z), blastOffSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        
	}

}
