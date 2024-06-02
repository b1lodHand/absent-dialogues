# Nodes

Nodes are the base elements of this tool. As I said, this is a *node based* dialogue system. So, In this section of the documentation, you will get a brief explanation of all node types included by default in this tool.

## Root Node
![Imgur Image](https://imgur.com/dXWjyTo.jpg)

[Root Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.RootNode.html) is automatically created when you create a new graph. This node acts as a starting point in the graph. Because of that, its **UNDESTROYABLE!!!**.


## Dialogue Part Node
![Imgur Image](https://imgur.com/v1ryjfE.jpg)

[Dialogue Part Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DialoguePartNode.html) works similar to the [Root Node](#root-node). This is also a starting point node **BUT** it is not the starting point of the graph itself. It is a starting point of the node chain it is connected to.



## Goto Node <a name = "goto-node"></a>
![Imgur Image](https://imgur.com/GH0crEP.jpg)

[Goto Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.GotoNode.html) is used to seperate the node chains to have a more clear graph window. The only mission of this node is to find the target **Dialogue Part Node** and teleport to it.

> [!IMPORTANT]
> The relation between the **Goto Node** and **Dialogue Part Node** is *string prone* for now. So, be careful while using them.


## Fast Speech Node
![Imgur Image](https://imgur.com/ENtvpBh.jpg)

[Fast Speech Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.FastSpeechNode.html) is one of the two speech nodes in this tool. This node is used to display a speech that has no options.


## Decision Speech Node
![Imgur Image](https://imgur.com/1x1jBK3.jpg)

[Decision Speech Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html) does the same thing with the [Fast Speech Node](#fast-speech-node). The only difference between them is that the Decision Speech Node has options the player acn choose from.


## Option Block
![Imgur Image](https://imgur.com/Uw7Ckf1.jpg)

[Option Block](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Option.html) is not a Node itself. It is used to display the options of a [Decision Speech Node](#decision-speech-node). Text written in the text field of this block will be displayed as an option.


## Condition Node
![Imgur Image](https://imgur.com/cX67Zaf.jpg)

[Condition Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ConditionNode.html) lets you to progress in different ways in the dialogue depending on some conditions. The conditions are based on the **Variable Comparers** like the ones in the [Option Block](#option-block).


## Action Node
![Imgur Image](https://imgur.com/kN2nR9h.jpg)

[Action Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ActionNode.html) is pretty self explanatory. It invokes some actions when it gets *reached* by the dialogue.

>[!TIP]
>There is also a property of Action Node called [CustomAction()](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ActionNode.html#com_absence_dialoguesystem_internals_ActionNode_CustomAction) which is a virtual method. And also **Action Node** one of the two node subtypes which you can derive new subtypes from (they aren't [sealed](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed)). So, you can derive from Action Node to have a node that has some more specific actions to perform.


## Sticky Note Node
![Imgur Image](https://imgur.com/JRQ6CzE.jpg)

[StickyNote](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.StickyNoteNode.html) is not really a node. It is derived from the node base type and that's all. It has no effect on the dialogue flow. But you can use it to leave some useful notes in the graph view. 


## Title Node
![Imgur Image](https://imgur.com/rk6yLRB.png)

[Title Node](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.TitleNode.html) is nearly the same as [Sticky Note Node](#sticky-note-node). It is just bigger and easy to see.

## What's Next?

This section is ended. Go to [Components](https://b1lodhand.github.io/absent-dialogues/docs/getting-started/components.html) to continue.