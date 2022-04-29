using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    [SerializeField] private List<Material> _skins = new();

    public List<Material> Skins => _skins;
}
