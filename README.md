# Colour-Blindness-Test
An Ishihara Test used to identify people with red-green colour blindness 

It was made using Unity.

The Ishihara test program files were made with three different scenes. First, the Persistent scene is responsible for storing the question and answer data. 
Next, the Menu Screen scene displays the instructions to the Ishihara test and contains a button which transitions to the Game scene. 
The Game scene contains four panels: the NewUserPanel, PatientIdPanel, QuestionPanel, and RoundOverPanel. 

The NewUserPanel enables the nurses to register the details of new patients into the system. 
After which, the patient ID number is entered in by the nurses in the PatientIdPanel to ensure that the results of the test are registered to the specific patient. 
Once this is done, the QuestionPanel displays the 17 multiple choice questions. 
Lastly, the RoundOverPanel saves the results to the MongoDB database when the Menu button is clicked. 

Upon start-up, the program enters the Persistent scene. This scene contains the DataController gameobject with the DataController script attached to it. 
The relevant snippet of the C# script can be seen in Figure 3.10. This script helps transition to the MenuScreen scene using SceneManager; 
and stores the question and answers data. Moreover, the script prevents the DataController gameobject from being destroyed when transitioning to 
different scenes by allowing it to persist in a separate scene called DontDestroyOnLoad. 
This scene was designed this way to ensure that there is only one instance of the DataController when the program is running.

Under the Unity inspector, the questions and answers can be modified easily. The program was designed this way to make handling
a large amount of question data easy to configure. The relevant Booleans can also be assigned to different answers, which are used to tally the
Ishihara test scores during the Game scene. This was achieved by using [System.Serializable] and linking the AnswerData, QuestionData and RoundData scripts.

The RoundData script enables the amount of points added when an answer is clicked to be set in the inspector. The QuestionData script enables the 
Ishihara plates to be assigned to each question. Lastly, the AnswerData allows the answer text options to be set. It also creates seven boolean values which are assigned 
to the answers in the following ways:

i. isCorrect was assigned to correct answers
ii. isControl was assigned to the correct answer for the demonstration plate
iii. isSeverity was assigned to answers that indicated total colour blindness
iv. isMildR assigned to answers that indicated red-weakness
v. isStrongR was assigned to answers that indicated red-blindness
vi. isMildG assigned to answers that indicated green-weakness
vii. isStrongG was assigned to answers that indicated green-blindness

When the Ishihara test program is run, the scene changes to the Menu Screen scene almost instantaneously. This scene gives the instructions for the test; 
and contains the MenuScreenController gameobject with the MenuScreenController script attached to it. 
The start button was assigned the StartGame() OnClick function, which transitions to the Game scene when clicked.
