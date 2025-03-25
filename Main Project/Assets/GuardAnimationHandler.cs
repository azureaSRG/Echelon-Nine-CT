using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GuardAnimationHandler : MonoBehaviour
{
	
	Animator animator;
	
	void Start()
	{
		
		animator = GetComponent<Animator>();
		
		if (animator == null)
		{
			Debug.LogError("Animator Not Found on " + gameObject.name);
		}
	}
	
    public void guardIsWalking()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",true);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void guardIsRunning()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",true);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void guardIsIdle()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void guardIsShooting()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",true);
		}	
	}
}
