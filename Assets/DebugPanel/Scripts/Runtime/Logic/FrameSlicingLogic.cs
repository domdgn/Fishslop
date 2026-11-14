using DebugPanel.Datas;

namespace DebugPanel.Logic
{
    public sealed class FrameSlicingLogic
    {
        readonly FrameSlicingData _frameSlicingData;

        public FrameSlicingLogic(FrameSlicingData frameSlicingData)
        {
            _frameSlicingData = frameSlicingData;
        }

        public void SetMaxMsPerFrame(int maxMsPerFrame)
        {
            _frameSlicingData.MaxMsPerFrame = maxMsPerFrame;
        }
    }
}