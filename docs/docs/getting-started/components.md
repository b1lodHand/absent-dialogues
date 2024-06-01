# Components

Components are essential when you're working with a tool which is integrated right into unity. In this section of the documentation, you will get a simple explanation of built-in components that come this tool.


## Dialogue Displayer

![Imgur Image](https://imgur.com/Uw9L3VB.png)

**Dialogue Displayer** is the only built-in way of displaying a dialogue on screen. But you can come up with your own solutions with the knowledge from the [mechanism](mechanism.md) section.

>[!WARNING]
>**Dialogue Displayer** is a singleton! Use it with this knowledge.


## Option Text

![Imgur Image](https://imgur.com/nTjOKrW.png)

**Option Text** is a needed component when working with the built-int **Dialogue Displayer.** It is simply responsible for the *clicking* action, *index* holding and of course, displaying the option.

>[!TIP]
>**Dialogue Instance** is designed to work with the **Dialogue Player**. You can read [Mechanism](mechanism.md) page for more details about that class.


## Dialogue Instance

![Imgur Image](https://imgur.com/CqkZrWU.png)

**Dialogue Instance** is the best way to play a dialogue in your game. It is pretty straight-forward to use. All you have to do is: attach, drag, press play. And you're done!

>[!TIP]
>The *player* section below in the inspector is only visible in **play mode.**


## Dialogue Sounds Player

![Imgur Image](https://imgur.com/8kGJWRF.png)

**Dialogue Sounds Player** is the *extension* component responsible for playing sounds from an audio source, using the information from **AdditionalSpeechData.**


## Dialogue Animations Player

![Imgur Image](https://imgur.com/KZMhiR4.png)

**Dialogue Animations Player** is the *extension* component responsible for managing animator controller over a dialogue, using the information from **AdditionalSpeechData**

## Dialogue Input Handler (Legacy)

![Imgur Image](https://imgur.com/23WeTkq.png)

**Dialogue Input Handler (Legacy)** is the *extension* component responsible for handling the input comes from player during the dialogue. It is marked **legacy** because it only works with the old input system of unity.