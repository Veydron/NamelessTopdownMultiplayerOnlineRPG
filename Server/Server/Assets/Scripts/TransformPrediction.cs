using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;
using FishNet.Object.Synchronizing;

namespace FishNet.Example.Prediction.Transforms
{
    public class TransformPrediction : NetworkBehaviour
    {

        private Pathfinding pathfinding;
        public Vector3 targetPosition;
        private List<Vector3> waypoints;
        private int waypointIndex = 0;
        public float targetDistanceCheck = 0.2f;
        private PlayerAnimation _playerAnimation;
        public bool isMoving = false;
        Vector2 normalizedDirections;
        private Transform[] allchildren;
        private GameObject Hair;

        private GameObject Face;

        private GameObject HairSit;

        private GameObject FaceSit;

        private int oldLookDirection;

        private CombatSystem _CB;

        [SyncVar(Channel = Transporting.Channel.Unreliable, ReadPermissions = ReadPermission.OwnerOnly, SendRate = 0f)]
        public bool isSitting;
        public void SittingDown(bool Value)
        {
            ChangeObjectOnOff(Hair, !Value);
            ChangeObjectOnOff(Face, !Value);
            ChangeObjectOnOff(HairSit, Value);
            ChangeObjectOnOff(FaceSit, Value);
            isSitting = Value;
            ChangeSit(Value);
        }

        [ObserversRpc(BufferLast = true, IncludeOwner = true)]
        private void ChangeSit(bool Value)
        {
            if (allchildren == null)
            {
                Debug.Log("AllChildren Null error");
                return;
            }
            allchildren[4].gameObject.SetActive(!Value);
            allchildren[5].gameObject.SetActive(!Value);
            allchildren[6].gameObject.SetActive(Value);
            allchildren[7].gameObject.SetActive(Value);
            isSitting = Value;
        }

        private void ChangeObjectOnOff(GameObject Obj, bool Value)
        {
            if (Obj != null)
            {
                Obj.SetActive(Value);
            }

        }

        void Start()
        {

        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _CB = this.gameObject.GetComponent<CombatSystem>();

            pathfinding = new Pathfinding(30, 30);

            //Debug.Log("Child Count: " + transform.childCount);

            allchildren = this.GetComponentsInChildren<Transform>();

            //    for (int i = 0; i < allchildren.Length-1; i++){ 
            //        Debug.Log(i + " = " + allchildren[i].name);
            //    }

            Face = allchildren[4].gameObject;
            Hair = allchildren[5].gameObject;
            FaceSit = allchildren[6].gameObject;
            HairSit = allchildren[7].gameObject;

            //FaceSit.SetActive(false);
            //HairSit.SetActive(false);

            //  allchildren = null;

            SittingDown(isSitting);
        }


        void Update()
        {


            if (Input.GetMouseButton(0) && base.IsOwner)
            {
                if (_CB.ActiveTarget == CombatSystem.MouseTarget.TILE)
                {
                    SearchPath(_CB.targetTile);
                }
                //SetTargetPosition();
                //SearchPath(targetPosition);
            }
            if (Input.GetKeyDown(KeyCode.Space) && base.IsOwner)
            {
                _playerAnimation.Jump();
            }

        }
        void SetTargetPosition()
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float point = 0f;
            if (plane.Raycast(ray, out point))
            {
                targetPosition = ray.GetPoint(point);
            }
        }
        void SearchPath(Vector3 TargetTile)
        {
            if (TargetTile == null){
                return;
            }
            targetPosition = TargetTile;

            if (waypoints != null)
            {
                if (waypointIndex > waypoints.Count - 1)
                {
                    waypointIndex = waypoints.Count - 1;
                }
                waypoints = pathfinding.FindPath(waypoints[waypointIndex], targetPosition);
            }
            else
            {
                waypoints = pathfinding.FindPath(transform.position, targetPosition);
            }

            waypointIndex = 0;
        }


