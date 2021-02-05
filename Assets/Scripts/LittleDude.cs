using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/******************************************************************************************************************************************************************************
 * This is that main controller for your Units. The methods you can use are clearly labeled, as well are the ones you are not allowed to use.
 * 
 * This controller's speed and physics settings are all preset. You are NOT allowed to move your unit's position by applying forces to it's Rigidbody. 
 * All of the unit's movement will be handled by it's NavMeshComponent. However, Do not use NavMeshAgent directly. Methods and variables have been 
 * provided in this script to be used instead. Using or manipulating the NavMeshAgent directly will result in disqualification.
 * 
 * However, you may apply spin forces to your rigidbody, as long as it does not move their position in a way that does not fit in the spirit of the competition.
 * (for example, you may not use huge spin forces in order to manipulate the physics engine.)
 * 
 * Never make assumptions! If you have any question or doubt on whether your code follows the rules, simply check-in at the discord and ask before proceeding. No exceptions
 * for illegal code will be made, so always double check before investing your time and effort!
 *
 *******************************************************************************************************************************************************************************/

public class LittleDude : MonoBehaviour
{
    [Header("Unit Information")]
    public bool isRed;
    public bool isBlue;
    public bool tagged;
    public bool hasFlag;
    public bool isMoving;
    public bool isSprinting;
    public bool isInOpponentArea;
    public bool inJail;
    public Vector3 destinationWhenTagged;

    public bool mustReturnToBase;   // After the a unit is freed from jail, it must return to it's own side before continuing the battle.
                                    // This bool controls that, and may not be altered directly. But it may be referenced.

    // The stamina varible should be treated as "read-only". Do not change directly.
    // Sprint will automatically turn off when stamina reaches 0.
    [Header("Stamina / Sprint")]
    public float stamina;
    public float staminaDecreaseRate; // You may not alter this in any way.

    [Header("Automatic Functions")]
    public bool autoMove;           // If true the unit will automatically move to the navTarget position if it is not within stopping distance.
    public bool autoLookAtTarget;   // If true the unit will automatically face the navTarget position.
    public bool autoResumeNavTarget;// After the unit is freed from jail, if true the unit will automatically resume targetting it's last known target 
                                    // (after it has returned to it's own side)

    [Header("Nav Settings")]
    public Vector3 navTarget;
    public float distanceToTarget;
    public float stopDistance;      // If you wish to change the NavMeshAgent's stopping distance you can do so with this variable. Do not use the NavMeshAgent directly.

    private NavMeshAgent navAgent;  // DO NOT USE DIRECTLY.
    private Animator animator;      // It is fine to call your own animations if you would prefer. Otherwise, methods have been provided.
                                    // Note: This unity uses "Root Animation" to move. Playing an animation is what actually drives it's movements.

    [Header("Battlefield Info")]
    public Flag redFlag;
    public Transform redJail;
    public Flag blueFlag;
    public Transform blueJail;

    // Auto attack and auto tag functions are included in this script. More info below.
    [Header("Attack Settings")]
    public bool autoAttack;
    public bool isAttacking;
    public HitBox hitBox;

    // This script's setup
    [Header("Setup")]
    public SkinnedMeshRenderer meshRenderer;
    public Transform flagSlot;
    public Material normalMat;
    public Material taggedMat;
    public Material mustReturnToBaseMat;
    public bool animationOverride;
    private BattleInfo battleInfo;

    // Please drag all of your scripts here.
    [Header("YOUR SCRIPTS HERE")] [Tooltip("Drag all of your scripts here.")]
    public UnityEngine.MonoBehaviour[] scriptsToOverride;
    public bool scriptsDisabled;

    private Rigidbody rb; // do not use directly for movement. Only use SetDestination to move your units.

    private void Awake()
    {
        if (!navAgent)
            navAgent = GetComponent<NavMeshAgent>();

        if (!animator)
            animator = GetComponent<Animator>();

        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Initialize start position.
        navTarget = transform.position;

        if (!battleInfo)
            battleInfo = BattleInfo.instance;

        Debug.Log("Battle Info successful", battleInfo);

        IntializeBattlefieldInfo();
    }

