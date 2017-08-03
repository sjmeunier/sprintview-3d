using UnityEngine;

namespace Assets.Scripts.DragonAnim
{
	public class DragonFly : StateMachineBehaviour
	{
		private float _flySpeed = 18.0f;
		private float _startTime;

		private GameObject _dragon;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
			_startTime = Time.time;
		}

		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_dragon != null)
			{
				if (Time.time - _startTime < 5.15f)
				{
					_dragon.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, _flySpeed * Time.deltaTime);
				}
			}
		}
	}
}
