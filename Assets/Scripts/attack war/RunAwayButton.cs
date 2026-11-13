using UnityEngine;
using UnityEngine.UI;

public class RunAwayButton : MonoBehaviour
{
    public Button runAwayBear;
    public Button btnBear;

    public Button runAwayFly;
    public Button BTNfly;

    public Button runAwayelefnt;
    public Button BTNelefnt;

    public Button runAwaywom;
    public Button BTNwom;


    void Start()
    {
        if (runAwayBear != null) runAwayBear.onClick.AddListener(OnRunAwayButtonClicked);
        if (runAwayelefnt != null) runAwayelefnt.onClick.AddListener(OnRunAwayButtonClickedelefnt);
        if (runAwaywom != null) runAwaywom.onClick.AddListener(OnRunAwayButtonClickedWOM);
        if (runAwayFly != null) runAwayFly.onClick.AddListener(OnRunAwayButtonClickedfly);
    }

    void OnRunAwayButtonClicked()
    {
        // ÇáÈÍË Úä Ãí æÍÔ áÏíå ÇáÊÇÛ "monsterEnemy"
        GameObject enemyObj = GameObject.FindGameObjectWithTag("monsterEnemy");
        if (enemyObj != null)
        {
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.RunAway(); // ÇÓÊÏÚÇÁ ÏÇáÉ ÇáåÑæÈ
            }
        }
        else
        {
            Debug.LogWarning("áã íÊã ÇáÚËæÑ Úáì Ãí æÍÔ ÈÇáÊÇÛ 'monsterEnemy'");
        }
        btnBear.interactable = false;
    }

    void OnRunAwayButtonClickedelefnt()
    {
        // ÇáÈÍË Úä Ãí æÍÔ áÏíå ÇáÊÇÛ "monsterEnemy"
        GameObject enemyObj = GameObject.FindGameObjectWithTag("monsterEnemy");
        if (enemyObj != null)
        {
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.RunAway(); // ÇÓÊÏÚÇÁ ÏÇáÉ ÇáåÑæÈ
            }
        }
        else
        {
            Debug.LogWarning("áã íÊã ÇáÚËæÑ Úáì Ãí æÍÔ ÈÇáÊÇÛ 'monsterEnemy'");
        }
        BTNelefnt.interactable = false;
    }

    void OnRunAwayButtonClickedfly()
    {
        // ÇáÈÍË Úä Ãí æÍÔ áÏíå ÇáÊÇÛ "monsterEnemy"
        GameObject enemyObj = GameObject.FindGameObjectWithTag("monsterFly");
        if (enemyObj != null)
        {
            y3soobEnemy enemyScript = enemyObj.GetComponent<y3soobEnemy>();
            if (enemyScript != null)
            {
                enemyScript.RunAway(); // ÇÓÊÏÚÇÁ ÏÇáÉ ÇáåÑæÈ
            }
        }
        else
        {
            Debug.LogWarning("áã íÊã ÇáÚËæÑ Úáì Ãí æÍÔ ÈÇáÊÇÛ 'monsterfly'");
        }
        BTNfly.interactable = false;

    }

    void OnRunAwayButtonClickedWOM()
    {
        // ÇáÈÍË Úä Ãí æÍÔ áÏíå ÇáÊÇÛ "monsterEnemy"
        GameObject enemyObj = GameObject.FindGameObjectWithTag("monsterEnemy");
        if (enemyObj != null)
        {
            EnemyRNG_Advanced enemyScript = enemyObj.GetComponent<EnemyRNG_Advanced>();
            if (enemyScript != null)
            {
                enemyScript.RunAway(); // ÇÓÊÏÚÇÁ ÏÇáÉ ÇáåÑæÈ
            }
        }
        else
        {
            Debug.LogWarning("áã íÊã ÇáÚËæÑ Úáì Ãí æÍÔ ÈÇáÊÇÛ 'monsterEnemy'");
        }
        BTNwom.interactable = false;

    }
}
