namespace CubeServer.Models
{
    // for reporting
    public class CubeTest
    {
        public CubeSet cubeSet;
        public Batch batch;
        public List<Cube> cubes;

        public void CalcAvgStrength()
        {
            double avg = 0;

            foreach(Cube c in cubes)
            {
                avg += c.MeasuredStrength;
            }

            batch.AvgStrength = avg / cubes.Count;
        }
    }
}