    private void Update()
    {
        if (!tagged) // tagged = this unit was tagged by an opponent and must go to jail.
        {
            // Checks to see if this unit is in the Opponent's zone.
            // if isInOpponentArea is true, the unit can be tagged by the opposing team.
            if (isBlue)
            {
                if (transform.position.x < 0)
                    isInOpponentArea = false;
                else if (transform.position.x > 0)
                    isInOpponentArea = true;
            }
            else
            {
                if (transform.position.x > 0)
                    isInOpponentArea = false;
                else if (transform.position.x < 0)
                    isInOpponentArea = true;
            }

            /******************************************************************************************************************************************
             * AUTO ATTACK / AUTO TAG NOTES
             * 
             * AttackCheck() is an auto attack / auto jail break tag function that can be used directly or replaced with your own.
             * 
             * AUTO ATTACK: Will automatically try to tag an opponent if they are in this unit's hitbox and are taggable.
             *  To disable and use manually: 
             *      1) Set autoAttack to false.
             *      2) Call ManualTag() to manually start the tag routine.
             *      
             * AUTO JAILBREAK TAG: Will automatically attempt to "jail break" an ally unit if that unit is in jail and within this unit's hit box.
             *  To disable and use manually: 
             *      1) Set autoAttack to false.
             *      2) Call ManualJailBreakTag() to manually start the tag routine.
             *      
             *******************************************************************************************************************************************/

            if (autoAttack)
                AttackCheck();
        }
        else
        {
            // This is an automatic function to check if this unit is tagged and needs to go to jail.
            GoToJailCheck();
        }

        // This automatically begins the moving animation if the unity has a navTarget and is not currently at that target.
        // animationOverride is provided in case you wish to disable this feature.
        if (!animationOverride)
            ShouldMoveCheck();

        // Controls the NavMeshAgent and makes sure it's target is the navTarget position.
        // If you prefer to call LittleDude.SetDestination() manually, set "autoMove" to false to disable this.
        if (navAgent.destination != navTarget && autoMove)
            navAgent.SetDestination(navTarget);

        // Faces the unit towards it's NavMeshAgent's steeringTarget, can be overridden using animationOverride;
        if (isMoving && !animationOverride && autoLookAtTarget)
            transform.LookAt(navAgent.steeringTarget);

        // Sprint and stamina drain. Do not alter stamina directly.
        if (isSprinting)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run W Root"))
                SetSprint();

            if (stamina > 0)
            {
                stamina -= staminaDecreaseRate * Time.deltaTime;
            }
            else
            {
                stamina = 0;
                SetWalk();
                //stamina regen routine
            }
        }

        // Checks to see if stopping distance was altered. Only use LittleDude.stoppingDistance.
        NavAgentSettingsCheck();

        /* 
         * MustReturnToBaseCheck()
         * This will override any SetDesitnation you try to call. This is on purpose, do not circumvent this.
         * Once the unit has made it back to their side, the unit will resume targetting it's last known navTarget. 
         * (to disable this, set AutoResumeNavTarget to false)
         */
        MustReturnToBaseCheck();

        // Handles flag pick up and returning. Units will automatically pick up or return the flag when close enough.
        // You may not pick up the flag using your own scripts.
        FlagPickupCheck();

