# Components

Components are essential when you're working with a tool which is integrated right into unity. In this section of the documentation, you will get a simple explanation of built-in components that come this tool.


## Dialogue Displayer

![Imgur Image](https://imgur.com/Uw9L3VB.png)

[Dialogue Displayer](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueDisplayer.html) is the only built-in way of displaying a dialogue on screen. But you can come up with your own solutions.

>[!WARNING]
>**Dialogue Displayer** is a singleton! Use it with this knowledge.


## Option Text

![Imgur Image](https://imgur.com/nTjOKrW.png)

[Option Text](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueOptionText.html) is a needed component when working with the built-int [Dialogue Displayer](#dialogue-displayer) It is simply responsible for the *clicking* action, *index* holding and of course, displaying the option.

>[!TIP]
>**Dialogue Instance** is designed to work with the [Dialogue Player](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialoguePlayer.html). You can read [Mechanism](https://b1lodhand.github.io/absent-dialogues/docs/getting-started/mechanism.html) page for more details about that class.


## Dialogue Instance

![Imgur Image](https://imgur.com/CqkZrWU.png)

[Dialogue Instance](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInstance.html) is the best way to play a dialogue in your game. It is pretty straight-forward to use. All you have to do is: attach, drag, press play. And you're done!

>[!TIP]
>The *player* section below in the inspector is only visible in **play mode.**


## Dialogue Sounds Player

![Imgur Image](https://imgur.com/8kGJWRF.png)

[Dialogue Sounds Player](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueSoundsPlayer.html) is the *extension* component responsible for playing sounds from an audio source, using the information from [AdditionalSpeechData](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.AdditionalSpeechData.html).


## Dialogue Animations Player

![Imgur Image](https://imgur.com/KZMhiR4.png)

[Dialogue Animations Player](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueAnimationsPlayer.html) is the *extension* component responsible for managing animator controller over a dialogue, using the information from [AdditionalSpeechData](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.AdditionalSpeechData.html).

## Dialogue Input Handler (Legacy)

![Imgur Image](https://imgur.com/23WeTkq.png)

[Dialogue Input Handler (Legacy)](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInputHandler_Legacy.html) is the *extension* component responsible for handling the input comes from player during the dialogue. It is marked **legacy** because it only works with the old input system of unity.

>[!NOTE]
>All of the *extension* components above are derived from the [DialogueExtensionBase](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueExtensionBase.html) class. See [Custom Dialogue Extensions](https://b1lodhand.github.io/absent-dialogues/docs/advanced/custom-dialogue-extensions.html) for more information..

## What's Next?

**Well, you know everything you need to know about this tool right now.** But if you want to modify it for your own use, you can go to [Advanced](https://b1lodhand.github.io/absent-dialogues/docs/advanced/custom-nodes.html) to see how you can do it easily.