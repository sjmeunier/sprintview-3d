using UnityEngine;
namespace Assets.Scripts.DragonAnim
{
	public class DragonWalk : StateMachineBehaviour
	{
		private float _walkspeed = 13.0f;
		private float _startTime;
		private float _footstepRate = 0.6f;
		private float _lastFootstep = 0f;
		private float _nextFootstep = 0f;

		private GameObject _dragon;
		private AudioSource _stepAudio;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_dragon = GameObject.Find("Dragon");
			_stepAudio = GameObject.Find("Dragon Footstep Audio").GetComponent<AudioSource>();
			_startTime = Time.time;
			_nextFootstep = 0.2f;
			_lastFootstep = _nextFootstep + (_footstepRate * 4) + (_footstepRate / 2);
			
		}

		override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_dragon != null)
			{
				_dragon.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, _walkspeed * Time.deltaTime);

				float elapsedTime = Time.time - _startTime;
				if (elapsedTime > _nextFootstep && elapsedTime < _lastFootstep)
				{
					if (!_stepAudio.isPlaying)
					{
						_stepAudio.Play();
					}
					_nextFootstep += _footstepRate;
				}
			}
		}
	}
}
