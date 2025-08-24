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
				done: res =>
				{
					if (!res.success)
					{


						// coinsEarnedThisGame is your match result (could be 0)
						/*PlayFabAchievementService.ReportCoinsAndEvaluate(GlobalValue.Coin, res =>
						{
							if (!res.success) return;

							if (res.newAchievements != null && res.newAchievements.Length > 0)
							{
								// Show just one popup (first new achievement)
								var newId = res.newAchievements[0];

								// For the popup, we need the defs to resolve the correct icon.
								PlayFabAchievementService.GetAchievementsOverview(ov =>
								{
									if (ov == null || !ov.success) { Debug.LogWarning("Overview failed"); return; }
									if (!ov.success) return;

									// Build a sprite resolver (you already have this inside the screen)
									// For a quick tect map or use Resources.Load<Sprite>(est, create a ScriptableObjname).
									System.Func<string, Sprite> resolve = (n) => Resources.Load<Sprite>(n);

									NewAchievementPopup.ShowOne(newId, ov.defs, resolve, onOK: () =>
									{
										// Ack so it doesn't show again
										PlayFabAchievementService.AckAchievementNotifications(new[] { newId }, _ => { }, err => { });
									});

								}, err => Debug.LogError(err.GenerateErrorReport()));
							}
						});*/
						ShowAchievementPopupsAfterMatch(GlobalValue.Coin);












						// On resume / after login
						PlayFabChallengeService.ResolveChallengeOnReopen(res =>
						{
							if (!res.success) return;

							if (res.state == "expired")
							{

								WinnerPopup.ShowFromResolve(
									res,
									null,                 // optional: PlayFabId -> DisplayName
									"WinnerScreen",      // your prefab path in Resources
									onOk: () =>
									{
										// anything you want after closing (e.g., refresh UI)
									}
								);
								//MessagePopup.ShowPopup($"Challenge over. You {res.outcome}. My:{res.myScore} Opp:{res.oppScore}");
								// Now your side is cleared; opponent will clear when they call this too.
							}
							else if (res.state == "active")
							{
								Debug.Log($"Still active. My:{res.myScore} Opp:{res.oppScore}");
							}
							else
							{
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











		void ShowAchievementPopupsAfterMatch(int coinsEarned)
		{
			PlayFabAchievementService.ReportCoinsAndEvaluate(coinsEarned, res =>
			{
				// fallback to overview->pending if no fresh unlocks in this call
				void ShowFromOverview()
				{
					PlayFabAchievementService.GetAchievementsOverview(ov =>
					{
						if (ov == null || !ov.success) return;
						if (ov.pending == null || ov.pending.Count == 0) return;

						var id = ov.pending[0];

						// use your own resolver (match your sprite names/paths)
						System.Func<string, Sprite> resolve = n => Resources.Load<Sprite>("UI/Achievements/" + n);
						NewAchievementPopup.ShowOne(id, ov.defs, resolve, onOK: () =>
						{
							PlayFabAchievementService.AckAchievementNotifications(new[] { id }, _ => { }, err => { });
						});
					}, err => Debug.LogError(err.GenerateErrorReport()));
				}

				if (res == null || !res.success) { ShowFromOverview(); return; }

				// If this match unlocked something, prefer showing that
				if (res.newAchievements != null && res.newAchievements.Length > 0)
				{
					var newId = res.newAchievements[0];
					PlayFabAchievementService.GetAchievementsOverview(ov =>
					{
						if (ov == null || !ov.success) return;
						System.Func<string, Sprite> resolve = n => Resources.Load<Sprite>("UI/Achievements/" + n);
						NewAchievementPopup.ShowOne(newId, ov.defs, resolve, onOK: () =>
						{
							PlayFabAchievementService.AckAchievementNotifications(new[] { newId }, _ => { }, err => { });
						});
					}, err => Debug.LogError(err.GenerateErrorReport()));
				}
				else
				{
					// No new unlocks in this match → show any queued pending from earlier
					ShowFromOverview();
				}
			},
			err => Debug.LogError(err.GenerateErrorReport()));
		}

	}
	



}