using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEditor;

public class Menu : MonoBehaviour
{
   [MenuItem("MyMenu/Do Something")]
   static void DoSomething()
   {
      Debug.Log("Doting Something...");
   }

   [MenuItem("MyMenu/Log Selected Transform Name")]
   static void LogSelectedTransformName()
   {
      Debug.Log("Seleceted Transform is on " + Selection.activeTransform.gameObject.name + " .");
   }

   [MenuItem("MyMenu/Log Selected Transform Name", true)]
   static bool ValidateLogSelectedTransformName()
   {
      return Selection.activeTransform != null;
   }

   [MenuItem("MyMenu/Do Something with a Shortcut Key %a")]
   static void DoSomethingWithAShortcutKey()
   {
      Debug.Log("Doing something with a Shortcut Key...");
   }

   [MenuItem("CONTEXT/Rigidbody/Double Mass")]
   static void DoubleMass(MenuCommand command)
   {
      Rigidbody rigidbody = (Rigidbody) command.context;
      rigidbody.mass = rigidbody.mass * 2;
      Debug.Log("Double Rigidbody's Mass to " + rigidbody.mass + " from Context Menu.");
   }

   [MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
   static void CreateCustomGameObject(MenuCommand menuCommand)
   {
      GameObject go = new GameObject("Custom GameObject");
      GameObjectUtility.SetParentAndAlign(go,menuCommand.context as GameObject);
      Undo.RegisterCompleteObjectUndo(go, "Create " + go.name);
      Selection.activeObject = go;
   }
   
}
