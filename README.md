# self-driving-car
## Introduction: 
This project explores the development of a self-driving car in a simulated 3D environment, created with Unity and powered by ML-Agents and PyTorch. Our goal was to train an intelligent agent capable of navigating different driving scenarios — starting from simple race circuits and gradually progressing to more complex environments such as parking lots and highways.

The car agent learns through reinforcement learning, using Unity’s ML-Agents toolkit as the bridge between the simulation environment and the PyTorch-based training pipeline. By rewarding safe and efficient driving behaviors (e.g., staying on track, avoiding collisions, following lanes), the model gradually acquires driving skills that generalize to increasingly challenging settings.

Beyond the technical challenge, this project was also about learning new concepts together and having fun. From experimenting with training setups to designing creative scenarios, we treated the process as much as a collaborative learning journey as a step into the world of autonomous driving research.

## Concepts & Environment Setup  

To illustrate the basic concepts behind our training process, let’s take the **race track map** as an example environment.  

### The Environment  
Here is the **race track** used for training:  
<img width="1792" height="820" alt="screenshot track" src="https://github.com/user-attachments/assets/d2c2176c-0a12-4132-95d0-64ef8b51f486" />  

And here is the **3D car model** used as the agent:  
<img width="1750" height="808" alt="screenshot car" src="https://github.com/user-attachments/assets/4724f0c1-b1f8-4529-9879-5affb5982fb1" />  

### Reinforcement Learning Setup  
Since the car is trained using **reinforcement learning**, it is crucial to provide a way for the agent to understand whether it is moving in the right direction. For this, we placed a sequence of **invisible checkpoints** along the track:  
<img width="1857" height="828" alt="screenshot with checkpoints" src="https://github.com/user-attachments/assets/600b05f8-6ca2-4c0c-9852-c575321e3a30" />  

- The car **receives a reward** when it successfully passes through a checkpoint.  
- The car **receives a penalty** when it collides with barriers or walls, teaching it to avoid unsafe driving.  

<img width="1383" height="647" alt="screenshot wall object" src="https://github.com/user-attachments/assets/3f0a576b-9a04-4116-a361-b8156ef56295" />  

### Sensors  
To make decisions, the car is equipped with a set of **ray-based sensors**:  
<img width="1180" height="795" alt="screenshot car sensors" src="https://github.com/user-attachments/assets/95cac05f-90ba-4d62-859f-0f101e434cd4" />  

- These sensors allow the car to detect **nearby walls** to avoid collisions.  
- They also help the car sense **checkpoints** and decide whether to continue forward or adjust its trajectory.  

In short, the environment provides a mix of **positive rewards** (for progress) and **negative rewards** (for mistakes), which guide the car toward learning safe and efficient driving behaviors.  
