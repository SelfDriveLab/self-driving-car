using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;

[RequireComponent(typeof(PrometeoCarController))]
public class ControlCarAgent : Agent {

    [SerializeField] private Transform checkpointTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer planeMeshRenderer;
    
    private PrometeoCarController prometeoCarController;
    private Rigidbody rb;

    public override void Initialize() {
        prometeoCarController = GetComponent<PrometeoCarController>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin(){
        transform.localPosition = new Vector3(30, 0, 25);
        transform.localRotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        prometeoCarController.CancelInvoke("DecelerateCar");
        prometeoCarController.CancelInvoke("RecoverTraction");
        
        prometeoCarController.ResetSteeringAngle();
        prometeoCarController.ThrottleOff();
        prometeoCarController.RecoverTraction();
        prometeoCarController.isDrifting = false;
        prometeoCarController.isTractionLocked = false;
        prometeoCarController.deceleratingCar = false;
    }
    
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(checkpointTransform.position);
    }

    public override void OnActionReceived(ActionBuffers actions){
        int throttleAction = actions.DiscreteActions[0]; // Branch 0: 0=Coast, 1=Forward, 2=Reverse
        int steerAction = actions.DiscreteActions[1]; // Branch 1: 0=Straight, 1=Left, 2=Right
        int handbrakeAction = actions.DiscreteActions[2]; // Branch 2: 0=Off, 1=On

        bool isCoasting = (throttleAction == 0);
        bool isAccelerating = (throttleAction == 1);
        bool isReversing = (throttleAction == 2);
        bool isGoingStraight = (steerAction == 0);
        bool isTurningLeft = (steerAction == 1);
        bool isTurningRight = (steerAction == 2);
        bool isHandbrakeOn = (handbrakeAction == 1);

        if (isAccelerating){
            prometeoCarController.CancelInvoke("DecelerateCar");
            prometeoCarController.deceleratingCar = false;
            prometeoCarController.GoForward();
        }
        else if (isReversing){
            prometeoCarController.CancelInvoke("DecelerateCar");
            prometeoCarController.deceleratingCar = false;
            prometeoCarController.GoReverse();
        }
        else if (isCoasting){
            prometeoCarController.ThrottleOff();
            if (!isHandbrakeOn && !prometeoCarController.deceleratingCar){
                prometeoCarController.InvokeRepeating("DecelerateCar", 0f, 0.1f);
                prometeoCarController.deceleratingCar = true;
            }
        }

        if (isTurningLeft){
            prometeoCarController.TurnLeft();
        }
        else if (isTurningRight){
            prometeoCarController.TurnRight();
        }
        else if (isGoingStraight && prometeoCarController.steeringAxis != 0f){
            prometeoCarController.ResetSteeringAngle();
        }

        if (isHandbrakeOn){
            prometeoCarController.CancelInvoke("DecelerateCar");
            prometeoCarController.deceleratingCar = false;
            prometeoCarController.Handbrake();
        }
        else{
            prometeoCarController.RecoverTraction();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allows manual control using keyboard for testing Discrete Actions

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W)) {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S)) {
            discreteActions[0] = 2;
        }
        else {
            discreteActions[0] = 0;
        }

        if (Input.GetKey(KeyCode.A)) {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D)) {
            discreteActions[1] = 2;
        }
        else {
            discreteActions[1] = 0;
        }
        
        if (Input.GetKey(KeyCode.Space)) {
            discreteActions[2] = 1;
        }
        else {
            discreteActions[2] = 0;
        }
    }

    
    
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Obstacle")) {
            SetReward(-1f);
            planeMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform == checkpointTransform) {
            SetReward(1f);
            planeMeshRenderer.material = winMaterial;
            EndEpisode();
        }
    }
}
