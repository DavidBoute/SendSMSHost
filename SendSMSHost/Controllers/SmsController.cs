using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SendSMSHost.Models;

namespace SendSMSHost.Controllers
{
    public class SmsController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();

        // GET: api/Sms
        public IQueryable<SmsDTO> GetSms()
        {
            var sms = db.Sms.Select(x => new SmsDTO
            {
                Id = x.Id.ToString(),
                Message = x.Message,
                Status_Id = x.StatusId,
                Status_Name = x.Status.Name,
                Contact_Id = x.ContactId.ToString(),
                Contact_FirstName = x.Contact.FirstName,
                Contact_LastName = x.Contact.LastName,
                Contact_Number = x.Contact.Number,
                TimeStamp = x.TimeStamp.ToString()
            });

            return sms;
        }

        // GET: api/Sms/5
        [ResponseType(typeof(SmsDTO))]
        public async Task<IHttpActionResult> GetSms(Guid id)
        {
            var sms = await db.Sms.Select(x => new SmsDTO
                {
                    Id = x.Id.ToString(),
                    Message = x.Message,
                    Status_Id = x.StatusId,
                    Status_Name = x.Status.Name,
                    Contact_Id = x.ContactId.ToString(),
                    Contact_FirstName = x.Contact.FirstName,
                    Contact_LastName = x.Contact.LastName,
                    Contact_Number = x.Contact.Number,
                    TimeStamp = x.TimeStamp.ToString()
                })
                .SingleOrDefaultAsync(x => x.Id == id.ToString());

            if (sms == null)
            {
                return NotFound();
            }

            return Ok(sms);
        }

        // PUT: api/Sms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSms(Guid id, Sms sms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sms.Id)
            {
                return BadRequest();
            }

            db.Entry(sms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SmsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Sms
        [ResponseType(typeof(SmsDTO))]
        public async Task<IHttpActionResult> PostSms(SmsDTO smsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sms = new Sms
            {
                Id = new Guid(smsDTO.Id),
                Message = smsDTO.Message,
                ContactId = new Guid(smsDTO.Contact_Id),
                StatusId = smsDTO.Status_Id,
                TimeStamp = DateTime.Parse(smsDTO.TimeStamp)
            };

            db.Sms.Add(sms);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SmsExists(sms.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sms.Id }, sms);
        }

        // DELETE: api/Sms/5
        [ResponseType(typeof(Sms))]
        public async Task<IHttpActionResult> DeleteSms(Guid id)
        {
            Sms sms = await db.Sms.FindAsync(id);
            if (sms == null)
            {
                return NotFound();
            }

            db.Sms.Remove(sms);
            await db.SaveChangesAsync();

            return Ok(sms);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SmsExists(Guid id)
        {
            return db.Sms.Count(e => e.Id == id) > 0;
        }
    }
}