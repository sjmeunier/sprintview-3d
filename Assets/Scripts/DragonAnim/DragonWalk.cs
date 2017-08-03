using UnityEngine;
namespace Assets.Scripts.DragonAnim
{
	public class DragonWalk : StateMachineBehaviour
	{
		private float _walkspeed = 13.0f;

		private GameObject _dragon;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
		}

		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_dragon != null)
			{
				_dragon.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, _walkspeed * Time.deltaTime);
			}
		}
	}
}
