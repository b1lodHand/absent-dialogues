# Custom Dialogue Extensions

In this section of the documentation, you will learn how to write your own dialogue extensions. Let's start.

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

There is nothing else you need to read. But if you want to know how you can publish any modifications you've made on the tool to the open-source community, I'd suggest you reading [Contributing](../introduction/contributing.md) section.

And again, thank you for reading this all the way!

**Have a nice day!**