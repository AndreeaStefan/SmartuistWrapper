using System.Collections.Generic;
using Assets.Scripts.Assessment;

namespace Assessment.Results
{
    public class FrameResultDb
    {
        private static List<FrameResult> Current = new List<FrameResult>();
        private static List<FrameResult> Previous = new List<FrameResult>();

        public static void AddResult(FrameResult fr)
        {
            Current.Add(fr);
        }

        public static List<FrameResult> GetCurrent()
        {
            return Current;
        }

        public static List<FrameResult> GetPrevious()
        {
            return Previous;
        }

        public static void NewRep()
        {
            Previous = Current;
            Current = new List<FrameResult>();
        }
    }
}