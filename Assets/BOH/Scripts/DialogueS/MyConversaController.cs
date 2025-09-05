using Conversa.Demo;
using UnityEngine;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine.UI;

namespace BOH.Conversa
{
    public class MyConversaController : MonoBehaviour
    {
        [SerializeField] private Conversation conversation;
		[SerializeField] private MyUIController uiController;

		[Header("Buttons")]
		[SerializeField] private Button restartConversationButton;
		[SerializeField] private Button updateSavepointButton;
		[SerializeField] private Button loadSavepointButton;

		private ConversationRunner runner;
		private string savepointGuid = string.Empty;

		private void Start()
		{
			//runner = new ConversationRunner(conversation);
			//runner.OnConversationEvent.AddListener(HandleConversationEvent);
			restartConversationButton.onClick.AddListener(HandleRestartConversation);
			if (updateSavepointButton != null)
			{
				updateSavepointButton.onClick.AddListener(HandleUpdateSavepoint);
				updateSavepointButton.interactable = false;
			}

			if (loadSavepointButton != null)
			{
				loadSavepointButton.onClick.AddListener(HandleLoadSavepoint);
			}
		}
		
		public void StartConversation(Conversation convo)
		{
			if (convo == null) { Debug.LogWarning("[Conversa] StartConversation: null convo"); return; }
			StopCurrentConversation();

			conversation = convo;
			runner = BuildRunner(convo);
			if (runner == null)
			{
				Debug.LogError("[Conversa] Could not create ConversationRunner");
				return;
			}

			uiController?.Show();
			runner.Begin();
			if (updateSavepointButton != null) updateSavepointButton.interactable = true;
		}
		
		public void StopCurrentConversation()
		{
			if (runner == null) return;

			runner.OnConversationEvent.RemoveListener(HandleConversationEvent);
			runner = null;

			uiController?.Hide();
			if (updateSavepointButton != null) updateSavepointButton.interactable = false;
		}

		private ConversationRunner BuildRunner(Conversation convo)
		{
			var r = new ConversationRunner(convo);
			r.OnConversationEvent.AddListener(HandleConversationEvent);
			return r;
		}
		
		private void HandleConversationEvent(IConversationEvent e)
		{
			switch (e)
			{
				case MessageEvent messageEvent:
					HandleMessage(messageEvent);
					break;
				case ChoiceEvent choiceEvent:
					HandleChoice(choiceEvent);
					break;
				case ActorMessageEvent actorMessageEvent:
					HandleActorMessageEvent(actorMessageEvent);
					break;
				case ActorChoiceEvent actorChoiceEvent:
					HandleActorChoiceEvent(actorChoiceEvent);
					break;
				case UserEvent userEvent:
					HandleUserEvent(userEvent);
					break;
				case I18nEvent i18NEvent:
					HandleI18nEvent(i18NEvent);
					break;
				case EndEvent _:
					HandleEnd();
					break;
				
			}
		}

		private void HandleActorMessageEvent(ActorMessageEvent evt)
		{

			var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
			if (evt.Actor is DemoActor avatarActor)
				uiController.ShowMessage(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Advance);
			else
				uiController.ShowMessage(actorDisplayName, evt.Message, null, evt.Advance);
		}

		private void HandleActorChoiceEvent(ActorChoiceEvent evt)
		{
			var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
			if (evt.Actor is DemoActor avatarActor)
				uiController.ShowChoice(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Options);
			else
				uiController.ShowChoice(actorDisplayName, evt.Message, null, evt.Options);
		}

		private void HandleMessage(MessageEvent e) => uiController.ShowMessage(e.Actor, e.Message, null, () => e.Advance());

		private void HandleI18nEvent(I18nEvent e)
		{
			var message = LocaleManager.Instance.Get(e.MessageKey);
			uiController.ShowMessage(e.Actor.DisplayName, message, e.Actor.Avatar, () => e.Advance());
		}

		private void HandleChoice(ChoiceEvent e) => uiController.ShowChoice(e.Actor, e.Message, null, e.Options);

		private static void HandleUserEvent(UserEvent userEvent)
		{
			if (userEvent.Name == "Food bought")
				Debug.Log("We can use this event to update the inventory, for instance");
		}

		private void HandleRestartConversation()
		{
			runner.Begin();
			Debug.Log("Restarting conversation");
			if (updateSavepointButton != null)
				updateSavepointButton.interactable = true;
		}

		private void HandleLoadSavepoint()
		{
			runner.BeginByGuid(savepointGuid);
		}

		private void HandleUpdateSavepoint()
		{
			savepointGuid = runner.CurrentNodeGuid;
		}

		private void HandleEnd()
		{
			uiController.Hide();
			if (updateSavepointButton != null)
				updateSavepointButton.interactable = false;
		}

		
    }
}