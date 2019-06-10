namespace Assessment
{
    public class Result
    {
        public float targetSize;
        public float targetDepth;
        public float movementTime;
        public string mappingType;

        public Result(float targetSize, float targetDepth, float time, string mappping)
        {
            this.targetDepth = targetDepth;
            this.targetSize = targetSize;
            movementTime = time;
            mappingType = mappping;
        }
    }
}