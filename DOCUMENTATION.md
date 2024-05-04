# Documentation of absent-dialogues

## Welcome

Welcome to the documentation. Here I will teach you how to use the tool in a basic way. The code itself is also documented with the *summery* feature of C#. So, you can find some advanced information on using the **internal code** of the tool. Now, let's start.

## Nodes

### Root Node
![Imgur Image](https://imgur.com/FwGmUyj.png)

**Root Node** is automatically created when you create a new graph. This node acts as a starting point in the graph. Because of that, its **UNDESTROYABLE!!!**.

</br>

### Dialogue Part Node
![Imgur Image](https://imgur.com/ETY999l.png)

**Dialogue Part Node** works similar to the **Root Node**. This is also a starting point node **BUT** it is not the starting point of the graph itself. It is a starting point of the node chain it is connected to.

**Properties:**
- *(str.)* **Dialogue Part Name:** This property is used by the **Goto Node** in order to find and teleport to this node. Read [Goto Node](#goto-node) for more details.

</br>

### Goto Node <a name = "goto-node"></a>
![Imgur Image](https://imgur.com/3g168Py.png)

**Goto Node** is used to seperate the node chains to have a more clear graph window. The only mission of this node is to find the target **Dialogue Part Node** and teleport to it.

**Properties:**
- *(str.)* **Target Dialogue Part Name:** **Goto Node** finds a **Dialogue Part Node** which has it's target property matchin to this property and teleports to it.

> [!IMPORTANT]
> The relation between the **Goto Node** and **Dialogue Part Node** is *string prone* for now. So, be careful while using them.

</br>

### Fast Speech Node
![Imgur Image](https://imgur.com/PmbBsqx.png)

**Fast Speech Node** is one of the two speech nodes in this tool. This node is used to display a speech that has no options.

**Properties:**
- *(Person)* **Person:** The person who speaks.
- *(str.)* **Speech:** Speech will be displayed.
- *(AdditionalSpeechData)* **Additional Speech Data:** This property contains some extra data that can be used when the speech gets displayed.

</br>

### Decision Speech Node
![Imgur Image](https://imgur.com/4BmngHT.png)

**Decision Speech Node** does the same thing with the **Fast Speech Node**. The only difference between them is that the **Decision Speech Node** also has options the player acn choose from.

**Properties:**
- *(Person)* **Person:** The person who speaks.
- *(str.)* **Speech:** Speech will be displayed.
- *(AdditionalSpeechData)* **Additional Speech Data:** This property contains some extra data that can be used when the speech gets displayed.
- *(List<Option>)* **Options:** Options will be displayed.

</br>

### Option Block
![Imgur Image](https://imgur.com/TkUgZ4I.png)

**Option Block** is not a Node itself. It is used to display the options of a **Decision Speech Node**. Text written in the text field of this block will be displayed as an option.

> [!TIP]
> The panel above the text field is an integrated version of the **VariableComparer** from one of my other tools. For more details, you can hover the block in the Dialogue Editor or go to: https://github.com/b1lodHand/absent-variables

</br>

### Condition Node
![Imgur Image](https://imgur.com/oeW8wXF.png)

**Condition Node** lets you to progress in different ways in the dialogue depending on some conditions. The conditions are based on the **Variable Comparers** like the ones in the **Option Block**.

**Properties:**
- *(ProcessorType)* **Processor:** Lets you select the way comparers work.
- *(List<VariableComparer>)* **Comparers:** List of the entire comparers used to process the output boolean.

</br>

### Action Node
![Imgur Image](https://imgur.com/jBtwkVz.png)

**Action Node** is pretty self explanatory. It invokes some actions when it gets *reached* by the dialogue.

**Properties:**
- *(List<VariableSetter>)* **VB Actions:** Actions that depend on my **absent-variables** package. These can be used to set some variables which can be used troughout the game.
- *(UnityEvent)* **Unity Events:** UnityEvents that **DO NOT** accept scene objects.

>[!TIP]
>There is also a property of **Action Node** called **CustomAction()** which is a virtual method. And also **Action Node** one of the two node subtypes which you can derive new subtypes from (they aren't *sealed*). So, you can derive from **Action Node** to have a node that has some more specific actions to perform.

</br>

### StickyNote Node
![Imgur Image](https://imgur.com/1Zx5oKb.png)

**StickyNote** is not really a node. It is derived from the node base type and that's all. It has no effect on the dialogue flow. But you can use it to leave some useful notes in the graph view. 

</br>

### Title Node
![Imgur Image](https://imgur.com/fhoK3Zd.png)

**Title Node** is nearly the same as **StickyNote**. It is just bigger and easy to see.

## OOPS!

Rest of the documentation is still in progress. Thank you for your patience!
