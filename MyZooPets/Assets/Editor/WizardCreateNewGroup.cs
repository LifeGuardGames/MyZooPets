using UnityEngine;
using UnityEditor;

public class WizardCreateNewGroup : ScriptableWizard
{

    [MenuItem("GameObject/Create New Group")]
    static void CreateWizard ()
    {
        ScriptableWizard.DisplayWizard<WizardCreateNewGroup>("Create Group", "Create");
    }

    void OnWizardCreate()
    {

    }
}
