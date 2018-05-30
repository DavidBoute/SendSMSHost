using SendSMSHost.Models;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SendSMSHost.Controllers
{
    public class StatusController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();

        // GET: api/Status
        public IQueryable<StatusDTO> GetStatus()
        {
            var statusList = db.Status
                            .OrderBy(x => x.Id)
                            .Select(x=> new StatusDTO(x));
            return statusList;
        }

        // GET: api/Status/5
        [ResponseType(typeof(StatusDTO))]
        public async Task<IHttpActionResult> GetStatus(int id)
        {
            var status = await db.Status.Select(x => new StatusDTO(x))
                .SingleOrDefaultAsync(x => x.Id == id);

            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StatusExists(int id)
        {
            return db.Status.Count(e => e.Id == id) > 0;
        }
    }
}