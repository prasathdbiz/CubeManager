using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBase;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CubeTester.Classes
{
    public class CubeTesterDB : Database
    {
        FLogger logger;

        public CubeTesterDB()
        {
            logger = Global.logger;
        }

        public bool GetCubeSets(Dictionary<int, CubeSet> cubeSets)
        {
            string sql;
            bool status = false;
            cubeSets.Clear();

            sql = string.Format("SELECT Id, ProjectId, SpecId, " +
                "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                "FROM Cubesets ORDER BY Id;");

            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CubeSet cs = new CubeSet();

                        cs.Id = reader.SafeGetInt(0);
                        cs.ProjectId = reader.SafeGetInt(1);
                        cs.SpecId = reader.SafeGetString(2);
                        cs.TestCriteria = reader.SafeGetString(3);
                        cs.ConcreteGrade = reader.SafeGetInt(4);
                        cs.ConcreteType = reader.SafeGetString(5);
                        cs.CharacteristicStrength = reader.GetDouble(6);
                        cs.StdDeviation = reader.GetDouble(7);
                        cs.SupplierId = reader.SafeGetString(8);
                        cs.Location = reader.SafeGetString(9);
                        cs.CastingDate = reader.SafeGetDateTime(10);
                        cs.CreateUser = reader.SafeGetString(11);
                        cs.CreateDate = reader.SafeGetDateTime(12);
                        cs.LastUpdateUser = reader.SafeGetString(13);
                        cs.LastUpdate = reader.SafeGetDateTime(14);

                        cubeSets.Add(cs.Id, cs);

                        status = true;
                    }
                }
            }

            return status;
        }

        public CubeSet GetCubeSet(int cubeSetId)
        {
            CubeSet cs = null;

            string sql = "SELECT Id, ProjectId, SpecId, " +
                "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                "FROM Cubesets WHERE Id=@Id;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("Id", cubeSetId);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cs = new CubeSet();

                        cs.Id = reader.SafeGetInt(0);
                        cs.ProjectId = reader.SafeGetInt(1);
                        cs.SpecId = reader.SafeGetString(2);
                        cs.TestCriteria = reader.SafeGetString(3);
                        cs.ConcreteGrade = reader.SafeGetInt(4);
                        cs.ConcreteType = reader.SafeGetString(5);
                        cs.CharacteristicStrength = reader.GetDouble(6);
                        cs.StdDeviation = reader.GetDouble(7);
                        cs.SupplierId = reader.SafeGetString(8);
                        cs.Location = reader.SafeGetString(9);
                        cs.CastingDate = reader.SafeGetDateTime(10);
                        cs.CreateUser = reader.SafeGetString(11);
                        cs.CreateDate = reader.SafeGetDateTime(12);
                        cs.LastUpdateUser = reader.SafeGetString(13);
                        cs.LastUpdate = reader.SafeGetDateTime(14);
                    }
                }
            }

            return cs;
        }

        public bool InsertOrUpdateCubeSet(CubeSet cs, SQLiteTransaction trans = null)
        {
            bool status = false;

            string sql = "INSERT INTO CubeSets (Id, ProjectId, SpecId, " +
                "TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength, StdDeviation, " +
                "SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate) " +
                "VALUES (@Id, @ProjectId, @SpecId, " +
                "@TestCriteria, @ConcreteGrade, @ConcreteType, @CharacteristicStrength, @StdDeviation, " +
                "@SupplierId, @Location, @CastingDate, @CreateUser, @CreateDate, @LastUpdateUser, @LastUpdate) " +
                "ON CONFLICT DO UPDATE SET ProjectId=@ProjectId, SpecId=@SpecId, TestCriteria=@TestCriteria, " +
                "ConcreteGrade=@ConcreteGrade, ConcreteType=@ConcreteType, " +
                "CharacteristicStrength=@CharacteristicStrength, StdDeviation=@StdDeviation, " +
                "SupplierId=@SupplierId, Location=@Location, CastingDate=@CastingDate, " +
                "CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                "WHERE Id=@Id;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Id", cs.Id);
                cmd.Parameters.AddWithValue("ProjectId", cs.ProjectId);
                cmd.Parameters.AddWithValue("SpecId", cs.SpecId);
                cmd.Parameters.AddWithValue("TestCriteria", cs.TestCriteria);
                cmd.Parameters.AddWithValue("ConcreteGrade", cs.ConcreteGrade);
                cmd.Parameters.AddWithValue("ConcreteType", cs.ConcreteType);
                cmd.Parameters.AddWithValue("CharacteristicStrength", cs.CharacteristicStrength);
                cmd.Parameters.AddWithValue("StdDeviation", cs.StdDeviation);
                cmd.Parameters.AddWithValue("SupplierId", cs.SupplierId);
                cmd.Parameters.AddWithValue("Location", cs.Location);
                cmd.Parameters.AddWithValue("CastingDate", cs.CastingDate);
                cmd.Parameters.AddWithValue("CreateUser", cs.CreateUser);
                cmd.Parameters.AddWithValue("CreateDate", cs.CreateDate);
                cmd.Parameters.AddWithValue("LastUpdateUser", cs.LastUpdateUser);
                cmd.Parameters.AddWithValue("LastUpdate", cs.LastUpdate);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

        public bool GetBatches(Dictionary<string,Batch> batches)
        {
            bool status = false;
            batches.Clear();

            string sql = "SELECT ScoNum, Id, CubeSetId, TestAge, " +
                    "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB " +
                    "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                    "FROM Batches;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Batch b = new Batch();

                        b.ScoNum = reader.SafeGetInt(0);
                        b.Id = reader.SafeGetInt(1);
                        b.CubeSetId = reader.SafeGetInt(2);
                        b.TestAge = reader.SafeGetInt(3);
                        b.TargetTestDate = reader.SafeGetDateTime(4);
                        b.Dimension = reader.GetDouble(5);
                        b.WitnessNum = reader.SafeGetInt(6);
                        b.AvgStrength = reader.SafeGetDouble(7);
                        b.RollingAvgStrength = reader.SafeGetDouble(8);
                        b.CriterionA = reader.SafeGetChar(9);
                        b.CriterionB = reader.SafeGetChar(10);
                        b.CreateUser = reader.SafeGetString(11);
                        b.CreateDate = reader.SafeGetDateTime(12);
                        b.LastUpdateUser = reader.SafeGetString(13);
                        b.LastUpdate = reader.SafeGetDateTime(14);

                        b.BatchNum = $"{b.ScoNum}-{b.Id}";

                        batches.Add(b.BatchNum, b);

                        status = true;
                    }
                }
            }

            return status;
        }

        public Batch GetBatch(int sco, int batchId)
        {
            Batch b = null;

            string sql = "SELECT ScoNum, Id, CubeSetId, TestAge, " +
                "TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, " +
                "CreateUser, CreateDate, LastUpdateUser, LastUpdate " +
                "FROM Batches WHERE Id=@Id AND ScoNum=@ScoNum;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("Id", batchId);
                cmd.Parameters.AddWithValue("ScoNum", sco);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        b = new Batch();

                        b.ScoNum = reader.SafeGetInt(0);
                        b.Id = reader.SafeGetInt(1);
                        b.CubeSetId = reader.SafeGetInt(2);
                        b.TestAge = reader.SafeGetInt(3);
                        b.TargetTestDate = reader.SafeGetDateTime(4);
                        b.Dimension = reader.GetDouble(5);
                        b.WitnessNum = reader.SafeGetInt(6);
                        b.AvgStrength = reader.SafeGetDouble(7);
                        b.RollingAvgStrength = reader.SafeGetDouble(8);
                        b.CriterionA = reader.SafeGetChar(9);
                        b.CriterionB = reader.SafeGetChar(10);
                        b.CreateUser = reader.SafeGetString(11);
                        b.CreateDate = reader.SafeGetDateTime(12);
                        b.LastUpdateUser = reader.SafeGetString(13);
                        b.LastUpdate = reader.SafeGetDateTime(14);

                        b.BatchNum = $"{b.ScoNum}-{b.Id}";
                    }
                }
            }

            return b;
        }

        public bool InsertOrUpdateCubeSetList(List<CubeSet> cubeSets)
        {
            bool status = false;

            if (cubeSets.Count == 0) return true; // no updates

            SQLiteTransaction trans = conn.BeginTransaction();

            foreach (CubeSet cs in cubeSets)
            {
                status = InsertOrUpdateCubeSet(cs, trans);
                if (!status) break;
            }

            if (status) trans.Commit();
            else trans.Rollback();

            return status;
        }

        public bool InsertOrUpdateBatchList(List<Batch> batches)
        {
            bool status = false;

            if (batches.Count == 0) return true; // no updates

            SQLiteTransaction trans = conn.BeginTransaction();

            foreach(Batch b in batches)
            {
                status = InsertOrUpdateBatch(b, trans);
                if (!status) break;
            }

            if (status) trans.Commit();
            else trans.Rollback();

            return status;
        }

        public bool InsertOrUpdateBatch(Batch b, SQLiteTransaction trans=null)
        {
            bool status = false;

            string sql = "INSERT INTO Batches (ScoNum, Id, CubeSetId, TestAge, " +
                "TargetTestDate, Dimension, WitnessNum, CreateUser, CreateDate, LastUpdateUser, LastUpdate) " +
                "VALUES (@ScoNum, @Id, @CubeSetId, @TestAge, " +
                "@TargetTestDate, @Dimension, @WitnessNum, @CreateUser, @CreateDate, @LastUpdateUser, @LastUpdate) " +
                "ON CONFLICT DO UPDATE SET CubeSetId=@CubeSetId, " +
                "TestAge=@TestAge, TargetTestDate=@TargetTestDate, Dimension=@Dimension, " +
                "WitnessNum=@WitnessNum, CreateUser=@CreateUser, CreateDate=@CreateDate, LastUpdateUser=@LastUpdateUser, " +
                "LastUpdate=@LastUpdate " +
                "WHERE ScoNum=@ScoNum AND Id=@Id;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("ScoNum", b.ScoNum);
                cmd.Parameters.AddWithValue("Id", b.Id);
                cmd.Parameters.AddWithValue("CubeSetId", b.CubeSetId);
                cmd.Parameters.AddWithValue("TestAge", b.TestAge);
                cmd.Parameters.AddWithValue("TargetTestDate", b.TargetTestDate);
                cmd.Parameters.AddWithValue("Dimension", b.Dimension);
                cmd.Parameters.AddWithValue("WitnessNum", b.WitnessNum);
                cmd.Parameters.AddWithValue("CreateUser", b.CreateUser);
                cmd.Parameters.AddWithValue("CreateDate", b.CreateDate);
                cmd.Parameters.AddWithValue("LastUpdateUser", b.LastUpdateUser);
                cmd.Parameters.AddWithValue("LastUpdate", b.LastUpdate);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

        public Cube GetCube(long barcode)
        {
            string sql;
            Cube c = null;

            sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6, " +
                    "AvgDimension, MeasuredMaxForce, " +
                    "MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime " +
                    "FROM Cubes WHERE Barcode=@Barcode");
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("Barcode", barcode);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        c = new Cube();

                        c.Id = reader.SafeGetLong(0);
                        c.Barcode = reader.SafeGetLong(1);
                        c.SampleRef = reader.SafeGetString(2);
                        c.ScoNum = reader.SafeGetInt(3);
                        c.BatchId = reader.SafeGetInt(4);
                        c.MeasuredDimX1 = reader.SafeGetDouble(5);
                        c.MeasuredDimX2 = reader.SafeGetDouble(6);
                        c.MeasuredDimX3 = reader.SafeGetDouble(7);
                        c.MeasuredDimX4 = reader.SafeGetDouble(8);
                        c.MeasuredDimX5 = reader.SafeGetDouble(9);
                        c.MeasuredDimX6 = reader.SafeGetDouble(10);
                        c.MeasuredDimY1 = reader.SafeGetDouble(11);
                        c.MeasuredDimY2 = reader.SafeGetDouble(12);
                        c.MeasuredDimY3 = reader.SafeGetDouble(13);
                        c.MeasuredDimY4 = reader.SafeGetDouble(14);
                        c.MeasuredDimY5 = reader.SafeGetDouble(15);
                        c.MeasuredDimY6 = reader.SafeGetDouble(16);
                        c.AvgDimension = reader.SafeGetDouble(17);
                        c.MeasuredMaxForce = reader.SafeGetDouble(18);
                        c.MeasuredStrength = reader.SafeGetDouble(19);
                        c.MeasuredWeight = reader.SafeGetDouble(20);
                        c.MeasuredDensity = reader.SafeGetDouble(21);
                        c.TesterId = reader.SafeGetInt(22);
                        c.TestResult = reader.SafeGetInt(23);
                        c.StatusCode = reader.SafeGetString(24);
                        c.Uploaded = reader.SafeGetInt(25) > 0;
                        c.ActualTestDate = reader.SafeGetDateTime(26);
                        c.UploadTime = reader.SafeGetDateTime(27);

                        c.BatchNum = $"{c.ScoNum}-{c.BatchId}";
                    }
                }
            }

            return c;
        }

        public bool GetCubes(List<Cube> cubes, string batchNum="", DateTime? dt=null)
        {
            string sql;
            bool status = false;
            cubes.Clear();
            string sqlwhere = "";

            if (batchNum.Length > 0)
            {
                string[] s = batchNum.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Length != 2) return false;

                int sco, batchId;
                if (int.TryParse(s[0], out sco) && int.TryParse(s[1], out batchId))
                {
                    sqlwhere = $"WHERE ScoNum={sco} AND BatchId={batchId}";
                }
                else
                {
                    return false;
                }

                if (dt != null)
                {
                    sqlwhere += $" AND DATE(LastUpdate)=DATE(@LastUpdate)";
                }

            }

            status = true;
            sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6, " +
                    "AvgDimension, MeasuredMaxForce, " +
                    "MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime " +
                    "FROM Cubes {0} ORDER BY LastUpdate DESC;", sqlwhere);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                if (dt != null)
                {
                    cmd.Parameters.AddWithValue("LastUpdate", (DateTime)dt);
                }

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cube c = new Cube();

                        c.Id = reader.SafeGetLong(0);
                        c.Barcode = reader.SafeGetLong(1);
                        c.SampleRef = reader.SafeGetString(2);
                        c.ScoNum = reader.SafeGetInt(3);
                        c.BatchId = reader.SafeGetInt(4);
                        c.MeasuredDimX1 = reader.SafeGetDouble(5);
                        c.MeasuredDimX2 = reader.SafeGetDouble(6);
                        c.MeasuredDimX3 = reader.SafeGetDouble(7);
                        c.MeasuredDimX4 = reader.SafeGetDouble(8);
                        c.MeasuredDimX5 = reader.SafeGetDouble(9);
                        c.MeasuredDimX6 = reader.SafeGetDouble(10);
                        c.MeasuredDimY1 = reader.SafeGetDouble(11);
                        c.MeasuredDimY2 = reader.SafeGetDouble(12);
                        c.MeasuredDimY3 = reader.SafeGetDouble(13);
                        c.MeasuredDimY4 = reader.SafeGetDouble(14);
                        c.MeasuredDimY5 = reader.SafeGetDouble(15);
                        c.MeasuredDimY6 = reader.SafeGetDouble(16);
                        c.AvgDimension = reader.SafeGetDouble(17);
                        c.MeasuredMaxForce = reader.SafeGetDouble(18);
                        c.MeasuredStrength = reader.SafeGetDouble(19);
                        c.MeasuredWeight = reader.SafeGetDouble(20);
                        c.MeasuredDensity = reader.SafeGetDouble(21);
                        c.TesterId = reader.SafeGetInt(22);
                        c.TestResult = reader.SafeGetInt(23);
                        c.StatusCode = reader.SafeGetString(24);
                        c.Uploaded = reader.SafeGetInt(25) > 0;
                        c.ActualTestDate = reader.SafeGetDateTime(26);
                        c.UploadTime = reader.SafeGetDateTime(27);

                        c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                        cubes.Add(c);

                        status = true;
                    }
                }
            }

            return status;
        }

        public bool GetUntestedCubes(List<Cube> cubes)
        {
            string sql;
            bool status = false;
            cubes.Clear();
            string sqlwhere = "";

            status = true;
            sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6, " +
                    "AvgDimension, MeasuredMaxForce, " +
                    "MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime " +
                    "FROM Cubes WHERE TestResult=0 {0} ORDER BY LastUpdate DESC;", sqlwhere);
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cube c = new Cube();

                        c.Id = reader.SafeGetLong(0);
                        c.Barcode = reader.SafeGetLong(1);
                        c.SampleRef = reader.SafeGetString(2);
                        c.ScoNum = reader.SafeGetInt(3);
                        c.BatchId = reader.SafeGetInt(4);
                        c.MeasuredDimX1 = reader.SafeGetDouble(5);
                        c.MeasuredDimX2 = reader.SafeGetDouble(6);
                        c.MeasuredDimX3 = reader.SafeGetDouble(7);
                        c.MeasuredDimX4 = reader.SafeGetDouble(8);
                        c.MeasuredDimX5 = reader.SafeGetDouble(9);
                        c.MeasuredDimX6 = reader.SafeGetDouble(10);
                        c.MeasuredDimY1 = reader.SafeGetDouble(11);
                        c.MeasuredDimY2 = reader.SafeGetDouble(12);
                        c.MeasuredDimY3 = reader.SafeGetDouble(13);
                        c.MeasuredDimY4 = reader.SafeGetDouble(14);
                        c.MeasuredDimY5 = reader.SafeGetDouble(15);
                        c.MeasuredDimY6 = reader.SafeGetDouble(16);
                        c.AvgDimension = reader.SafeGetDouble(17);
                        c.MeasuredMaxForce = reader.SafeGetDouble(18);
                        c.MeasuredStrength = reader.SafeGetDouble(19);
                        c.MeasuredWeight = reader.SafeGetDouble(20);
                        c.MeasuredDensity = reader.SafeGetDouble(21);
                        c.TesterId = reader.SafeGetInt(22);
                        c.TestResult = reader.SafeGetInt(23);
                        c.StatusCode = reader.SafeGetString(24);
                        c.Uploaded = reader.SafeGetInt(25) > 0;
                        c.ActualTestDate = reader.SafeGetDateTime(26);
                        c.UploadTime = reader.SafeGetDateTime(27);

                        c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                        cubes.Add(c);

                        status = true;
                    }
                }
            }

            return status;
        }


        public bool GetCubesForUpload(List<Cube> cubes)
        {
            string sql;
            bool status = false;
            cubes.Clear();

            status = true;
            sql = string.Format("SELECT Id, Barcode, SampleRef, ScoNum, BatchId, " +
                    "MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6, " +
                    "MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6, " +
                    "AvgDimension, MeasuredMaxForce, " +
                    "MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId, " +
                    "TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime " +
                    "FROM Cubes WHERE TestResult>0 AND Uploaded=0;");
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cube c = new Cube();

                        c.Id = reader.SafeGetLong(0);
                        c.Barcode = reader.SafeGetLong(1);
                        c.SampleRef = reader.SafeGetString(2);
                        c.ScoNum = reader.SafeGetInt(3);
                        c.BatchId = reader.SafeGetInt(4);
                        c.MeasuredDimX1 = reader.SafeGetDouble(5);
                        c.MeasuredDimX2 = reader.SafeGetDouble(6);
                        c.MeasuredDimX3 = reader.SafeGetDouble(7);
                        c.MeasuredDimX4 = reader.SafeGetDouble(8);
                        c.MeasuredDimX5 = reader.SafeGetDouble(9);
                        c.MeasuredDimX6 = reader.SafeGetDouble(10);
                        c.MeasuredDimY1 = reader.SafeGetDouble(11);
                        c.MeasuredDimY2 = reader.SafeGetDouble(12);
                        c.MeasuredDimY3 = reader.SafeGetDouble(13);
                        c.MeasuredDimY4 = reader.SafeGetDouble(14);
                        c.MeasuredDimY5 = reader.SafeGetDouble(15);
                        c.MeasuredDimY6 = reader.SafeGetDouble(16);
                        c.AvgDimension = reader.SafeGetDouble(17);
                        c.MeasuredMaxForce = reader.SafeGetDouble(18);
                        c.MeasuredStrength = reader.SafeGetDouble(19);
                        c.MeasuredWeight = reader.SafeGetDouble(20);
                        c.MeasuredDensity = reader.SafeGetDouble(21);
                        c.TesterId = reader.SafeGetInt(22);
                        c.TestResult = reader.SafeGetInt(23);
                        c.StatusCode = reader.SafeGetString(24);
                        c.Uploaded = reader.SafeGetInt(25) > 0;
                        c.ActualTestDate = reader.SafeGetDateTime(26);
                        c.UploadTime = reader.SafeGetDateTime(27);

                        c.BatchNum = $"{c.ScoNum}-{c.BatchId}";

                        cubes.Add(c);

                        status = true;
                    }
                }
            }

            return status;
        }

        public bool UpdateCube(Cube c)
        {
            bool status = false;

            string sql = "UPDATE Cubes SET Barcode=@Barcode, SampleRef=@SampleRef, ScoNum=@ScoNum, BatchId=@BatchId, " +
                    "MeasuredDimX1=@MeasuredDimX1, MeasuredDimX2=@MeasuredDimX2, MeasuredDimX3=@MeasuredDimX3, " +
                    "MeasuredDimX4=@MeasuredDimX4, MeasuredDimX5=@MeasuredDimX5, MeasuredDimX6=@MeasuredDimX6, " +
                    "MeasuredDimY1=@MeasuredDimY1, MeasuredDimY2=@MeasuredDimY2, MeasuredDimY3=@MeasuredDimY3, " +
                    "MeasuredDimY4=@MeasuredDimY4, MeasuredDimY5=@MeasuredDimY5, MeasuredDimY6=@MeasuredDimY6," +
                    "AvgDimension=@AvgDimension, MeasuredMaxForce=@MeasuredMaxForce, MeasuredStrength=@MeasuredStrength, " +
                    "MeasuredWeight=@MeasuredWeight, MeasuredDensity=@MeasuredDensity, TesterId=@TesterId, " +
                    "TestResult=@TestResult, StatusCode=@StatusCode, Uploaded=@Uploaded, ActualTestDate=@ActualTestDate, " +
                    "UploadTime=@UploadTime, LastUpdateUser=@LastUpdateUser " +
                    "WHERE Id=@Id;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("Id", c.Id);
                cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                cmd.Parameters.AddWithValue("MeasuredDimX1", c.MeasuredDimX1);
                cmd.Parameters.AddWithValue("MeasuredDimX2", c.MeasuredDimX2);
                cmd.Parameters.AddWithValue("MeasuredDimX3", c.MeasuredDimX3);
                cmd.Parameters.AddWithValue("MeasuredDimX4", c.MeasuredDimX4);
                cmd.Parameters.AddWithValue("MeasuredDimX5", c.MeasuredDimX5);
                cmd.Parameters.AddWithValue("MeasuredDimX6", c.MeasuredDimX6);
                cmd.Parameters.AddWithValue("MeasuredDimY1", c.MeasuredDimY1);
                cmd.Parameters.AddWithValue("MeasuredDimY2", c.MeasuredDimY2);
                cmd.Parameters.AddWithValue("MeasuredDimY3", c.MeasuredDimY3);
                cmd.Parameters.AddWithValue("MeasuredDimY4", c.MeasuredDimY4);
                cmd.Parameters.AddWithValue("MeasuredDimY5", c.MeasuredDimY5);
                cmd.Parameters.AddWithValue("MeasuredDimY6", c.MeasuredDimY6);
                cmd.Parameters.AddWithValue("AvgDimension", c.AvgDimension);
                cmd.Parameters.AddWithValue("MeasuredMaxForce", c.MeasuredMaxForce);
                cmd.Parameters.AddWithValue("MeasuredStrength", c.MeasuredStrength);
                cmd.Parameters.AddWithValue("MeasuredWeight", c.MeasuredWeight);
                cmd.Parameters.AddWithValue("MeasuredDensity", c.MeasuredDensity);
                cmd.Parameters.AddWithValue("TesterId", c.TesterId);
                cmd.Parameters.AddWithValue("TestResult", c.TestResult);
                cmd.Parameters.AddWithValue("StatusCode", c.StatusCode);
                cmd.Parameters.AddWithValue("Uploaded", c.Uploaded);
                cmd.Parameters.AddWithValue("ActualTestDate", c.ActualTestDate);
                cmd.Parameters.AddWithValue("UploadTime", c.UploadTime);
                cmd.Parameters.AddWithValue("LastUpdateUser", "webapi");

                status = (cmd.ExecuteNonQuery() == 1);
            }


            return status;
        }

        public bool InsertOrUpdateCubeList(List<Cube> cubes)
        {
            bool status = false;

            if (cubes.Count == 0) return true; // nothing to update

            SQLiteTransaction trans = conn.BeginTransaction();

            foreach (Cube c in cubes)
            {
                Cube curCube = GetCube(c.Barcode);
                if (curCube == null || curCube.LastUpdate < c.LastUpdate)
                {
                    status = InsertOrUpdateCube(c, trans);
                    if (!status) break;
                }
            }

            if (status) trans.Commit();
            else trans.Rollback();

            return status;
        }

        public bool InsertOrUpdateCube(Cube c, SQLiteTransaction trans=null)
        {
            bool status = false;

            string sql = "INSERT INTO Cubes (Id, Barcode, SampleRef, ScoNum, BatchId, MeasuredStrength, " +
                    "MeasuredDensity, TestResult, StatusCode, LastUpdateUser, LastUpdate) " +
                    "VALUES (@Id, @Barcode, @SampleRef, @ScoNum, @BatchId, @MeasuredStrength, " +
                    "@MeasuredDensity, @TestResult, @StatusCode, @LastUpdateUser, @LastUpdate) " +
                    "ON CONFLICT DO UPDATE SET Barcode=@Barcode, SampleRef=@SampleRef, ScoNum=@ScoNum, BatchId=@BatchId, " +
                    "MeasuredStrength=@MeasuredStrength, MeasuredDensity=@MeasuredDensity, " +
                    "TestResult=@TestResult, StatusCode=@StatusCode, " +
                    "LastUpdateUser=@LastUpdateUser, LastUpdate=@LastUpdate " +
                    "WHERE Id=@Id;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Id", c.Id);
                cmd.Parameters.AddWithValue("Barcode", c.Barcode);
                cmd.Parameters.AddWithValue("SampleRef", c.SampleRef);
                cmd.Parameters.AddWithValue("ScoNum", c.ScoNum);
                cmd.Parameters.AddWithValue("BatchId", c.BatchId);
                cmd.Parameters.AddWithValue("MeasuredStrength", c.MeasuredStrength);
                cmd.Parameters.AddWithValue("MeasuredDensity", c.MeasuredDensity);
                cmd.Parameters.AddWithValue("TestResult", c.TestResult);
                cmd.Parameters.AddWithValue("StatusCode", c.StatusCode);
                cmd.Parameters.AddWithValue("LastUpdateUser", c.LastUpdateUser);
                cmd.Parameters.AddWithValue("LastUpdate", c.LastUpdate);

                status = (cmd.ExecuteNonQuery() == 1);
            }

            return status;
        }

        public bool DeleteCube(long barcode)
        {
            bool status;

            string sql = "DELETE FROM Cubes WHERE Barcode=@Barcode;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("Barcode", barcode);

                status = (cmd.ExecuteNonQuery() > 0);
            }

            return status;
        }

        public bool ClearCubeSetTable()
        {
            string sql = "DELETE FROM CubeSets;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool ClearBatchTable()
        {
            string sql = "DELETE FROM Batches;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public bool ClearCubeTable()
        {
            string sql = "DELETE FROM Cubes;";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn, trans))
            {
                cmd.ExecuteNonQuery();
            }

            return true;
        }
    }
}
