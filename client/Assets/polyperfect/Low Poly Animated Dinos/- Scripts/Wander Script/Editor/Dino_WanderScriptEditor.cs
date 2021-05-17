using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PolyPerfect
{
    [CustomEditor(typeof(Dino_WanderScript))]
    [CanEditMultipleObjects]
    public class Dino_WanderScriptEditor : Common_WanderScriptEditor { }
    }