using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public int hp;
    public float x, y, z;
}

public class PlayerManager : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;
    
    [Header("UI")]
    public Button saveButton;
    public Button loadButton;
    public Text   hpText;
    public Text   posText;

    private string savePath;
    private Vector3 lastCheckpointPosition;

    void Start()
    {
        savePath = Application.persistentDataPath + "/playerData.json";
        if (saveButton != null) saveButton.onClick.AddListener(Save);
        if (loadButton != null) loadButton.onClick.AddListener(Load);

        Load();
    }

    void Update()
    {
            
        float moveX = Input.GetAxis("Horizontal") * 5f * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * 5f * Time.deltaTime;
        
        transform.Translate(moveX, 0, moveZ);
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hit(10);
        }
        
        UpdateText();
    }

    public void Hit(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        Debug.Log("Le joueur prend " + damage + " dégâts !");
    }
    
    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        Debug.Log("Dernier checkpoint enregistré : " + position);
    }

    public void Save()
    {
        Vector3 posToSave = (lastCheckpointPosition != Vector3.zero) ? lastCheckpointPosition : transform.position;
        
        PlayerData data = new PlayerData
        {
            hp = currentHP, 
            x = posToSave.x,
            y = posToSave.y,
            z = posToSave.z
        };
        
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);

        Debug.Log("Données sauvegardées (" + savePath + ")");
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            currentHP = data.hp;
            transform.position = new Vector3(data.x, data.y, data.z);
            Debug.Log("Données chargées : HP = " + currentHP);
        }
        else
        {
            currentHP = maxHP;
            Debug.Log("Aucune sauvegarde trouvée, HP réinitialisés.");
        }
        UpdateText();
    }
    
    void UpdateText()
    {
        if (hpText)
            hpText.text = "HP : " + currentHP + " / " + maxHP;
        
        if (posText)
            posText.text = "Position : " + transform.position.ToString("F2");
    }

    void GoToNearestCheckpoint()
    {
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        if (checkpoints.Length == 0)
        {
            Debug.Log("Aucun checkpoint trouvé !");
            return;
        }

        Checkpoint nearest = checkpoints[0];
        float minDist = Vector3.Distance(transform.position, nearest.transform.position);

        foreach (var cp in checkpoints)
        {
            float dist = Vector3.Distance(transform.position, cp.transform.position);
            if (dist < minDist)
            {
                nearest = cp;
                minDist = dist;
            }
        }

        transform.position = nearest.transform.position;
        Debug.Log("Téléporté au checkpoint le plus proche : " + nearest.name);
        UpdateText();
    }
}
