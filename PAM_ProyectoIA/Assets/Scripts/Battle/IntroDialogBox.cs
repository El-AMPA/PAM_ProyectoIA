using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogBox : MonoBehaviour
{
	[SerializeField] string introMessage;
	[SerializeField] int lettersPerSecond;
	[SerializeField] Text dialogText;
	[SerializeField] BattleSystem battleSystem;
	[SerializeField] Text continueText;

    private void Start()
    {
		StartCoroutine(introTextRoutine());
    }

	private IEnumerator introTextRoutine()
    {
		yield return TypeDialog(introMessage);
		continueText.gameObject.SetActive(true);
		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
		battleSystem.StartBattle();
		gameObject.SetActive(false);
    }

    public void SetDialog(string dialog)
	{
		dialogText.text = dialog;
	}

	public IEnumerator TypeDialog(string dialog)
	{
		dialogText.text = "";
		foreach(var letter in dialog.ToCharArray())
		{
			dialogText.text += letter;
			yield return new WaitForSeconds(1f / lettersPerSecond);
		}

		yield return new WaitForSeconds(1f);
	}
}
