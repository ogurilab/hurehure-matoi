using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(-1, 0, -1);
    public Animator playerAnimator;
    public string playerAnimationName;
    private Vector3 lastPosition;
    private Animator anim;
    private bool runFlg;

    void Start()
    {
        anim = GetComponent<Animator>();
        lastPosition = player.position;
    }

    void Update()
    {
        Vector3 movement = player.position - lastPosition;
        lastPosition = player.position;

        if (movement.magnitude > 0.01f)
        {
            runFlg = true;
        }
        else
        {
            runFlg = false;
        }

        anim.SetBool("Run", runFlg);

        Vector3 followPosition = player.position + offset;
        transform.position = followPosition;

        AnimatorStateInfo playerStateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (playerStateInfo.IsName(playerAnimationName))
        {
            anim.SetTrigger("Sasumata");
        }
    }
}
