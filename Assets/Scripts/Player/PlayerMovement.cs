using System;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveForce;
    [SerializeField] private Rigidbody2D rigid2D;

    public PredictionRigidbody2D PredictRigid2D;
    private InputAction moveAction;

    void Awake()
    {
        PredictRigid2D = ObjectCaches<PredictionRigidbody2D>.Retrieve();
        PredictRigid2D.Initialize(rigid2D);
    }    

    void Start() 
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void OnDestroy()
    {
        ObjectCaches<PredictionRigidbody2D>.StoreAndDefault(ref PredictRigid2D);
    }

    public override void OnStartNetwork()
    {
        base.TimeManager.OnTick += TimeManager_OnTick;
        base.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }

    public override void OnStopNetwork()
    {
        base.TimeManager.OnTick -= TimeManager_OnTick;
        base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
    }

    private void TimeManager_OnTick()
    {
        RunInputs(CreateReplicationData());
        CreateReconcile();
    }

    [Replicate]
    private void RunInputs(ReplicateData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        Vector2 force = new Vector2(data.Horizontal, data.Vertical) * moveForce;
        PredictRigid2D.AddForce(force);

        PredictRigid2D.Simulate();
    }

    private ReplicateData CreateReplicationData()
    {
        if (!base.IsOwner || moveAction == null)
            return default;
        
        // float horizontal = Input.GetAxis("Horizontal");
        // float vertical = Input.GetAxis("Vertical");
        // ReplicateData data = new(horizontal, vertical);

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        ReplicateData data = new(moveInput.x, moveInput.y);

        return data;
    }

    private void TimeManager_OnPostTick()
    {
        CreateReconcile();
    }

    public override void CreateReconcile()
    {
        ReconcileData reconcileData = new(PredictRigid2D);
        ReconcileState(reconcileData);
    }

    [Reconcile]
    private void ReconcileState(ReconcileData data, Channel channel = Channel.Unreliable)
    {
        PredictRigid2D.Reconcile(data.PredictionRigidbody);
    }

    public struct ReplicateData : IReplicateData
    {
        public float Horizontal;
        public float Vertical;
        public ReplicateData(float horizontal, float vertical) : this()
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    public struct ReconcileData : IReconcileData
    {
        //PredictionRigidbody is used to synchronize rigidbody states
        //and forces. This could be done manually but the PredictionRigidbody
        //type makes this process considerably easier. Velocities, kinematic state,
        //transform properties, pending velocities and more are automatically
        //handled with PredictionRigidbody.
        public PredictionRigidbody2D PredictionRigidbody;
        
        public ReconcileData(PredictionRigidbody2D pr) : this()
        {
            PredictionRigidbody = pr;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }
}
