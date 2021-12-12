using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShaderUtility
{
    private static Dictionary<Shader, int> _shaderColorPropertyIDDict = new Dictionary<Shader, int>
    {
        [Shader.Find("Standard")] = Shader.PropertyToID("_Color")
    };
    
    public static void SetColorForHP(Shader shader, MaterialPropertyBlock block, float normalizedHp)
    {
        if (_shaderColorPropertyIDDict.TryGetValue(shader, out var nameID))
        {
            var lerped = Color.Lerp(Color.red, Color.white, normalizedHp);
            block.SetColor(nameID, lerped);
        }
        else if (shader.name.Contains("Legacy Shaders/"))
        {
            var lerped = Color.Lerp(Color.red, Color.white, normalizedHp);
            block.SetColor("_MainColor", lerped);
        }
        else
        {
            var shaderName = shader != null? shader.name: "nullShader";
            Debug.LogError($"{shaderName} cannot find colorProperty");
        }
    }
}