        if (tagged && !scriptsDisabled)
        {
            DisableScripts();
        }
        else if (!tagged && scriptsDisabled)
        {
            EnableScripts();
        }
    }

    // Sets or updates NavMeshAgent destination, triggering the calculation of a new path.
    // You may also set the variable directly using LittleDude.navTarget
    public void SetDestination(Vector3 _pos)
    {
        navTarget = _pos;
    }

    // *******************************************************************************************************************************
    // Speeds of the character are animation driven. In order to set your unit to spinting, walking, or idle, use the methods below.
    // *******************************************************************************************************************************

    // Used by default
    public void SetWalk()
    {
        animator.Play("Walk W Root");
        isMoving = true;
        isSprinting = false;
    }

    // Sprint will automatically switch to walk when staminia is at 0.
    public void SetSprint()
    {
        if (stamina > 0)
        {
            animator.Play("Run W Root");
            isMoving = true;
            isSprinting = true;
        }
    }

    public void SetIdle()
    {
        navTarget = transform.position;
        animator.Play("Idle");
        isMoving = false;
    }

    // Mainly used for tag animation, but I will allow this method to be public if you want to get creative for how you use it.
    // HOWEVER: This is only allowed to be called on your OWN units, not your opponents. 
    // (ie, you may not make your opponent's units fall over using your own scripts)
    public void SetFallOver()
    {
        animator.Play("Die");
    }

    public void ManualTag()
    {
        StartCoroutine(TagRoutine());
    }

    public void ManualJailBreakTag()
    {
        StartCoroutine(JailBreakTagRoutine());
    }

    // *****************************************************************************************************************************************************
    // Here are the acceptable methods relating to the NavMeshAgent. If you wish to use something that is not listed below and believe it does not give you 
    // an unfair advantage, feel free to bring it up in the Discord channel. I have included the Unity Docmentation descriptions above each method.
    // *****************************************************************************************************************************************************


    /*
     * LittleDude.CalculatePath
     * 
     * This function can be used to plan a path ahead of time to avoid a delay in gameplay when the path is needed. 
     * Another use is to check if a target position is reachable before moving the agent.
     */
    public void CalculatePath(Vector3 _targetPosition, NavMeshPath _path)
    {
        navAgent.CalculatePath(_targetPosition, _path);
    }


    /* 
     * LittleDude.FindClosestEdge
     * 
     * The returned NavMeshHit object contains the position and details of the nearest point on the nearest edge of the Navmesh. 
     * Since an edge typically corresponds to a wall or other large object, this could be used to make a character take cover as close to the wall as possible.
     */
    public bool FindClosestEdge(out NavMeshHit _hit)
    {
        return navAgent.FindClosestEdge(out _hit);
    }


    /* 
     * LittleDude.Raycast
     * 
     * Trace a straight path towards a target postion in the NavMesh without moving the agent.
     * 
     * This function follows the path of a "ray" between the agent's position and the specified target position. 
     * If an obstruction is encountered along the line then a true value is returned and the position and 
     * other details of the obstructing object are stored in the hit parameter. This can be used to check 
     * if there is a clear shot or line of sight between a character and a target object. 
     * This function is preferable to the similar Physics.Raycast because the line tracing is performed in 
     * a simpler way using the navmesh and has a lower processing overhead.
     */
    public bool Raycast(Vector3 _targetPosition, out NavMeshHit _hit)
    {
        return navAgent.Raycast(_targetPosition, out _hit);
    }

    /*
     * LittleDude.ResetPath
     * 
     * Clears the current path. 
     */
    public void ResetPath()
    {
        navAgent.ResetPath();
        navTarget = transform.position;
    }




    // ****************************************************************
    // YOU MAY NOT ALTER OR CALL DIRECTLY ANY METHODS BELOW THIS LINE. 
    // ****************************************************************

    // ***********************************************************************************************************************************************
    // NOTE: A script called RefereeMaster will be checking throughout the game to make sure nothing 
    // below this line is being used inappropraitely. 
    // 
    // Any direct calls to the methods below will result in immediate disqualification!
    // ************************************************************************************************************************************************




    private void ShouldMoveCheck()
    {
        distanceToTarget = Vector3.Distance(transform.position, navTarget);
        if (distanceToTarget > stopDistance && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) 
            SetWalk();
        else if (distanceToTarget <= stopDistance && isMoving)
            SetIdle();
    }

    private void AttackCheck()
    {
        if (hitBox.enemyTargets.Count > 0 && !isAttacking && !isInOpponentArea)
        {
            StartCoroutine(TagRoutine());
        }
        else if (hitBox.friendlyTargets.Count > 0 && !isAttacking && isInOpponentArea && !tagged)
        {
            StartCoroutine(JailBreakTagRoutine());
        }
    }

    public void MustReturnToBaseCheck()
    {
        if (mustReturnToBase)
        {
            if (isBlue)  // Blue
            {
                // Must return to a negative X position
                navTarget = new Vector3(-5, 0, transform.position.z);
                SetDestination(navTarget);
                if (transform.position.x < 0)
                {
                    mustReturnToBase = false;
                    tagged = false;
                    meshRenderer.material = normalMat;
                    if (autoResumeNavTarget)
                        SetDestination(destinationWhenTagged);

                    EnableScripts();
                }

            }
            else if (isRed)  // Red
            {
                // Must return to a positive X position
                navTarget = new Vector3(5, 0, transform.position.z);
                SetDestination(navTarget);
                if (transform.position.x > 0)
                {
                    mustReturnToBase = false;
                    tagged = false;
                    meshRenderer.material = normalMat;
                    if (autoResumeNavTarget)
                        SetDestination(destinationWhenTagged);

                    EnableScripts();
                }
            }
        }
    }

    private void IntializeBattlefieldInfo()
    {
        redFlag = battleInfo.redFlag;
        redJail = battleInfo.redJail;
        blueFlag = battleInfo.blueFlag;
        blueJail = battleInfo.blueJail;
    }

    private IEnumerator TagRoutine()
    {
        isAttacking = true;
        animator.applyRootMotion = false;
        animator.Play("Attack 01");
        yield return new WaitForSeconds(0.25f);
        HitCheck();
        yield return new WaitForSeconds(0.75f);
        animator.applyRootMotion = true;
        isAttacking = false;
    }

    private IEnumerator JailBreakTagRoutine()
    {
        isAttacking = true;
        animator.applyRootMotion = false;
        animator.Play("Attack 01");
        yield return new WaitForSeconds(0.25f);
        FriendlyHitCheck();
        yield return new WaitForSeconds(0.75f);
        animator.applyRootMotion = true;
        isAttacking = false;
    }

    private void HitCheck()
    {
        LittleDude targ = hitBox.ClosestEnemyTarget();
        if (targ != null)
        {
            targ.Tag();
        }
    }

    private void FriendlyHitCheck()
    {
        LittleDude targ = hitBox.ClosestFriendlyTarget();
        if (targ != null)
        {
            targ.JailBreak();
        }
    }

    private void Tag()
    {
        destinationWhenTagged = navTarget;
        tagged = true;
        DisableScripts();
        if (hasFlag)
        {
            flagSlot.GetChild(0).GetComponent<Flag>().Drop();
            hasFlag = false;
        }
        meshRenderer.material = taggedMat;
    }

    private void NavAgentSettingsCheck()
    {
        if (navAgent.stoppingDistance != stopDistance)
            navAgent.stoppingDistance = stopDistance;
    }


    private bool startedFallOver;
    private void GoToJailCheck()
    {
        if (tagged && !inJail && !mustReturnToBase)
        {
            if (!startedFallOver)
            {
                startedFallOver = true;
                StartCoroutine(FallOverRoutine());
            }

            float dist;
            if (isBlue)
            {
                if (navTarget != redJail.position)
                    navTarget = redJail.position;
                
                dist = Vector3.Distance(transform.position, redJail.position);
            }
            else 
            {
                if (navTarget != blueJail.position)
                    navTarget = blueJail.position;

                dist = Vector3.Distance(transform.position, blueJail.position);
            }

            if (dist < 5) // change this to scale with how many are in jail.
                inJail = true;


        }
    }

    private IEnumerator FallOverRoutine()
    {
        animationOverride = true;
        SetDestination(transform.position);
        SetFallOver();
        FreezeRigidbody();
        animator.applyRootMotion = false;
        yield return new WaitForSeconds(2);
        animationOverride = false;

        UnfreezeRigidbody();
        animator.applyRootMotion = true;

        if (isBlue)
        {
            if (navTarget != redJail.position)
                navTarget = redJail.position;
        }
        else
        {
            if (navTarget != blueJail.position)
                navTarget = blueJail.position;
        }

        SetWalk();
    }

    public void JailBreak()
    {
        
        if (tagged && !mustReturnToBase)
        {
            inJail = false;
            //startedFallOver = false;
            meshRenderer.material = mustReturnToBaseMat;
            hitBox.friendlyTargets.Clear();
            mustReturnToBase = true;
            inJail = false;
        }
    }

    public void FlagPickupCheck()
    {
        // check if blue or red
        // check if can pick up / return
        // check if their flag has been picked up
        if (isBlue)
        {
            if (!tagged)
            {
                // Blue Flag Return
                if (!blueFlag.atHomeBase && battleInfo.blueFlagDropped)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
                    foreach (var hitCollider in hitColliders)
                    {

                        if (hitCollider.gameObject.CompareTag("Blue Flag"))
                        {
                            hitCollider.gameObject.GetComponent<Flag>().Return();
                            print("Blue flag return attempted");
                        }
                    }
                }

                // Red Flag Pickup
                if (!redFlag.pickedUp)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.gameObject.CompareTag("Red Flag"))
                        {
                            hitCollider.gameObject.GetComponent<Flag>().PickUp(flagSlot, this);
                            hasFlag = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (!tagged)
            {
                // Red Flag Return
                if (!redFlag.atHomeBase && battleInfo.redFlagDropped)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.gameObject.CompareTag("Red Flag"))
                        {
                            hitCollider.gameObject.GetComponent<Flag>().Return();
                            print("Red flag return attempted");
                        }
                    }
                }

                // Blue Flag Pickup
                if (!blueFlag.pickedUp)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.gameObject.CompareTag("Blue Flag"))
                        {
                            blueFlag.PickUp(flagSlot, this);
                            hasFlag = true;
                        }
                    }
                }
            }
        }
        
    }

    private void FreezeRigidbody()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.mass = 1000;
    }

    private void UnfreezeRigidbody()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        rb.mass = 1;
    }

    public void DisableScripts()
    {
        if (scriptsToOverride.Length != 0)
        {
            scriptsDisabled = true;
            for (int i = 0; i < scriptsToOverride.Length; i++)
            {
                if (scriptsToOverride[i] != null)
                    scriptsToOverride[i].enabled = false;
            }
        }
    }

    public void EnableScripts()
    {
        if (scriptsToOverride.Length != 0)
        {
            scriptsDisabled = false;
            for (int i = 0; i < scriptsToOverride.Length; i++)
            {
                if (scriptsToOverride[i] != null)
                    scriptsToOverride[i].enabled = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (navAgent.path != null)
        {
            NavMeshPath path = navAgent.path;
            if (path != null)
            {
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                }
            }
        }
    }
}
