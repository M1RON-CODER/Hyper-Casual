using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum Buildings { CashRegister }
    private Buildings _buildingType;

    public Buildings BuildingType => _buildingType;
}
