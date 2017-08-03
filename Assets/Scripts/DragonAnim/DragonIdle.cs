using UnityEngine;

namespace Assets.Scripts.DragonAnim
{
	public class DragonIdle : StateMachineBehaviour
	{

		private GameObject _dragon;
		private GameObject _dragonFire;
		private float _startTime;
		private AudioSource _dragonGrowl;
		private AudioSource _dragonRoar;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
			_dragonFire = GameObject.Find("Dragon Fire");
			_dragonRoar = GameObject.Find("Dragon Roar Audio").GetComponent<AudioSource>();
			_dragonGrowl = GameObject.Find("Dragon Growl Audio").GetComponent<AudioSource>();
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
				if (elapsedTime > 4.9 && elapsedTime < 6.5f)
				{
					if (!_dragonRoar.isPlaying)
					{
						_dragonRoar.Play();
					}
				}
				else if (elapsedTime >= 6.5f)
				{
					if (fire.isPlaying)
					{
						fire.Stop();
					}
				}

				//Play ominous growl
				if (elapsedTime > 11.0f && elapsedTime < 13.0f)
				{
					if (!_dragonGrowl.isPlaying)
					{
						_dragonGrowl.Play();
					}
				}
			}
		}
	}
}