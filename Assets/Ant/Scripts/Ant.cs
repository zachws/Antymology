using UnityEngine;
using System.Collections;

public class Ant : MonoBehaviour {
    Animator ant;
    public GameObject mesh;
    public Material[] materials;
    private IEnumerator coroutine;
	// Use this for initialization
	void Start () {
        ant = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.S))
        {
            ant.SetBool("idle", true);
            ant.SetBool("walk", false);
            ant.SetBool("run", false);
            ant.SetBool("eat", false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            ant.SetBool("walk", true);
            ant.SetBool("idle", false);
            ant.SetBool("run", false);
            ant.SetBool("eat", false);
        }
        if (Input.GetKey(KeyCode.R))
        {
            ant.SetBool("run", true);
            ant.SetBool("walk", false);
            ant.SetBool("idle", false);
            ant.SetBool("eat", false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ant.SetBool("turnleft", true);
            ant.SetBool("turnright", false);
            ant.SetBool("walk", false);
            ant.SetBool("idle", false);
            ant.SetBool("run", false);
            ant.SetBool("eat", false);
            StartCoroutine("idle");
            idle();
        }
        if (Input.GetKey(KeyCode.D))
        {
            ant.SetBool("turnright", true);
            ant.SetBool("turnleft", false);
            ant.SetBool("walk", false);
            ant.SetBool("idle", false);
            ant.SetBool("run", false);
            ant.SetBool("eat", false);
            StartCoroutine("idle");
            idle();
        }
        if (Input.GetKey(KeyCode.F))
        {
            ant.SetBool("attack", true);
            ant.SetBool("idle", false);
            ant.SetBool("run", false);
            ant.SetBool("walk", false);
            StartCoroutine("idle");
            idle();
        }
        if (Input.GetKey(KeyCode.Keypad1))
        {
            ant.SetBool("hit", true);
            ant.SetBool("idle", false);
            ant.SetBool("run", false);
            ant.SetBool("walk", false);
            StartCoroutine("idle");
            idle();
        }
        if (Input.GetKey(KeyCode.Keypad0))
        {
            ant.SetBool("die", true);
            ant.SetBool("idle", false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            ant.SetBool("eat", true);
            ant.SetBool("idle", false);
            ant.SetBool("walk", false);
            ant.SetBool("run", false);
        }
        if (Input.GetKey(KeyCode.G))
        {
            ant.SetBool("launch", true);
            ant.SetBool("idle", false);
            StartCoroutine("idle");
            idle();
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            mesh.GetComponent<SkinnedMeshRenderer>().material= materials[0];
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            mesh.GetComponent<SkinnedMeshRenderer>().material = materials[1];
        }
	}
    IEnumerator idle()
    {
        yield return new WaitForSeconds(0.5f);
        ant.SetBool("idle", true);
        ant.SetBool("turnleft", false);
        ant.SetBool("turnright", false);
        ant.SetBool("walk", false);
        ant.SetBool("attack", false);
        ant.SetBool("hit", false);
        ant.SetBool("launch", false);
    }
}
