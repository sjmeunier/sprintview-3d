using UnityEngine;

namespace Assets.Scripts.DragonAnim
{
	public class DragonIdle : StateMachineBehaviour
	{

		private GameObject _dragon;
		private GameObject _dragonFire;
		private float _startTime;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
			_dragonFire = GameObject.Find("Dragon Fire");
			_startTime = Time.time;
		}

		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_dragon != null && _dragonFire != null)
			{
				ParticleSystem fire = _dragonFire.GetComponent<ParticleSystem>();

				float elapsedTime = Time.time - _startTime;
				if (elapsedTime > 5.1 && elapsedTime < 6.5f)
				{
					if (!fire.isPlaying)
					{
						fire.Play();
					}
				}
				else if (elapsedTime >= 6.5f)
				{
					if (fire.isPlaying)
					{
						fire.Stop();
					}
				}
			}
		}
	}
}