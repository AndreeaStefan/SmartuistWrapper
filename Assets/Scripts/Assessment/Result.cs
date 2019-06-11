namespace Assessment
{
    public class Result
    {
        public int targetIndex;
        public float targetSize;
        public float targetDepth;
        public float movementTime;
        public string mappingType;

        public Result(int targetIndex, float targetSize, float targetDepth, float time, string mappping)
        {
            this.targetIndex = targetIndex;
            this.targetDepth = targetDepth;
            this.targetSize = targetSize;
            movementTime = time;
            mappingType = mappping;
        }
    }
}