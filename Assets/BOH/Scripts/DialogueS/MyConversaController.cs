using BOH;
using Conversa.Demo;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine;

public class MyConversaController : MonoBehaviour
{
    [SerializeField] private Conversation conversation;
    [SerializeField] private MyUIController uiController;

    private ConversationRunner runner;

    private void Start()
    {
        runner = new ConversationRunner(conversation);
        runner.OnConversationEvent.AddListener(HandleConversationEvent);
    }

    public void Begin()
    {
        runner.Begin();
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

    private void HandleEnd()
    {
        uiController.Hide();
    }

    private void HandleUserEvent(UserEvent userEvent)
    {
        if (userEvent.Name == "Food bought")
            Debug.Log("We can use this event to update the inventory, for instance");
    }
    
    private void HandleI18nEvent(I18nEvent e)
    {
        var message = LocaleManager.Instance.Get(e.MessageKey);
        uiController.ShowMessage(e.Actor.DisplayName, message, e.Actor.Avatar, () => e.Advance());
    }

    private void HandleActorChoiceEvent(ActorChoiceEvent evt)
    {
        var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
        if (evt.Actor is DemoActor avatarActor)
            uiController.ShowChoice(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Options);
        else
            uiController.ShowChoice(actorDisplayName, evt.Message, null, evt.Options);
    }

    private void HandleActorMessageEvent(ActorMessageEvent evt)
    {
        var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
        if (evt.Actor is DemoActor avatarActor)
            uiController.ShowMessage(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Advance);
        else
            uiController.ShowMessage(actorDisplayName, evt.Message, null, evt.Advance);
    }

    private void HandleMessage(MessageEvent e)
    {
        uiController.ShowMessage(e.Actor, e.Message, null, () => e.Advance());
    }

    private void HandleChoice(ChoiceEvent e)
    {
        uiController.ShowChoice(e.Actor, e.Message, null, e.Options);
    }
}