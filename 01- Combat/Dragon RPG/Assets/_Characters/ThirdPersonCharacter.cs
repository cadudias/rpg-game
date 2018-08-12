using UnityEngine;

namespace RPG.Characters
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;

		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;

		Rigidbody my_rigidbody;
		Animator animator;

		float turnAmount;
		float forwardAmount;
		Vector3 groundNormal;

		void Start()
		{
            animator = GetComponent<Animator>();
            my_rigidbody = GetComponent<Rigidbody>();
            my_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		}

		public void Move(Vector3 move, bool crouch, bool jump)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);

			move = Vector3.ProjectOnPlane(move, groundNormal);
            turnAmount = Mathf.Atan2(move.x, move.z);
			forwardAmount = move.z;

			ApplyExtraTurnRotation();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}

		void UpdateAnimator(Vector3 move)
		{
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (move.magnitude > 0)
            {
                animator.speed = m_AnimSpeedMultiplier;
            }
        }

        void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}
	}
}
