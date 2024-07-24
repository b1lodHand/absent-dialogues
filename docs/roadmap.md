# Roadmap

In this section of the documentation, you will see some details about the past and the future of this project.

## Latest Version [(v1.2.0)](https://github.com/b1lodHand/absent-dialogues/releases/latest)
* **Import/Export** support. It is called **'Backup System'**.
* **Bugfix:** Fixed a bug which caused NullReferenceException when **IPerformDelayedClone**.DelayedClone function got called.
* **Bugfix:** Fixed a bug that caused **DialogueDisplayer** to not hold occupience state truly when an instance exits dialogue.
* **Bugfix:** Fixed a bug that caused **DialoguePlayer** to not invoke *OnContinue* action when the state is *DialoguePlayerState.WillExit*.
* **Changed** interface **IContainSpeech** completely.
* **Changed** some properties to get/set from get-only.
* **Added** the *event* action **OnExitDialogue** to **DialogueInstance**.
* **Added** first option selection feature to **DialogueDisplayer**. Now it selects the first option when a dialogue displayed.


## Upcoming Version
* An extension based on the **absent-saves** package.
* **Bugfix:** Error when opening the Dialogue Editor Window for the first time. 

## Already on the Plans
* **Optimization:** Breaking the **circular dependency** between nodes and dialogue.
* **Shift+Space** inspector glitch fix.
* **New images and gifs** for the documentation.
* **New icons** for the components, nodes and etc.
* Access modifier **regulation.** *(prob. not backwards capable).*
* Optimization in code.

## In the Future (Without Certainty)
* **Localization** for dialogues.
* **Integration** with some other applications.
* Generic option feature.
* Dialogue override graph feature.