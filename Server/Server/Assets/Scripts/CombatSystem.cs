using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.Transforms;
using FishNet.Object;
using UnityEngine;

public class CombatSystem : NetworkBehaviour
{
    public enum State
    {
        IDLE,
        SIT,
        ATTACK,
    }

    public enum MouseTarget
    {
        TILE,
        PLAYER,
    }

    public Vector3 targetTile;
    private GameObject targetGameObject;
    public State CharacterStateIs = State.IDLE;
    public MouseTarget ActiveTarget = MouseTarget.TILE;
    private TransformPrediction _TF;
    private PlayerAnimation _PA;

    public float attackSpeed = 2f;

    public int attackRange = 2;

    public int weaponDamage = 40;

    public int maxHealth = 100;
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        _TF = this.gameObject.GetComponent<TransformPrediction>();
        _PA = this.gameObject.GetComponent<PlayerAnimation>();

        health = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        SetTargetTile();

        if (Input.GetMouseButton(0) && base.IsOwner)
        {
            TargetLogic();
        }

        if (CharacterStateIs == State.IDLE)
        {

        }

        if (CharacterStateIs == State.ATTACK)
        {
            AttackMode();
        }

    }

    void SetTargetTile()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float point = 0f;

        if (Physics.Raycast(ray, out hit, 100))
        {
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.2f);
            //Debug.Log(hit.transform.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                targetGameObject = hit.collider.gameObject;
                ActiveTarget = MouseTarget.PLAYER;
                //Debug.Log("its a Player, on tile: " + targetGameObject.transform.position);
                //MouseAttackIcon
            }

            return;
        }

        if (plane.Raycast(ray, out point))
        {
            targetTile = ray.GetPoint(point);
            ActiveTarget = MouseTarget.TILE;
            //Debug.Log("Tile is: " + targetTile);
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.2f);
        }

    }

    void TargetLogic()
    {
        if (ActiveTarget == MouseTarget.TILE)
        {
            CharacterStateIs = State.IDLE;
            _PA.Attacking(false);
            RPCIdle();
            return;
        }

        if (ActiveTarget == MouseTarget.PLAYER)
        {
            if (_TF.isMoving || _TF.isSitting)
            {
                return;
            }
            CharacterStateIs = State.ATTACK;
            //Debug.Log("PEW PEW PEW");
        }

    }

    void AttackMode()
    {
        _PA.Attacking(true);
        RPCAttacking();
    }


    [ServerRpc(RequireOwnership = true)]
    private void RPCIdle()
    {
        OBSERVEIdle();
    }
    [ServerRpc(RequireOwnership = true)]
    private void RPCAttacking()
    {
        if (_TF.isMoving || _TF.isSitting)
        {
            return;
        }
        OBSERVEAttacking();
    }


    [ObserversRpc(BufferLast = true, IncludeOwner = false)]
    private void OBSERVEIdle()
    {
        _PA.Attacking(false);
    }
    [ObserversRpc(BufferLast = true, IncludeOwner = false)]
    private void OBSERVEAttacking()
    {
        _PA.Attacking(true);
    }




}

/*


//- mouseover enemy
    raycast every update
//- mouse = attack symbol
    mouseIcon change from raycast object tag
//- click = attack mode
    set attack mode
    set target
//- if attack mode
    check if target in reach
        when not request pathfinding with -cells weapon Range
        walk to target
        halt x cells weaponrange bevor target
    check if target in reach
    attack
        play attack animation
        (do damage if hit)
    check if target in reach
    change to idlemode if sit or click to move
//- if idle mode
    do nothing
*/
