using Conversa.Demo.Scripts;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine;

public class MyConversaController : MonoBehaviour
{
    [SerializeField] private Conversation conversation;
    [SerializeField] private UIController uiController;

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
            case EndEvent _:
                HandleEnd();
                break;
        }
    }

    private void HandleEnd()
    {
        throw new System.NotImplementedException();
    }

    private void HandleUserEvent(UserEvent userEvent)
    {
        throw new System.NotImplementedException();
    }

    private void HandleActorChoiceEvent(ActorChoiceEvent actorChoiceEvent)
    {
        throw new System.NotImplementedException();
    }

    private void HandleActorMessageEvent(ActorMessageEvent actorMessageEvent)
    {
        throw new System.NotImplementedException();
    }

    private void HandleMessage(MessageEvent messageEvent)
    {
        throw new System.NotImplementedException();
    }

    private void HandleChoice(ChoiceEvent choiceEvent)
    {
        throw new System.NotImplementedException();
    }
}