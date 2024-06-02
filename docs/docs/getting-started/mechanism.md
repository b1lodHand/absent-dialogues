# Mechanism

In this section of the documentation, I'll be explaining you the base mechanism this tool relies on. I highly recommend you to read this section if you want to **contribute** to this tool or just wnat to **expand** the tool for your own use. And also, before starting, I recommend you to read the [Basic Setup](https://b1lodhand.github.io/absent-dialogues/docs/introduction/basic-setup.html) section before reading this one. Let's give it a go!

## The Graph

![Imgur Image](https://imgur.com/BURGJAA.png)

Here is the *demo* graph that comes with the tool itself. As you can see, there are **nodes**, a **VariableBank**, and an **inspector**. You will be learning more about these in the further pages, so I won't explain all of them now.

Instead, I will be covering the **whole concept** of this tools working principle in a superficial way.

So, when you create a graph, you create a new scriptable object called a **Dialogue.** And that dialogue comes with it's own another scriptable object, and that one's called the **Blackboard VB.** Blackboard VB is the same thing with the **VariableBank**, it just has a special name for itself.

The only things you need to know about nodes for now is: they have reference the nodes connected to them by the **right-side.** So the flow is one-way only. The only node that breaks this rule is the **Goto Node.**

![Imgur Image](https://imgur.com/nshGIFK.png#left) 

And as you can see, all of the nodes that you create also gets created as an usset under the dialogue you've created.

>[!WARNING]
>As you may noticed, the **RootNode** got created by itself, with the dialogue. It is because the root node is a must-have node per dialogue. So, don't try to delete it.

Now you know that everything is stored as **scriptable object** assets on the disk. Let's see when you press play.

## The Runtime System

![Imgur Image](https://imgur.com/tC2pYDR.png)

Here is the demo graph again, but it is on the flow (which means it is used by a dialogue instance).

The node with the **red** outline is the current node that gets displayed on screen, while the **gray** outliend ones are the 'already seen' ones.

But as you can notice from the dialogue field at he top of this window, this is not our original **'Demo.asset'** dialogue file. This is a **clone** dialogue.

Well that's how this system works. When you're using a **Dialogue Player**, it clones the dialogue you referenced on the construction. 

The purpose with this is to prevent you from losing any data on the flow. None of the changes you've made will write to the disk, they will stay on the memory.

>[!TIP]
>You can select a game object with a **Dialogue Instance** attached in order to see it's cloned dialogue live.

## The Dialogue Player

**Dialogue Player** is the class responsible for using a dialogue. What it does is pretty simple.

1. **Clone** the **referenced dialogue** (when constructor gets called).
2. Let you progress with the **'Continue(...)'** function.
3. Let you decide what to do with the current **state**, and the **data** of the current node.

And that's it!

Dialogue Player does not perform anything by itself. It only updates it's state in order for you to know what is going on with the flow.

I highly recommend reading the API Documentation of Dialogue player for further information about the functions and the state of it.

>[!WARNING]
>As said earlier, changes made during play mode (or in-game) **does not get saved by default.** So, you can write your own save logic over it, or use my [absent-saves](https://github.com/b1lodHand/absent-saves) package.

## What's Next?

This section is ended. Go to [Nodes](https://b1lodhand.github.io/absent-dialogues/docs/getting-started/nodes.html) to continue.