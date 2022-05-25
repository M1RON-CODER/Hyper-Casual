using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSON : MonoBehaviour
{
    private static string _progressFileName = "Progress.json";
    private static string _cashRegisterFileName = "CashRegister.json";

    public static void SaveProgress(List<Progress.Object> objects)
    {
        var json = JsonConvert.SerializeObject(objects, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        
        File.WriteAllText($"{Application.dataPath}/Save/{_progressFileName}", json);
    }

    public static void SaveCashRegister(List<CashRegister> cashRegisters)
    {
        var json = JsonConvert.SerializeObject(cashRegisters, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        File.WriteAllText($"{Application.dataPath}/Save/{_cashRegisterFileName}", json);
    }

    public static List<Progress.Object> ReadFileProgress(List<Progress.Object> objects)
    {
        string path = $"{Application.dataPath}/Save/{_progressFileName}";
        if (!File.Exists(path))
        {
            SaveProgress(objects);
            return objects;
        }

        var json = JsonConvert.DeserializeObject<List<Progress.Object>>(File.ReadAllText(path));
        for (int i = 0; i < json.Count; i++)
        {
            for (int j = 0; j < json[i].progressPoints.Count; j++)
            {
                objects[i].progressPoints[j].SetPrice(json[i].progressPoints[j].Cost);
            }
        }

        return objects;
    }

    public static List<CashRegister> ReadFileCashRegister(List<CashRegister> cashRegisters)
    {
        string path = $"{Application.dataPath}/Save/{_cashRegisterFileName}";
        if (!File.Exists(path))
        {
            SaveCashRegister(cashRegisters);
            return cashRegisters;
        }

        var json = JsonConvert.DeserializeObject<List<CashRegister>>(File.ReadAllText(path));
        for (int i = 0; i < json.Count; i++)
        {
            cashRegisters[i].Cash.Initialize(json[i].Cash.CashAmount);
        }

        return cashRegisters;
    }    
}
