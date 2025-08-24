using UnityEngine;
using UnityEngine.UI;
namespace PhoenixaStudio
{
	public class MainMenu_GameOver : MonoBehaviour
	{
		//init the text object
		public Text score;
		public Text best;

		void Start()
		{
			//display the score and best value
			// e.g., p1Total, p2Total are your *current totals*
			PlayFabChallengeService.UpdateActiveScore(
				myScore: GameManager.Instance.Score,
				
				appendHistory: true,          // set false if you don't need per-round history
				done: res => {
					if (!res.success)
					{ 
						// On resume / after login
						PlayFabChallengeService.ResolveChallengeOnReopen(res => {
							if (!res.success) return;

							if (res.state == "expired") {

								WinnerPopup.ShowFromResolve(
									res,
									null,                 // optional: PlayFabId -> DisplayName
									"WinnerScreen",      // your prefab path in Resources
									onOk: () => {
										// anything you want after closing (e.g., refresh UI)
									}
								);
								//MessagePopup.ShowPopup($"Challenge over. You {res.outcome}. My:{res.myScore} Opp:{res.oppScore}");
								// Now your side is cleared; opponent will clear when they call this too.
							}
							else if (res.state == "active") {
								Debug.Log($"Still active. My:{res.myScore} Opp:{res.oppScore}");
							}
							else {
								Debug.Log("No active challenge.");
							}
						}, err => Debug.LogError(err.GenerateErrorReport()));

									Debug.LogWarning(res.message);
					}
				},
				fail: err => Debug.LogError(err.GenerateErrorReport())
			);

			score.text = GameManager.Instance.Score + "";
			best.text = "Best: " + GlobalValue.Best;
		}
	}
}