namespace BOH
{
    // Lightweight interfaces to decouple systems and conversational nodes
    public interface IFlagService  { bool HasFlag(string flag); }
    public interface ITrustService { int  GetTrust(string npcId); }
    public interface IStoryService { int  GetChapter(); }
}

