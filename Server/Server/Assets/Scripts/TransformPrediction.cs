﻿using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace FishNet.Example.Prediction.Transforms
{
    public class TransformPrediction : NetworkBehaviour
    {

        private Pathfinding pathfinding;
        public Vector3 targetPosition;
        private List<Vector3> waypoints;
        private int waypointIndex = 0;
        public float targetDistanceCheck = 0.2f;
        void Start()
        {
            pathfinding = new Pathfinding(30, 30);
        }
        void Update()
        {
            if (Input.GetMouseButtonUp(0) && base.IsOwner)
            {
                SetTargetPosition();
                SearchPath();
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
        void SearchPath()
        {
            if (waypoints != null)
            {
                if (waypointIndex > waypoints.Count-1){
                    waypointIndex = waypoints.Count-1;
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
 
            if (waypoints == null){
                return;
            }
            if (waypointIndex > waypoints.Count -1){
                return;
            }
            float distance = DistanceAB();

            if (distance < targetDistanceCheck){
                waypointIndex = waypointIndex + 1;
                if (waypointIndex > waypoints.Count-1){
                    return;
                }
            }

            Vector2 normalizedDirections = NormalizedDirection();
            float horizontal = normalizedDirections.x;
            float vertical = normalizedDirections.y;

            //No input to send.
            if (horizontal == 0f && vertical == 0f)
                return;

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

        private Vector2 NormalizedDirection()
        {
            Vector2 startVector = new Vector2(transform.position.x, transform.position.z);
            Vector2 endVector = new Vector2(waypoints[waypointIndex].x, waypoints[waypointIndex].z);
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
            transform.position += (move * _moveRate * delta);
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
            transform.rotation = rd.Rotation;
        }


    }


}