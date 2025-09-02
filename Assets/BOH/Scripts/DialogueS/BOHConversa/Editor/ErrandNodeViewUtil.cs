// put in the same Editor folder for reuse
static class ErrandNodeViewUtil
{
    public static TField GetPrivate<TNode, TField>(TNode node, string name)
        => (TField)typeof(TNode).GetField(name, System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(node);

    public static void SetPrivate<TNode>(TNode node, string name, object value)
        => typeof(TNode).GetField(name, System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(node, value);
}