﻿using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using SendSMSHost.Models;

using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace SendSMSHost.Controllers
{
    public class SmsController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();

        // GET: api/Sms
        public IQueryable<SmsDTO> GetSms()
        {
            var sms = db.Sms.OrderBy(x=> x.TimeStamp).ProjectTo<SmsDTO>();

            return sms;
        }

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

        // PUT: api/Sms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSms(Guid id, SmsDTO smsDTO)
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
                if (!SmsExists(id))
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
        public async Task<IHttpActionResult> DeleteSms(string id)
        {
            Guid guid = new Guid(id);
            Sms sms = await db.Sms.FindAsync(guid);
            if (sms == null)
            {
                return NotFound();
            }

            db.Sms.Remove(sms);
            await db.SaveChangesAsync();

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