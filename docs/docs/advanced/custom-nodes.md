# Custom Nodes

In this section of the documentation, you will learn how to create your own custom nodes. Let's start.

When creating custom nodes, you have three options:

1. Using the **Node** class as base.
2. Using the **ConditionNode** class as base.
3. Using the **ActionNode** class as base.

Well, the **2nd** and the **3rd** options will come with their limitations. Only a little part of their inheritable members are editable. So, if you want to create a unique node, best option is the **1st** one.

You can find a more detailed information about **2nd** and **3rd** options in the [API Documentation.](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.html)

## Using Node as Base

Well, the node class has a lot to inherit from. You can find a better list in it's [API Documentation](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.Node.html) section. You can use those metyhodsand properties to create any node you want.

>[!NOTE]
>If you want to create a node which has some really unique features, you might need to modify the [NodeView](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.NodeView.html) class. This is only necessary if your custom node needs to display or hide some data in the graph.

## What's Next?

This section is ended. Go to [Custom Dialogue Extensions](https://b1lodhand.github.io/absent-dialogues/docs/advanced/custom-dialogue-extensions.html) to continue.