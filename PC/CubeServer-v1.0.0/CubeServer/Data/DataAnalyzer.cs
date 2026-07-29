using CubeServer.Models;
using CubeServer.Pages;

namespace CubeServer.Data
{
    public class DataAnalyzer
    {
        public bool CalcAvgStrengthForBatches()
        {
            bool status;

            List<Batch> batches = new List<Batch>();
            status = Global.db.GetBatchesForCalc(batches);
            if (!status) return false;

            int total;
            foreach (Batch b in batches)
            {
                // get all cubes for this batch
                List<Cube> cubes = new List<Cube>();
                status = Global.db.GetCubes(cubes, b.ScoNum, b.Id, CubeViewOption.All, out total);
                if (!status) break;

                // check if all cubes for batch has been tested
                bool allTested = true;
                foreach(Cube c in cubes)
                {
                    if (c.TestResult == 0)
                    {
                        allTested = false;
                        break;
                    }
                }

                if (!allTested) continue;

                // calculate avg
                double avgStrength = 0;
                foreach (Cube c in cubes)
                {
                    avgStrength += c.MeasuredStrength;
                }
                avgStrength /= cubes.Count;
                b.AvgStrength = avgStrength;

                // update to table
                status = Global.db.UpdateAvgStrengthForBatch(b, "system");
                if (!status) continue;
            }
            
            return status;
        }

        public bool GroupBatchesWithTargetTestDates(List<Batch> batches, Dictionary<DateOnly,List<Batch>> grouped)
        {
            grouped.Clear();

            foreach(Batch b in batches)
            {
                if (b.TargetTestDate.HasValue)
                {
                    DateOnly date = DateOnly.FromDateTime((DateTime)b.TargetTestDate);
                    if (grouped.ContainsKey(date))
                    {
                        grouped[date].Add(b);
                    }
                    else
                    {
                        List<Batch> batchList = new List<Batch>();
                        batchList.Add(b);
                        grouped.Add(date, batchList);
                    }
                }
            }

            return true;
        }

        public double Average(double[] arr)
        {
            double avg = 0;
            foreach(double d in arr)
            {
                avg += d;
            }
            avg /= arr.Length;
            return avg;
        }

        public bool CalcRollingAvgStrength()
        {
            bool status;
            int total;

            List<CubeSet> cubeSets = new List<CubeSet>();
            status = Global.db.GetCubeSetsForCalc(cubeSets);
            if (!status) return false;

            foreach(CubeSet cs in cubeSets)
            {
                List<Batch> batches = new List<Batch>();
                status = Global.db.GetBatchesForCubeSet(cs.Id, batches, 28);
                if (!status) return false;

                Dictionary<DateOnly, List<Batch>> groupedBatches = new Dictionary<DateOnly, List<Batch>>();
                status = GroupBatchesWithTargetTestDates(batches, groupedBatches);
                if (!status) return false;

                double[] pastAvgStrength = new double[4];
                int idx = 0;

                foreach (DateOnly d in groupedBatches.Keys)
                {
                    double grpAvgStrength = 0;
                    int batchCnt = 0;
                    foreach (Batch b in groupedBatches[d])
                    {
                        // ignore batches that have been calculated
                        //if (b.AvgStrength > 0 && b.RollingAvgStrength > 0) continue;

                        List<Cube> cubes = new List<Cube>();
                        status = Global.db.GetCubes(cubes, b.ScoNum, b.Id, CubeViewOption.All, out total);
                        if (!status) continue;

                        // check if all cubes for batch has been tested
                        bool allTested = true;
                        foreach (Cube c in cubes)
                        {
                            if (c.TestResult == 0)
                            {
                                allTested = false;
                                break;
                            }
                        }

                        if (!allTested) continue;

                        // calculate avg
                        double avgStrength = 0;
                        foreach (Cube c in cubes)
                        {
                            avgStrength += c.MeasuredStrength;
                        }
                        avgStrength /= cubes.Count;
                        b.AvgStrength = avgStrength;

                        grpAvgStrength += avgStrength;
                        batchCnt++;
                    }

                    if (batchCnt > 0)
                    {
                        grpAvgStrength /= batchCnt;

                        if (idx < pastAvgStrength.Length)
                        {
                            pastAvgStrength[idx++] = grpAvgStrength;
                        }
                        else
                        {
                            Array.Copy(pastAvgStrength, 1, pastAvgStrength, 0, pastAvgStrength.Length - 1);
                            pastAvgStrength[idx - 1] = grpAvgStrength;
                        }

                        foreach (Batch b in groupedBatches[d])
                        {
                            if (idx < pastAvgStrength.Length)
                            {
                                b.RollingAvgStrength = 0;
                            }
                            else
                            {
                                b.RollingAvgStrength = Average(pastAvgStrength);
                            }
                        }

                        // update to table
                        foreach (Batch b in groupedBatches[d])
                        {
                            if (b.TestAge == 28)
                            {
                                b.CriterionB = b.AvgStrength >= cs.ConcreteGrade - 4 ? 'P' : 'F';
                                if (b.RollingAvgStrength > 0)
                                {
                                    b.CriterionA = b.RollingAvgStrength >= cs.ConcreteGrade + 1 ? 'P' : 'F';
                                }
                                else
                                {
                                    b.CriterionA = ' ';
                                }
                            }

                            status = Global.db.UpdateAvgStrengthForBatch(b, "system");
                            if (!status) continue;
                        }
                    }
                }

            }

            return true;

        }
    }
}
