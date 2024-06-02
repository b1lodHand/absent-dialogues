# Custom Dialogue Extensions

In this section of the documentation, you will learn how to write your own dialogue extensions. Let's start.

To create custom dialogue extensions, you have to inherit the [DialogueExtensionBase](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueExtensionBase.html) class.

I highy recommend reading the [API Documentation](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueExtensionBase.html) to have a better understanding of the *extension* concept.

If you want to add your custom extension to the **'Add Extension'** menu in the context menu of the [DialogueInstance](https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.DialogueInstance.html), you can copy and paste the code below **inside** your new extension class.

```c#
#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/DialogueInstance/Add Extension/EXTENSION_NAME")]
        static void AddExtensionMenuItem(UnityEditor.MenuCommand command)
        {
            DialogueInstance instance = (DialogueInstance)command.context;
            instance.AddExtension<EXTENSION_TYPE>();
        }
#endif
```

## What's Next?

**You've done it!** You've read all the way to the end, mate. I really appreciate you. Know you don't just know how this tool works in a detailed way but you also know how you can modify it. 

There is nothing else you need to read. But if you want to know how you can publish any modifications you've made on the tool to the open-source community, I'd suggest you reading [Contributing](https://b1lodhand.github.io/absent-dialogues/docs/introduction/contributing.html) section.

And again, thank you for reading this all the way!

**Have a nice day!**