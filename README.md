# Project Title

Cognitive Augmented Reality Assistant (CARA) for the Blind

## Introduction

This Unity project includes a VR environment is developed for benchmarking various types of visual and cognitive prostheses. 
It is intially developed as a platform to compare CARA and vOICe, two drastically different visual/cognitive prostheses.
However, the flexibility of the environment makes it possible to implement and test a wide variety of prosthetic systesms.

### Prerequisites

Software:
This VR environment is developed in Windows 10 with Unity 2017.1.0f3 with free assets downloaded from Unity asset store. 
In principle it can opened by newer versions of Unity but it has not been tested.

Hardware:
This environment runs on the HTC Vive VR headset and with the Microsoft Xbox One S wireless controller.
Other combinations of Steam VR compatible headset and game controllers can be used.

### Installing

Download the entire respository and open with Unity editor. 

You can either choose to run in the Unity editor or build the project and run as a stand-alone application.

## Running experiments

The current version includes 3 tasks: aiming, chair finding and key pick-up. 
Run the environment with either of the methods described above and the experiment starts. 
The experimenter can switch task by pressing numpad key 0-3, with 0 activating practice environment and 1-3 activationg tasks 1-3.
By default, CARA is implemented. Press "M" to mute CARA to test other systems (e.g. vOICe). To unmute, press "U".

Frame by frame logs of subject position, orientation, target position, key experiment parameters are saved as text files in D:/VR/ by default.

For more details please see the publication [to be added].

## Authors

* **Yang Liu**

## License
This project is licensed under the MIT license
## Acknowledgments
