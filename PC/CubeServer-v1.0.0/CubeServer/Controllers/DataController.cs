using CubeServer.Data;
using CubeServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CubeServer.Pages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        const bool anonymousAuth = false;

        string webApiUser = "webapi";

        // GET: api/data/getdata?lastUpdate=<datetime>
        [HttpGet("getdata")]
        public ActionResult GetData(DateTime lastUpdate)
        {
            if (anonymousAuth)
            {
                (TokenStatus tokstatus, Token token) = TokenManager.Authenticate(HttpContext);
                if (tokstatus != TokenStatus.TokenOk) return Unauthorized();
            }

            SyncPacket packet = new SyncPacket();

            if (!Global.db.GetBatchesForTest(packet.batches, lastUpdate)) return BadRequest();

            int totalCubes;
            
            foreach (Batch b in packet.batches)
            {
                // Get cube sets
                CubeSet cs = Global.db.GetCubeSet(b.CubeSetId);
                if (cs != null) 
                {
                    packet.cubeSets.Add(cs);
                }

                // get cubes
                List<Cube> cl = new List<Cube>();
                if (!Global.db.GetCubes(cl, b.ScoNum, b.Id, CubeViewOption.Untested, out totalCubes, 0, 0, lastUpdate)) return BadRequest();
                
                foreach(Cube c in cl)
                {
                    packet.cubes.Add(c);
                }

            }

            // deleted cubes
            lock (Global.app.lckDeletedCubeIds)
            {
                packet.deletedCubeIds = new List<long>(Global.app.deleteCubeIds);
                Global.app.deleteCubeIds.Clear();
            }

            return Ok(packet);
        }

        // PUT api/data/update/<barcode>
        [HttpPut("updatecube/{barcode:long}")]
        public ActionResult UpdateCubeData(int barcode, [FromBody] Cube cube)
        {
            if (anonymousAuth)
            {
                (TokenStatus tokstatus, Token token) = TokenManager.Authenticate(HttpContext);
                if (tokstatus != TokenStatus.TokenOk) return Unauthorized();
            }

            if (cube == null) return BadRequest();

            cube.Uploaded = true;

            Global.logger.LogMessageEx("Info", "Uploaded cube {0} strength = {1}kN.", cube.BarcodeStr, cube.MeasuredStrength);

            Global.db.EvaluateCube(cube);

            if (!Global.db.UpdateCube(cube, "webapi")) return BadRequest();

            return Ok(cube);
        }

    }
}
