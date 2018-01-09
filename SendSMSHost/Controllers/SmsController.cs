using AutoMapper;
using AutoMapper.QueryableExtensions;
using SendSMSHost.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SendSMSHost.Controllers
{
    public class SmsController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();

        // GET: api/Sms
        public IQueryable<SmsDTO> GetSms()
        {
            var smsList = db.Sms.OrderBy(x => x.TimeStamp).ProjectTo<SmsDTO>();

            return smsList;
        }

        //// GET: api/SmsPhone
        //public IQueryable<SmsDTO> GetSmsPhone()
        //{
        //    var statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");
        //    var smsList = db.Sms.Where(x => x.StatusId != statusCreated.Id).OrderBy(x => x.TimeStamp).ProjectTo<SmsDTO>();

        //    return smsList;
        //}

        // GET: api/Sms/5
        [ResponseType(typeof(SmsDTO))]
        public async Task<IHttpActionResult> GetSms(Guid id)
        {
            var sms = await db.Sms.ProjectTo<SmsDTO>()
                .SingleOrDefaultAsync(x => x.Id == id.ToString());

            if (sms == null)
            {
                return NotFound();
            }

            return Ok(sms);
        }

        // PUT: api/Sms
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSms(SmsDTO smsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Sms sms = Mapper.Map<Sms>(smsDTO);
            db.Set<Sms>().Attach(sms);
            db.Entry(sms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SmsExists(sms.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(smsDTO);
        }

        // POST: api/Sms
        [ResponseType(typeof(SmsDTO))]
        public async Task<IHttpActionResult> PostSms(SmsDTO smsDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // Indien ContactId == null dan opzoeken of nieuw contact voorzien
            if (String.IsNullOrWhiteSpace(smsDTO.ContactId))
            {
                // kijken of nummer al in gebruik is
                Contact contact = db.Contacts.SingleOrDefault(x => x.Number == smsDTO.ContactNumber);
                if (contact == null) // nieuw contact maken
                {
                    contact = new Contact
                    {
                        Id = Guid.NewGuid(),
                        FirstName = smsDTO.ContactNumber,
                        LastName = "",
                        Number = smsDTO.ContactNumber,
                        IsAnonymous = true
                    };

                    db.Contacts.Add(contact);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
                smsDTO.ContactId = contact.Id.ToString();
                smsDTO.ContactFirstName = contact.FirstName;
                smsDTO.ContactLastName = contact.LastName;
            }

            Sms sms = Mapper.Map<Sms>(smsDTO);

            sms.Id = Guid.NewGuid();
            sms.TimeStamp = DateTime.Now;
            sms.Status = await db.Status.SingleOrDefaultAsync(x => x.Name == "Created");
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

            smsDTO = Mapper.Map<SmsDTO>
                (
                    await db.Sms.ProjectTo<SmsDTO>()
                        .SingleOrDefaultAsync(x => x.Id == sms.Id.ToString())
                );

            return CreatedAtRoute("DefaultApi", new { id = smsDTO.Id }, smsDTO);
        }

        // DELETE: api/Sms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteSms(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Sms sms = await db.Sms.FindAsync(id);
            if (sms == null)
            {
                return NotFound();
            }

            db.Sms.Remove(sms);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return Ok();
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