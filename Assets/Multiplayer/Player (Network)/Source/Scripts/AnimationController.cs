using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private FirstPersonController controller;

    [HideInInspector] public bool isInTheCar;
    public GameObject hip;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!controller || !controller.playerCanMove) return;

        // Calculate local velocity
        Vector3 localVelocity = controller.transform.InverseTransformDirection(controller.CurrentVelocity);
        float forwardSpeed = Mathf.Abs(localVelocity.z);
        float sideSpeed = Mathf.Abs(localVelocity.x);

        bool isRunning = forwardSpeed > 0.1f || sideSpeed > 0.1f;
        animator.SetBool("Running", isRunning);

        // Crouching animation
        animator.SetBool("Crouched", controller.isCrouched && isRunning);

        // Falling animation
        animator.SetBool("Falling", controller.isFalling);

        // Car state
        animator.SetBool("isInTheCar", isInTheCar);

        // Jumping animation
        if (controller.isJumpingThisFrame)
        {
            StartCoroutine(SetJumpingBool(0.1f));
        }
    }

    IEnumerator SetJumpingBool(float interval)
    {
        animator.SetBool("Jumping", true);
        yield return new WaitForSeconds(interval);
        animator.SetBool("Jumping", false);
    }
    public void OnEnable()
    {
        hip.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
