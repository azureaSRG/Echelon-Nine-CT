using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyAnimationHandler : MonoBehaviour
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
	
    public void enemyIsWalking()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",true);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void enemyIsRunning()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",true);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void enemyIsIdle()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",false);
		}
	}
	
	public void enemyIsShooting()
	{
		if (animator != null)
		{
			animator.SetBool("isWalking",false);
			animator.SetBool("isRunning",false);
			animator.SetBool("isShooting",true);
		}	
	}
}
