# IriTrack
This is the code for the paper IriTrack: Face Presentation Attack Detection using Iris Tracking. The paper link is [here](https://dl.acm.org/doi/abs/10.1145/3463515).

To run the program, you should run the scripts/work.ps1 in the PowerShell. Specifically, the command "scripts/work.ps1" represents that the system will show the randomly generated patterns to the users and detect face presentation attacks. The command "scripts/work.ps1 true" represents that the system will show the customized patterns to the users and detect face presentation attacks. 

After you run the command, you should click the "开始" button to turn on the system and then the user could start tracking the movement of the slider on the screen. 

![image](https://github.com/Elsawei123/IriTrack/blob/main/IMG/IMG.png)

The customized parameters (i.e., the angles and the lines) can be changed in the "scripts/1_Generation/result.txt". In this file, the first parameter represents the numbers of the angles and the following parameters are the angles, weights, and the lines participating in the generation of the pattern, respectively. The speed of the slipper can be changed in "scripts/2_Track/track.ps1".
