using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

[RequireComponent(typeof(SpeakerTag), typeof(MethodTag), typeof(CooldownTag))]
public class Tags : MonoBehaviour
{
    private readonly Dictionary<string, ITag> _map = new ();

    private void Start()
    {
        _map.Add("speaker", GetComponent<SpeakerTag>());
        _map.Add("method", GetComponent<MethodTag>());
        _map.Add("cooldown", GetComponent<CooldownTag>());
    }

    public ITag GetValue(string key)
    {
        return _map.GetValueOrDefault(key);
    }



}
