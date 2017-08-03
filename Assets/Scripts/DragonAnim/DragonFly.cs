using UnityEngine;

namespace Assets.Scripts.DragonAnim
{
	public class DragonFly : StateMachineBehaviour
	{
		private float _flySpeed = 18.0f;
		private float _startTime;
		private float _flapRate = 0.8f;
		private float _lastFlap = 0f;
		private float _nextFlap = 0f;

		private GameObject _dragon;
		private AudioSource _flapAudio;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
			_flapAudio = GameObject.Find("Dragon Flap Audio").GetComponent<AudioSource>();
			_startTime = Time.time;
			_nextFlap = 1.10f;
			_lastFlap = _nextFlap + (_flapRate * 4) + (_flapRate / 2);
			
		}

		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_dragon != null)
			{
				float elapsedTime = Time.time - _startTime;

				
				if (elapsedTime < 5.15f)
				{
					_dragon.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, _flySpeed * Time.deltaTime);
				}
				
				if (elapsedTime > _nextFlap && elapsedTime < _lastFlap && Settings.IsSoundOn)
				{
					if (!_flapAudio.isPlaying)
					{
						_flapAudio.Play();
					}
					_nextFlap += _flapRate;
				}

			}
		}
	}
}