        /// <summary>
        /// Data on how to move.
        /// This is processed locally as well sent to the server for processing.
        /// Any inputs or values which may affect your move should be placed in your own MoveData.
        /// The structure type may be named anything. Classes can also be used but will generate garbage, so structures
        /// are recommended.
        /// </summary>
        public struct MoveData
        {
            public float Horizontal;
            public float Vertical;

            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }

        /// <summary>
        /// Data on how to reconcile.
        /// Server sends this back to the client. Once the client receives this they
        /// will reset their object using this information. Like with MoveData anything that may
        /// affect your movement should be reset. Since this is just a transform only position and
        /// rotation would be reset. But a rigidbody would include velocities as well. If you are using
        /// an asset it's important to know what systems in that asset affect movement and need
        /// to be reset as well.
        /// </summary>
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public ReconcileData(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        #region Serialized.
        /// <summary>
        /// How many units to move per second.
        /// </summary>
        [Tooltip("How many units to move per second.")]
        [SerializeField]
        private float _moveRate = 5f;
        #endregion

        #region Private.
        /// <summary>
        /// The last MoveData client sent.
        /// </summary>
        private MoveData _clientMoveData;
        #endregion

        private void Awake()
        {
            /* Prediction is tick based so you must
             * send datas during ticks. You can use whichever
             * tick best fits your need, such as PreTick, Tick, or PostTick.
             * In most cases you will send/move using Tick. For rigidbodies
             * you will send using PostTick. I subscribe to ticks using
             * the InstanceFinder class, which finds the first NetworkManager
             * loaded. If you are using several NetworkManagers you would want
             * to subscrube in OnStartServer/Client using base.TimeManager. */
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnUpdate += TimeManager_OnUpdate;
            _playerAnimation = GetComponent<PlayerAnimation>();
        }

        private void OnDestroy()
        {
            //Unsubscribe as well.
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnUpdate -= TimeManager_OnUpdate;
            }
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                /* Call reconcile using default, and false for
                 * asServer. This will reset the client to the latest
                 * values from server and replay cached inputs. */
                Reconciliation(default, false);
                /* CheckInput builds MoveData from user input. When there
                 * is no input CheckInput returns default. You can handle this
                 * however you like but Move should be called when default if
                 * there is no input which needs to be sent to the server. */
                CheckInput(out MoveData md);
                /* Move using the input, and false for asServer.
                 * Inputs are automatically sent with redundancy. How many past
                 * inputs will be configurable at a later time.
                 * When a default value is used the most recent past inputs
                 * are sent a predetermined amount of times. It's important you
                 * call Move whether your data is default or not. FishNet will
                 * automatically determine how to send the data, and run the logic. */
                Move(md, false);
            }
            if (base.IsServer)
            {
                /* Move using default data with true for asServer.
                 * The server will use stored data from the client automatically.
                 * You may also run any sanity checks on the input as demonstrated
                 * in the method. */
                Move(default, true);
                /* After the server has processed input you will want to send
                 * the result back to clients. You are welcome to skip
                 * a few sends if you like, eg only send every few ticks.
                 * Generate data required on how the client will reset and send it by calling your Reconcile
                 * method with the data, again using true for asServer. Like the
                 * Replicate method (Move) this will send with redundancy a certain
                 * amount of times. If there is no input to process from the client this
                 * will not continue to send data. */
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation);
                Reconciliation(rd, true);
            }
        }


        private void TimeManager_OnUpdate()
        {
            /* Move every frame using the clients last
             * movedata and the frames delta. This will move
             * the client smoothly while only sending data
             * every tick. */
            if (base.IsOwner)
                MoveWithData(_clientMoveData, Time.deltaTime);
        }


        /// <summary>
        /// A simple method to get input. This doesn't have any relation to the prediction.
        /// </summary>
        private void CheckInput(out MoveData md)
        {
            md = default;

            if (waypoints == null)
            {
                return;
            }
            if (waypointIndex > waypoints.Count - 1)
            {
                return;
            }

            float distance = DistanceAB();

            if (distance < targetDistanceCheck)
            {
                waypointIndex = waypointIndex + 1;
                if (waypointIndex > waypoints.Count - 1)
                {
                    return;
                }
            }

            if (isSitting)
            {
                normalizedDirections = NormalizedDirection(targetPosition);
            }
            else
            {
                normalizedDirections = NormalizedDirection(waypoints[waypointIndex]);
            }

            float horizontal = normalizedDirections.x;
            float vertical = normalizedDirections.y;

            /*
            isMoving = (horizontal != 0f || vertical != 0f );
            _playerAnimation.SetMoving(isMoving);

            if (isMoving == false){
                Debug.Log("isMoving = False");
                return;
            }
            Debug.Log("isMoving = True");
            */

            //Make movedata with input.
            md = new MoveData()
            {
                Horizontal = horizontal,
                Vertical = vertical
            };
        }

        private float DistanceAB()
        {
            Vector2 startVector = new Vector2(transform.position.x, transform.position.z);
            Vector2 endVector = new Vector2(waypoints[waypointIndex].x, waypoints[waypointIndex].z);
            float distance = Vector2.Distance(startVector, endVector);
            return distance;
        }

        private Vector2 NormalizedDirection(Vector3 end)
        {
            Vector2 startVector = new Vector2(transform.position.x, transform.position.z);
            Vector2 endVector = new Vector2(end.x, end.z);
            Vector2 normalized = endVector - startVector;

            return normalized.normalized;
        }



        /// <summary>
        /// Replicate attribute indicates the data is being sent from the client to the server.
        /// When Replicate is present data is automatically sent with redundancy.
        /// The replay parameter becomes true automatically when client inputs are
        /// being replayed after a reconcile. This is useful for a variety of things,
        /// such as if you only want to show effects the first time input is run you will
        /// do so when replaying is false.
        /// </summary>
        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            /* You can check if being run as server to
             * add security checks such as normalizing
             * the inputs. */
            if (asServer)
            {
                //Sanity check!
            }
            /* You may also use replaying to know
             * if a client is replaying inputs rather
             * than running them for the first time. This can
             * be useful because you may only want to run
             * VFX during the first input and not during
             * replayed inputs. */
            if (!replaying)
            {
                //VFX!
            }

            /* Run logic as if it were an offline game.
            * It's important to use TickDelta as your deltaTime
            * when running in the simulation tick. */
            if (asServer || replaying)
                MoveWithData(md, (float)base.TimeManager.TickDelta);
            /* When running as client and not
             * replaying set the clientMoveData to what
             * was passed in. This will be used in OnUpdate
             * to move the client smoothly over each frame
             * rather than at the tickDelta. This really
             * only applies for non-physics based movement such as
             * transform or character controller. If you were
             * to see the rigidbody example you'd notice the
             * movement is done in this method for both
             * client and server, and not over time in update.
             * While you may be able to do some sort of interpolation
             * in update with a rigidbody, that's up to you to
             * figure out. */
            else if (!asServer)
                _clientMoveData = md;
        }

        private void MoveWithData(MoveData md, float delta)
        {
            Vector3 move = new Vector3(md.Horizontal, 0f, md.Vertical);
            if (isSitting)
            {
                transform.position += (Vector3.zero * _moveRate * delta);
                isMoving = false;
                waypoints = null;
                _playerAnimation.SetMoving(isMoving);
                //SittingDown(true);

                if (Face != null)
                {

                }
                else
                {
                    Debug.Log("A) Face IS null");
                }

                if (move != Vector3.zero)
                {
                    move = new Vector3(md.Horizontal, 90f, md.Vertical);
                    var rotationTemp = Quaternion.LookRotation(move);
                    float eulerAngleY = rotationTemp.eulerAngles.y;

                    if (eulerAngleY > -22.5f && eulerAngleY < 22.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }
                    if (eulerAngleY > 22.5f && eulerAngleY < 67.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 45, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 45, 0);
                    }
                    if (eulerAngleY > 67.5f && eulerAngleY < 112.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 90, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 90, 0);
                    }
                    if (eulerAngleY > 112.5f && eulerAngleY < 157.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 135, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 135, 0);
                    }
                    if (eulerAngleY > 157.5f && eulerAngleY < 202.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 180, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 180, 0);
                    }
                    if (eulerAngleY > 202.5f && eulerAngleY < 247.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 225, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 225, 0);
                    }
                    if (eulerAngleY > 247.5f && eulerAngleY < 292.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 270, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 270, 0);
                    }
                    if (eulerAngleY > 292.5f && eulerAngleY < 337.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 315, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 315, 0);
                    }
                    if (eulerAngleY > 337.5f && eulerAngleY < 382.5f)
                    {
                        HairSit.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        FaceSit.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }
                }

                int pufferVar = Mathf.RoundToInt(HairSit.transform.eulerAngles.y);
                int rotationLookDirection = pufferVar;
                int fl = rotationLookDirection;
                //Debug.Log("HairSit.Rot.Y: " + rotationLookDirection);
                //Debug.Log("oldLookD: " + oldLookDirection);

                if (oldLookDirection == 0)
                {
                    if (fl == 90 || fl == 135 || fl == 180)
                    {
                        oldLookDirection = 45;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 270 || fl == 225)
                    {
                        oldLookDirection = 315;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 45)
                {
                    if (fl == 135 || fl == 180 || fl == 225)
                    {
                        oldLookDirection = 90;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 315 || fl == 270)
                    {
                        oldLookDirection = 0;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 90)
                {
                    if (fl == 180 || fl == 225 || fl == 270)
                    {
                        oldLookDirection = 135;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 0 || fl == 315)
                    {
                        oldLookDirection = 45;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 135)
                {
                    if (fl == 225 || fl == 270 || fl == 315)
                    {
                        oldLookDirection = 180;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 45 || fl == 0)
                    {
                        oldLookDirection = 90;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 180)
                {
                    if (fl == 270 || fl == 315 || fl == 0)
                    {
                        oldLookDirection = 225;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 90 || fl == 45)
                    {
                        oldLookDirection = 135;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 225)
                {
                    if (fl == 315 || fl == 0 || fl == 45)
                    {
                        oldLookDirection = 270;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 135 || fl == 90)
                    {
                        oldLookDirection = 180;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 270)
                {
                    if (fl == 0 || fl == 45 || fl == 90)
                    {
                        oldLookDirection = 315;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 180 || fl == 135)
                    {
                        oldLookDirection = 225;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }
                if (oldLookDirection == 315)
                {
                    if (fl == 45 || fl == 90 || fl == 135)
                    {
                        oldLookDirection = 0;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                    if (fl == 225 || fl == 180)
                    {
                        oldLookDirection = 270;
                        transform.rotation = Quaternion.Euler(0, oldLookDirection, 0);
                        return;
                    }
                }                  

            }

            else
            {
                transform.position += (move * _moveRate * delta);
                isMoving = (md.Horizontal != 0f || md.Vertical != 0f);
                _playerAnimation.SetMoving(isMoving);
                //SittingDown(false);

                if (Face != null)
                {

                }
                else
                {
                    Debug.Log("B) Face IS null");
                }

                if (move != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(move);
                }

                if (!isMoving)
                {
                    int A = (int)this.transform.position.x;
                    int B = (int)this.transform.position.z;
                    Vector2 soll = new Vector2((float)A + 0.5f, (float)B + 0.5f);
                    Vector3 ist = new Vector2(this.transform.position.x, this.transform.position.z);
                    float different = Vector2.Distance(ist, soll);
                    transform.position = new Vector3(soll.x, 0, soll.y);
                }
            }
        }

        /// <summary>
        /// A Reconcile attribute indicates the client will reconcile
        /// using the data and logic within the method. When asServer
        /// is true the data is sent to the client with redundancy,
        /// and the server will not run the logic.
        /// When asServer is false the client will reset using the logic
        /// you supply then replay their inputs.
        /// </summary>
        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            //transform.rotation = rd.Rotation;
        }


    }


}