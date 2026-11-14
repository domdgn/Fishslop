namespace DebugPanel.Datas
{
    public readonly struct MemberCreationData
    {
        public readonly bool ShowAsDebugAction;
        public readonly bool DontShowAsDebugAction;

        public MemberCreationData(bool showAsDebugAction, bool dontShowAsDebugAction)
        {
            ShowAsDebugAction = showAsDebugAction;
            DontShowAsDebugAction = dontShowAsDebugAction;
        }
    }
}